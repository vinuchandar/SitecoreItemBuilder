using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OneNorth.SitecoreItemBuilder.CreateCustomItem.Models;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework.Commands;
using CustomItem = OneNorth.SitecoreItemBuilder.CreateCustomItem.Models.CustomItem;

namespace OneNorth.SitecoreItemBuilder.CreateCustomItem
{
    public class Helper
    {
        readonly Database[] _databases = new Database[1] { Sitecore.Configuration.Factory.GetDatabase("web") };
        public void AddComponentsToItem(Item item)
        {
            if (item == null)
                return;

            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var customItemSetting = Settings.Entities.FirstOrDefault(x => x.TemplateId == item.TemplateID);
            if (customItemSetting == null || item.Database.Name != masterDb.Name)
                return;

            try
            {
                    foreach (var component in customItemSetting.Components)
                    {
                        InsertComponents(item, component, item);
                    }

                    InsertRenderings(item, customItemSetting);

            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error in " + GetType().Name, ex, this);
            }
        }

        public void InsertComponents(Item item, Component component, Item entityItem)
        {
            using (new SecurityDisabler())
            {
                try
                {
                    var componentItem = item.Add(component.ItemName, new TemplateID(component.TemplateId));

                    componentItem.Editing.BeginEdit();

                    componentItem.Appearance.DisplayName = component.DisplayName;

                    componentItem.Appearance.Sortorder = component.SortOrder;

                    if (!string.IsNullOrEmpty(component.Fields))
                    {
                        var fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(component.Fields);
                        foreach (var field in fields)
                        {
                            if (componentItem[field.Key] != null)
                            {
                                componentItem[field.Key] = GetComponentField(field.Value, entityItem);
                            }
                        }
                    }

                    componentItem.Editing.EndEdit();
                    PublishItem(componentItem);
                    

                    if (component.Components.Any())
                    {
                        foreach (var childComponent in component.Components)
                        {
                            InsertComponents(componentItem, childComponent, entityItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error("Error in " + GetType().Name, ex, this);
                }
            }
        }

        public void PublishItem(Item item)
        {
            string domainUser = @"sitecore\admin";

            if (Sitecore.Security.Accounts.User.Exists(domainUser))
            {
                Sitecore.Security.Accounts.User user =
                    Sitecore.Security.Accounts.User.FromName(domainUser, false);

                using (new Sitecore.Security.Accounts.UserSwitcher(user))
                {
                    Sitecore.Publishing.PublishManager.PublishItem(item, _databases, item.Languages, true, false);
                }
            }

        }

        public string GetComponentField(string value, Item entityItem)
        {
            if (value.Equals("UseEntityName", StringComparison.InvariantCultureIgnoreCase))
                return entityItem["Name"];
            if (value.Equals("UseEntityOverview", StringComparison.InvariantCultureIgnoreCase))
                return entityItem["Overview"];
            if (value.Equals("UseEntityDescription", StringComparison.InvariantCultureIgnoreCase))
                return entityItem["Description"];
            return value;
        }

        public void InsertRenderings(Item item, CustomItem customitem)
        {
            IEnumerable<Rendering> renderings = customitem.Renderings;
            using (new SecurityDisabler())
            {
                try
                {
                    item.Editing.BeginEdit();

                    LayoutField layoutField = new LayoutField(item.Fields[Sitecore.FieldIDs.FinalLayoutField]);
                    LayoutDefinition layoutDefinition = new LayoutDefinition();// LayoutDefinition.Parse(layoutField.Value);
                    DeviceDefinition deviceDefinition = layoutDefinition.GetDevice(customitem.DeviceId);
                    deviceDefinition.Layout = customitem.LayoutId;

                    renderings.SelectMany(x => GenerateRenderings(item, x)).ToList().ForEach(r => deviceDefinition.AddRendering(r));

                    item.Fields[Sitecore.FieldIDs.FinalLayoutField].Value = layoutDefinition.ToXml().Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "xmlns:p=\"p\" xmlns:s=\"s\" p:p=\"1\"");

                    item.Editing.EndEdit();      
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error("Error in " + GetType().Name, ex, this);
                }
            }
        }

        public IEnumerable<RenderingDefinition> GenerateRenderings(Item item, Rendering rendering, string placeholderValue = "")
        {
            var renderingDefinitions = new List<RenderingDefinition>();

            var renderingDefinition = GenerateRendering(item, rendering, placeholderValue);

            renderingDefinitions.Add(renderingDefinition);            

            foreach (var childRendering in rendering.Renderings)
            {
                placeholderValue = renderingDefinition.Placeholder + string.Format(childRendering.Placeholder, renderingDefinition.UniqueId);
                renderingDefinitions.AddRange(GenerateRenderings(item, childRendering, placeholderValue));
            }

            return renderingDefinitions;
        }

        public RenderingDefinition GenerateRendering(Item item, Rendering rendering, string placeholderValue = "")
        {
            RenderingDefinition renderingDefinition = new RenderingDefinition();             

            renderingDefinition.ItemID = rendering.Id;

            if (rendering.HasDataSource)
            {
                var dataSourceItem = item.Database.GetItem(item.Paths.Path + rendering.DataSource);
                if (dataSourceItem == null)
                    dataSourceItem = item.Database.GetItem(rendering.DataSource);
                renderingDefinition.Datasource = dataSourceItem != null
                    ? dataSourceItem.ID.ToString()
                    : string.Empty;
            }

            renderingDefinition.Placeholder = !string.IsNullOrEmpty(placeholderValue) ? placeholderValue : rendering.Placeholder;

            return renderingDefinition;
        }
    }
}
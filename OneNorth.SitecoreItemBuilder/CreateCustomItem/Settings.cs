using System.Collections.Generic;
using System.Xml;
using OneNorth.SitecoreItemBuilder.CreateCustomItem.Models;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Xml;

namespace OneNorth.SitecoreItemBuilder.CreateCustomItem
{
    public class Settings
    {
        private static List<CustomItem> _repositoryEntities;
                
        public static List<CustomItem> Entities
        {
            get
            {
                if (_repositoryEntities == null)
                {
                    var entityElements = new List<CustomItem>();

                    foreach (XmlNode node in Factory.GetConfigNodes("customEntities/customEntity"))
                    {
                        var entity = new CustomItem();

                        var templateId = XmlUtil.GetAttribute("templateId", node);
                        if (!string.IsNullOrEmpty(templateId))
                            entity.TemplateId = new ID(templateId);

                        entity.DeviceId = XmlUtil.GetAttribute("deviceId", node);
                        
                        entity.LayoutId = XmlUtil.GetAttribute("layoutId", node);
                        
                        var components = new List<Component>();
                        var componentsNode = node.SelectSingleNode("descendant::components");
                        foreach (XmlNode componentNode in componentsNode.ChildNodes)
                        {
                            components.Add(new Component(componentNode));
                        }
                        entity.Components = components;

                        var renderings = new List<Rendering>();
                        var renderingsNode = node.SelectSingleNode("descendant::renderings");
                        foreach (XmlNode renderingNode in renderingsNode.ChildNodes)
                        {
                            renderings.Add(new Rendering(renderingNode));
                        }
                        entity.Renderings = renderings;

                        entityElements.Add(entity);
                    }

                    _repositoryEntities = entityElements;
                }

                return _repositoryEntities;
            }
        }
    }
}
using System;
using OneNorth.SitecoreItemBuilder.CreateCustomItem;
using Sitecore.Data.Events;
using Sitecore.Events;

namespace OneNorth.SitecoreItemBuilder.Pipelines
{
    public class CreateCustomItem
    {
        private Helper helper { get; set; }

        protected void OnItemCreated(object sender, EventArgs args)
        {
            if (args == null)
                return;
            helper = new Helper();
            helper.AddComponentsToItem(Event.ExtractParameter<ItemCreatedEventArgs>(args, 0).Item);
        }
    }
}
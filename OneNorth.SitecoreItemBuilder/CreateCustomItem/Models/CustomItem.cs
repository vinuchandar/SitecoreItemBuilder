using System.Collections.Generic;
using Sitecore.Data;

namespace OneNorth.SitecoreItemBuilder.CreateCustomItem.Models
{
    public class CustomItem
    {
        public ID TemplateId { get; set; }
        public IEnumerable<Component> Components { get; set; }
        public IEnumerable<Rendering> Renderings { get; set; }
        public string DeviceId { get; set; }
        public string LayoutId { get; set; }
    }
}
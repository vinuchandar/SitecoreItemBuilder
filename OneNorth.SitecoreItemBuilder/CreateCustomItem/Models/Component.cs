using System.Collections.Generic;
using System.Xml;
using Sitecore.Data;
using Sitecore.Xml;

namespace OneNorth.SitecoreItemBuilder.CreateCustomItem.Models
{
    public class Component
    {
        public ID TemplateId { get; set; }
        public string ItemName { get; set; }
        public string DisplayName { get; set; }
        public int SortOrder { get; set; }
        public string Fields { get; set; }
        public IEnumerable<Component> Components { get; set; }

        public Component() { }
        public Component(XmlNode node)
        {
            ID templateId;            
            TemplateId = (ID.TryParse(XmlUtil.GetAttribute("templateId", node, string.Empty), out templateId))
                ? templateId
                : ID.Undefined;

            ItemName = XmlUtil.GetAttribute("itemName", node, string.Empty);

            DisplayName = XmlUtil.GetAttribute("displayName", node, string.Empty);

            SortOrder = int.Parse(XmlUtil.GetAttribute("sortOrder", node, "100"));

            Fields = XmlUtil.GetAttribute("fields", node, string.Empty);

            var components = new List<Component>();
            if (node.HasChildNodes)
            {                
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    components.Add(new Component(childNode));
                }                
            }
            Components = components;
        }
    }
}
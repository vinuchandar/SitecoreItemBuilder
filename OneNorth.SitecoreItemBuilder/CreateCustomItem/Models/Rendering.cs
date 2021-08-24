using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Xml;

namespace OneNorth.SitecoreItemBuilder.CreateCustomItem.Models
{
    public class Rendering
    {        
        public string Id { get; set; }
        public string Placeholder { get; set; }
        public string DataSource { get; set; }        
        public bool HasDataSource { get; set; }
        public IEnumerable<Rendering> Renderings { get; set; }

        public Rendering() { }
        public Rendering(XmlNode node, string datasource = "")
        {
            Id = XmlUtil.GetAttribute("id", node, string.Empty);

            DataSource = datasource + XmlUtil.GetAttribute("datasource", node, string.Empty);

            Placeholder = XmlUtil.GetAttribute("placeholder", node, string.Empty);

            HasDataSource = !String.IsNullOrWhiteSpace(DataSource);

            var renderings = new List<Rendering>();
            if (node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    renderings.Add(new Rendering(childNode, DataSource));
                }
            }
            Renderings = renderings;
        }
    }
}
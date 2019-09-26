using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Fusi.Tools;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// Data profile. This includes metadata about facets and flags definitions.
    /// </summary>
    public class DataProfile
    {
        /// <summary>
        /// Gets the facets.
        /// </summary>
        public IFacet[] Facets { get; private set; }

        /// <summary>
        /// Gets the flags definitions.
        /// </summary>
        public IFlagDefinition[] Flags { get; private set; }

        /// <summary>
        /// Gets the tag sets.
        /// </summary>
        public TagSet[] TagSets { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="DataProfile"/>.
        /// </summary>
        public DataProfile()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DataProfile"/>.
        /// </summary>
        /// <param name="profile"></param>
        public DataProfile(XElement profile)
        {
            Load(profile);
        }

        private static IFacet[] LoadFacets(XElement element)
        {
            if (element == null) return new IFacet[0];

            List<IFacet> facets = new List<IFacet>();
            foreach (XElement facetElement in element.Elements("facet"))
            {
                // @id @label
                // dsc?, parts?/part+
                IFacet facet = new Facet
                {
                    Id = facetElement.Attribute("id").Value,
                    Label = facetElement.Attribute("label").Value,
                    Description = facetElement.Element("dsc")?.Value
                };

                // parts
                XElement partsElement = facetElement.Element("parts");
                if (partsElement != null)
                {
                    foreach (XElement partElement in partsElement.Elements("part"))
                    {
                        facet.PartDefinitions.Add(new PartDefinition
                        {
                            TypeId = partElement.Attribute("type").Value,
                            RoleId = partElement.ReadOptionalAttribute("role", null),
                            Name = partElement.ReadOptionalAttribute("name", null),
                            IsRequired = partElement.ReadOptionalAttribute("required", false),
                            // IsTextLayer = xePart.ReadOptionalAttribute("layer", false),
                            ColorKey = partElement.ReadOptionalAttribute("color", null),
                            GroupKey = partElement.ReadOptionalAttribute("group", null),
                            SortKey = partElement.ReadOptionalAttribute("sort", null),
                            Description = partElement.Value.Length > 0 ? partElement.Value : null
                        });
                    }
                }

                facets.Add(facet);
            }

            return facets.ToArray();
        }

        private static IFlagDefinition[] LoadFlags(XElement element)
        {
            if (element == null) return new IFlagDefinition[0];

            List<IFlagDefinition> flags = new List<IFlagDefinition>();
            foreach (XElement flagElement in element.Elements("flag"))
            {
                // @id @label @color
                IFlagDefinition flag = new FlagDefinition
                {
                    Id = int.Parse(flagElement.Attribute("id").Value, NumberStyles.HexNumber),
                    Label = flagElement.Attribute("label").Value,
                    ColorKey = flagElement.Attribute("color").Value,
                    Description = flagElement.Value.Length > 0 ? flagElement.Value : null
                };
                flags.Add(flag);
            }

            return flags.ToArray();
        }

        private static TagSet[] LoadTagSets(XElement element)
        {
            if (element == null) return new TagSet[0];

            List<TagSet> sets = new List<TagSet>();
            foreach (XElement setElement in element.Elements("set"))
            {
                TagSet set = new TagSet
                {
                    Id = setElement.Attribute("id").Value
                };
                foreach (XElement tagElement in setElement.Elements("t"))
                    set.AddTag(tagElement.Attribute("id").Value, tagElement.Value);
                sets.Add(set);
            }

            return sets.ToArray();
        }

        /// <summary>
        /// Loads facets and flags profile from the specified XML element.
        /// </summary>
        /// <param name="element">The profile XML root element.</param>
        /// <returns>profile</returns>
        /// <exception cref="ArgumentNullException">null XML element</exception>
        public virtual void Load(XElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            Facets = LoadFacets(element.Element("facets"));
            Flags = LoadFlags(element.Element("flags"));
            TagSets = LoadTagSets(element.Element("tag-sets"));
        }
    }
}

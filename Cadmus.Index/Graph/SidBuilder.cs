﻿using System;
using System.Text;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// The source ID (SID) builder used to build source IDs for graph nodes,
    /// starting from either items or parts pins.
    /// </summary>
    public static class SidBuilder
    {
        /// <summary>
        /// Builds the source ID.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="id">The GUID item/part identifier.</param>
        /// <param name="partRoleId">The optional part role identifier.</param>
        /// <param name="pinName">Optional name of the source pin.</param>
        /// <param name="pinValue">Optional value of the EID source pin.</param>
        /// <returns>ID or null.</returns>
        /// <exception cref="ArgumentNullException">id</exception>
        public static string Build(NodeSourceType sourceType,
            string id, string partRoleId = null,
            string pinName = null, string pinValue = null)
        {
            if (sourceType == NodeSourceType.User) return null;
            if (id == null) throw new ArgumentNullException(nameof(id));

            StringBuilder sb = new StringBuilder(id);

            switch (sourceType)
            {
                case NodeSourceType.ItemFacet:
                    sb.Append("/facet");
                    break;
                case NodeSourceType.ItemGroup:
                    sb.Append("/group");
                    break;
                case NodeSourceType.Pin:
                    // [:roleId]
                    if (!string.IsNullOrEmpty(partRoleId))
                        sb.Append(':').Append(partRoleId);
                    // /pin
                    sb.Append('/').Append(pinName);
                    // [/value]
                    if (!string.IsNullOrEmpty(pinValue))
                        sb.Append('/').Append(pinValue);
                    break;
                default: return null;
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// The type of source for a node.
    /// </summary>
    public enum NodeSourceType
    {
        /// <summary>User.</summary>
        User = 0,
        /// <summary>Item.</summary>
        Item,
        /// <summary>Item's facet.</summary>
        ItemFacet,
        /// <summary>Item's group.</summary>
        ItemGroup,
        /// <summary>Part's pin.</summary>
        Pin
    }
}
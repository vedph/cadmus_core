using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cadmus.Core
{
    /// <summary>
    /// Base class for parts implementations.
    /// </summary>
    /// <seealso cref="IPart" />
    public abstract class PartBase : IPart
    {
        /// <summary>
        /// The prefix prepended to the ID of each part's fragment ID.
        /// All the part's fragments IDs must start with this prefix.
        /// For instance, a comment part's fragment might have its ID equal
        /// to "fr.comment".
        /// </summary>
        public const string FR_PREFIX = "fr.";

        /// <summary>
        /// The role ID reserved for the base text part. There can be only 1
        /// part type with this role; that part represents the base text
        /// referenced by text layers.
        /// </summary>
        public const string BASE_TEXT_ROLE_ID = "base-text";

        /// <summary>
        /// Gets or sets the part identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the item the parts belongs to.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the part's type identifier. This is usually derived
        /// from the part's class <see cref="TagAttribute"/> attribute.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <remarks>
        /// Parts of the same type in the same item can have different
        /// roles. For instance, two date parts may refer to the date of the
        /// original text and to that of its later copy. In this case, a
        /// role ID helps selecting the desired part from an item.
        /// </remarks>
        public string? RoleId { get; set; }

        /// <summary>
        /// Gets or sets the thesaurus scope. This is an arbitrary string
        /// representing the suffix to be appended to a thesaurus ID (just
        /// before the <c>@</c> introducing its language ID) when loading
        /// thesauri. The suffix is appended with a leading dot, and should
        /// include only letters A-Z or a-z, digits, dash, and underscore.
        /// </summary>
        /// <remarks>This scope is used for those parts which require to
        /// load different thesauri according to runtime data. For instance,
        /// an apparatus part might require to load a different set of witnesses
        /// according to the work it refers to. Thus, an editor could append
        /// the part's thesaurus scope to its required thesaurus ID before
        /// loading it. For instance, say the part wants to load a thesaurus
        /// with ID <c>witnesses@en</c>: if the part has its
        /// <see cref="ThesaurusScope"/> equal to <c>lucr</c>, the editor will
        /// rather load <c>witnesses.lucr@en</c>.
        /// </remarks>
        public string? ThesaurusScope { get; set; }

        /// <summary>
        /// Creation date and time (UTC).
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// ID of the user who created the resource.
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// ID of the user who last saved the resource.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartBase"/> class.
        /// </summary>
        protected PartBase()
        {
            Id = Guid.NewGuid().ToString();
            ItemId = "";
            CreatorId = UserId = "";
            TagAttribute? attr = GetType().GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            TypeId = attr != null ? attr.Tag : GetType().FullName!;
            TimeCreated = TimeModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a data pin from the specified key=value pair.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>data pin</returns>
        /// <exception cref="ArgumentNullException">null name</exception>
        protected DataPin CreateDataPin(string name, string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return new DataPin
            {
                ItemId = ItemId,
                PartId = Id,
                RoleId = RoleId,
                Name = name,
                Value = value
            };
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>pins</returns>
        public abstract IEnumerable<DataPin> GetDataPins(IItem? item = null);

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public abstract IList<DataPinDefinition> GetDataPinDefinitions();

        /// <summary>
        /// Build the ID used to instantiate a part via a part provider,
        /// given the specified part's type and role IDs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The result is either equal to the part's type ID (e.g.
        /// "it.vedph.note"), or, for a layer part, equal to the part's
        /// type ID + ":" + the fragment's type ID (e.g.
        /// "it.vedph.token-text-layer:fr.it.vedph.comment").
        /// </para>
        /// <para>
        /// The convention underlying this method assumes that any fragment type ID
        /// starts with the "fr." prefix, and that a layer part has the fragment type
        /// ID as its role ID. For instance, a token-based text layer part for
        /// comments has type ID="it.vedph.token-text-layer", and role
        /// ID="fr.it.vedph.comment". So, each layer part has the
        /// corresponding fragment ID as its role. Should you want to use the
        /// same fragment type with different roles, add a new part type
        /// definition with role=fragment ID + colon + role ID,
        /// e.g. "fr.it.vedph.comment:scholarly".
        /// </para>
        /// </remarks>
        /// <param name="typeId">The type identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>Provider ID.</returns>
        /// <exception cref="ArgumentNullException">typeId</exception>
        public static string BuildProviderId(string typeId, string? roleId)
        {
            if (typeId == null) throw new ArgumentNullException(nameof(typeId));

            string result = typeId;

            if (roleId?.StartsWith(FR_PREFIX, StringComparison.Ordinal) == true)
            {
                // get the fragment type from the initial fr. up to the 1st colon
                // (excluded) if any, as all what follows the dot does not belong
                // to the fragment type ID (e.g. "fr.comment:scholarly")
                int i = roleId.IndexOf(':', FR_PREFIX.Length);
                string frType = i > -1 ? roleId[..i] : roleId;
                result += ":" + frType;
            }

            return result;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(Id).Append(' ').Append(TypeId);
            if (RoleId != null) sb.Append(" [").Append(RoleId).Append(']');
            return sb.ToString();
        }
    }
}

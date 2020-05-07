using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Bibliography author.
    /// </summary>
    public class BibAuthor
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the optional role identifier. This can represent the
        /// role of the author in the bibliographic record, e.g. "editor",
        /// "translator", "organization", etc.
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(LastName);
            if (!string.IsNullOrEmpty(FirstName))
                sb.Append(", ").Append(FirstName);
            if (!string.IsNullOrEmpty(RoleId))
                sb.Append(" (").Append(RoleId).Append(')');
            return sb.ToString();
        }
    }
}

using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Archive document's attachments part. This list any number of attachments
    /// to a document, represented by a list of <see cref="ArchiveAttachment"/>'s.
    /// Tag: <c>net.fusisoft.archive-attachments</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("net.fusisoft.archive-attachments")]
    public sealed class ArchiveAttachmentsPart : PartBase
    {
        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        public List<ArchiveAttachment> Attachments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveAttachmentsPart"/>
        /// class.
        /// </summary>
        public ArchiveAttachmentsPart()
        {
            Attachments = new List<ArchiveAttachment>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <remarks>
        /// <para>Search pins:</para>
        /// <list type="bullet">
        /// <item>
        /// <term>att-count</term>
        /// <description>count of attachments</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            if (Attachments?.Count == 0) return Array.Empty<DataPin>();

            return from a in Attachments
                   select CreateDataPin("att-count",
                       a.Count.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[ArchiveAttachments] {Attachments?.Count ?? 0}";
        }
    }

    #region ArchiveAttachment
    /// <summary>
    /// An attachment to an archive document.
    /// </summary>
    public sealed class ArchiveAttachment
    {
        /// <summary>
        /// Gets or sets the note about the attachment.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the sheets count for this attachment.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the optional date for this attachment.
        /// </summary>
        public ArchiveDate Date { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Attachment: c.{Count} - d.{Date}: {Note}";
        }
    }
    #endregion
}

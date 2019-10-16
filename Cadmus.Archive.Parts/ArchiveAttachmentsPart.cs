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
    /// </summary>
    /// <remarks>
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>attachment.count</term>
    /// <description>count of attachments</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="PartBase" />
    [Tag("archive-attachments")]
    public sealed class ArchiveAttachmentsPart : PartBase
    {
        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        public List<ArchiveAttachment> Attachments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveAttachmentsPart"/> class.
        /// </summary>
        public ArchiveAttachmentsPart()
        {
            Attachments = new List<ArchiveAttachment>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            if (Attachments?.Count == 0) return Array.Empty<DataPin>();

            return from a in Attachments
                   select CreateDataPin("attachment.count",
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
            return $"{nameof(ArchiveAttachmentsPart)}: {Attachments.Count}";
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
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Attachment: c.{Count} - d.{Date}: {Note}";
        }
    }
    #endregion
}

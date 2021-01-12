using System.Globalization;
using System.Collections.Generic;
using Cadmus.Core;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Archive document's attachments part. This list any number of attachments
    /// to a document, represented by a list of <see cref="ArchiveAttachment"/>'s.
    /// Tag: <c>it.vedph.archive-attachments</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("it.vedph.archive-attachments")]
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
        /// <returns>The pins: <c>tot-count</c>.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return new DataPin[]
            {
                CreateDataPin("tot-count",
                    Attachments?.Count.ToString(CultureInfo.InvariantCulture) ?? "0")
            };
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Integer,
                    "tot-count",
                    "The total count of attachments.")
            });
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

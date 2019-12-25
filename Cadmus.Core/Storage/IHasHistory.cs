namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Interface implemented by history records.
    /// </summary>
    public interface IHasHistory : IHasVersion
    {
        /// <summary>
        /// Gets or sets the identifier of the data record this history record
        /// refers to.
        /// </summary>
        string ReferenceId { get; }

        /// <summary>
        /// Gets or sets the record status.
        /// </summary>
        EditStatus Status { get; set; }
    }

    /// <summary>
    /// Edit status.
    /// </summary>
    public enum EditStatus
    {
        /// <summary>The item has been created.</summary>
        Created = 0,

        /// <summary>The item was existing and has been updated.</summary>
        Updated = 1,

        /// <summary>The item has been deleted. This is a status typically
        /// used in history.</summary>
        Deleted = 2
    }
}

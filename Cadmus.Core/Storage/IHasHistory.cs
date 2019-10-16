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
}

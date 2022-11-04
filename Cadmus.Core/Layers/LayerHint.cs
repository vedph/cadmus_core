using System.Text;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// A reconciliation hint about a text layer's fragment.
    /// </summary>
    public class LayerHint
    {
        /// <summary>
        /// Gets or sets the fragment's location.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the edit operation, which summarizes the editing
        /// operation which happened in the base text and is related to the
        /// fragment targeted by this hint.
        /// </summary>
        public string? EditOperation { get; set; }

        /// <summary>
        /// Gets or sets the optional patch operation, which defines an
        /// automatic patch operation which could be applied to reconcile
        /// the target fragment to the new base text.
        /// </summary>
        public string? PatchOperation { get; set; }

        /// <summary>
        /// Gets or sets the impact level. This is a numeric value representing
        /// the level of impact of editing operations on the targeted fragment.
        /// Usually values are 0=unaffected, 1=potentially affected, 2=affected
        /// with possible automatic patch.
        /// </summary>
        public int ImpactLevel { get; set; }

        /// <summary>
        /// Gets or sets the optional description connected to this hint.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(Location).Append('=').Append(ImpactLevel)
              .Append(": ").Append(EditOperation);

            if (!string.IsNullOrEmpty(Description))
                sb.Append(" (").Append(Description).Append(')');

            if (!string.IsNullOrEmpty(PatchOperation))
                sb.Append(" [").Append(PatchOperation).Append(']');

            return sb.ToString();
        }
    }
}

using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A physical 2D or 3D size, comprising 2 or 3
    /// <see cref="PhysicalDimension"/>'s.
    /// </summary>
    public class PhysicalSize
    {
        /// <summary>
        /// Gets or sets an optional tag used to categorize or group several
        /// sizes.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public PhysicalDimension W { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public PhysicalDimension H { get; set; }

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        public PhysicalDimension D { get; set; }

        /// <summary>
        /// Gets or sets an optional note.
        /// </summary>
        public string Note {get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(Tag))
                sb.Append('[').Append(Tag).Append("] ");

            if (W?.Value != 0) sb.Append(W.ToString());

            if (H?.Value != 0)
            {
                if (sb.Length > 0) sb.Append(" × ");
                sb.Append(H.ToString());
            }
            if (D?.Value != 0)
            {
                if (sb.Length > 0) sb.Append(" × ");
                sb.Append(D.ToString());
            }

            if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}

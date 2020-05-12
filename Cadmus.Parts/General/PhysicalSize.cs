using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A physical 2D or 3D size.
    /// </summary>
    public class PhysicalSize
    {
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
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
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
            return sb.ToString();
        }
    }
}

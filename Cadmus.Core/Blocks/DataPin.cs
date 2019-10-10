using System;
using System.Text.RegularExpressions;
namespace Cadmus.Core.Blocks
{
    /// <summary>
    /// Part data pin.
    /// </summary>
    public class DataPin
    {
        private static readonly Regex _keyRegex =
            new Regex(@"^[a-zA-Z0-9\-_\.]+$");

        private string _name;

        #region Properties
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the part identifier.
        /// </summary>
        public string PartId { get; set; }

        /// <summary>
        /// Gets or sets the optional role identifier.
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// Gets or sets the pin name.
        /// </summary>
        /// <value>The name.</value>
        /// <exception cref="ArgumentNullException">null name</exception>
        /// <exception cref="ArgumentException">invalid name</exception>
        public string Name
        {
            get => _name;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (!_keyRegex.IsMatch(value))
                {
                    throw new ArgumentException(LocalizedStrings.Format(
                        Properties.Resources.InvalidDataPinName, value));
                }

                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
        #endregion

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{_name}=[{Value}]";
        }
    }
}
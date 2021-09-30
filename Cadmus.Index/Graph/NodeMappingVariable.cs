using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A variable extracted from a <see cref="NodeMapping"/>. This can derive
    /// from either a placeholder in a template, or from a macro instruction
    /// as the full value of some parameter. In both cases, the variable has a
    /// unique ID, a name extracted from the ID (=ID minus argument if any),
    /// and an optional, single numeric argument extracted from the ID.
    /// </summary>
    public class NodeMappingVariable
    {
        /// <summary>
        /// Gets or sets the variable identifier. For macros, this is the variable
        /// name minus its <c>$</c> prefix; for placeholders, it's the placeholder
        /// name minus its wrapping braces. The optional arguments are preserved
        /// as part of the ID, while being extracted in <see cref="Arguments"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name. This is equal to <see cref="Id"/> minus
        /// the optional <see cref="Arguments"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional arguments used in some of the macros.
        /// Each argument is introduced by a colon <c>:</c>.
        /// </summary>
        public IList<string> Arguments { get; private set; }

        /// <summary>
        /// Gets or sets the variable's value, when set.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets a value indicating whether this variable has arguments.
        /// </summary>
        /// <value>
        /// <c>true</c> if this variable has arguments; otherwise, <c>false</c>.
        /// </value>
        public bool HasArguments => Arguments?.Count > 0;

        /// <summary>
        /// Adds the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <exception cref="ArgumentNullException">arg</exception>
        public void AddArgument(string arg)
        {
            if (arg == null) throw new ArgumentNullException(nameof(arg));

            if (Arguments == null) Arguments = new List<string>();
            Arguments.Add(arg);
        }

        /// <summary>
        /// Gets the argument.
        /// </summary>
        /// <param name="index">The argument index.</param>
        /// <returns>The argument value, or null if not found.</returns>
        public string GetArgument(int index)
        {
            if (Arguments == null || index < 0 || index >= Arguments.Count)
                return null;
            return Arguments[index];
        }

        /// <summary>
        /// Gets the argument as an integer.
        /// </summary>
        /// <param name="index">The argument index.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The argument or <paramref name="defaultValue"/> if not
        /// found or not valid.</returns>
        public int GetArgument(int index, int defaultValue)
        {
            string arg = GetArgument(index);
            if (arg == null) return defaultValue;
            return int.TryParse(arg, out int n) ? n : defaultValue;
        }

        /// <summary>
        /// Removes all the arguments from this variable.
        /// </summary>
        public void RemoveArguments() => Arguments?.Clear();

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Id);
            if (Value != null) sb.Append('=').Append(Value);
            return sb.ToString();
        }
    }
}

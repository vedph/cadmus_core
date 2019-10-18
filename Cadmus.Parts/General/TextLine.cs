using System;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A single, generic line of text. More specialized text lines can be derived
    /// from this class.
    /// </summary>
    public class TextLine
    {
        private static readonly char[] TOKENS_SEPARATORS =
            { ' ', '\t', '\r', '\n' };

        private int _y;
        private string _text;

        /// <summary>
        /// Line Y value.
        /// </summary>
        /// <remarks>This is not the line number but just a line ID, whose
        /// function is not only defining its identity against all other lines
        /// in the text, but also to define its order in the sequence of lines.
        /// The number starts from 1.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value less than 1.</exception>
        public int Y
        {
            get { return _y; }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));
                _y = value;
            }
        }

        /// <summary>
        /// Text of this line.
        /// </summary>
        /// <exception cref="ArgumentNullException">value</exception>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        #region Tokens
        /// <summary>
        /// Get the number of the token whose character is at the specified column
        /// number in this line.
        /// </summary>
        /// <param name="columnNumber">column number (1-N)</param>
        /// <returns>token number (1-N) or 0 if no token at this column</returns>
        public int TokenFromColumn(int columnNumber)
        {
            if (string.IsNullOrEmpty(_text)) return 0;

            int token = 1;
            columnNumber--;
            for (int i = 0; i < _text.Length && i <= columnNumber; i++)
            {
                if (_text[i] != ' ') continue;
                if (columnNumber == i) return 0;
                token++;
            }

            return token;
        }

        /// <summary>
        /// Get the column number corresponding to the beginning of the specified
        /// token number.
        /// </summary>
        /// <param name="tokenNumber">token number (1-N)</param>
        /// <returns>column number (1-N or 0 if token not found)</returns>
        public int ColumnFromToken(int tokenNumber)
        {
            if (string.IsNullOrEmpty(_text)) return 0;
            if (tokenNumber == 1) return 1;

            int currentToken = 1;
            for (int i = 0; i < _text.Length; i++)
            {
                if (Array.IndexOf(TOKENS_SEPARATORS, _text[i]) > -1)
                {
                    if (++currentToken == tokenNumber) return i + 2;
                }
            }

            return 0;
        }

        /// <summary>
        /// Get the bounds for the specified token.
        /// </summary>
        /// <param name="tokenNumber">token number (1-N)</param>
        /// <returns>left bound (=index of 1st token char) and right bound (=index past the 
        /// last token char) in a tuple, or null if no text</returns>
        public Tuple<int,int> GetTokenBounds(int tokenNumber)
        {
            if (string.IsNullOrEmpty(_text)) return null;
            int left = 0, right, n = 1;

            for (int i = 0; n < tokenNumber && i < _text.Length; i++)
            {
                if (_text[i] == ' ')
                {
                    left = i + 1;
                    n++;
                }
            }

            if (n == tokenNumber)
            {
                for (right = left;
                    right < _text.Length && _text[right] != ' '; right++) ;
            }
            else
            {
                left = 0;
                right = 0;
            }

            return Tuple.Create(left, right);
        }

        /// <summary>
        /// Get the tokens in this line.
        /// </summary>
        /// <returns>Array of tokens.</returns>
        public string[] GetTokens()
        {
            return _text != null ?
                _text.Split(TOKENS_SEPARATORS) : Array.Empty<string>();
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{_y:00000}] {_text}";
        }
    }
}

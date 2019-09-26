using System;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A single, generic line of text. More specialized text lines can be derived
    /// from this class.
    /// </summary>
    public class TextLine
    {
        private static readonly char[] TOKENS_SEPARATORS = { ' ', '\t', '\r', '\n' };

        /// <summary>
        /// Line Y value.
        /// </summary>
        /// <remarks>This is not the line number but just a line ID, whose
        /// function is not only defining its identity against all other lines
        /// in the text, but also to define its order in the sequence of lines.
        /// The number starts from 1.</remarks>
        public int Y { get; set; }

        /// <summary>
        /// Text of this line.
        /// </summary>
        public string Text { get; set; }

        #region Tokens
        /// <summary>
        /// Get the number of the token whose character is at the specified column
        /// number in this line.
        /// </summary>
        /// <param name="colNumber">column number (1-N)</param>
        /// <returns>token number (1-N) or 0 if no token at this column</returns>
        public int TokenFromColumn(int colNumber)
        {
            if (String.IsNullOrEmpty(Text)) return 0;

            int tok = 1;
            colNumber--;
            for (int i = 0; i < Text.Length && i <= colNumber; i++)
            {
                if (Text[i] != ' ') continue;
                if (colNumber == i) return 0;
                tok++;
            }

            return tok;
        }

        /// <summary>
        /// Get the column number corresponding to the beginning of the specified
        /// token number.
        /// </summary>
        /// <param name="tokenNumber">token number (1-N)</param>
        /// <returns>column number (1-N or 0 if token not found)</returns>
        public int ColumnFromToken(int tokenNumber)
        {
            if (String.IsNullOrEmpty(Text)) return 0;
            if (tokenNumber == 1) return 1;

            int tok = 1;
            for (int i = 0; i < Text.Length; i++)
            {
                if (Array.IndexOf(TOKENS_SEPARATORS, Text[i]) > -1)
                {
                    if (++tok == tokenNumber) return i + 2;
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
            if (String.IsNullOrEmpty(Text)) return null;
            int left = 0, right, n = 1;

            for (int i = 0; n < tokenNumber && i < Text.Length; i++)
            {
                if (Text[i] == ' ')
                {
                    left = i + 1;
                    n++;
                }
            }

            if (n == tokenNumber)
            {
                for (right = left;
                    right < Text.Length && Text[right] != ' '; right++) ;
            }
            else
            {
                left = 0;
                right = 0;
            }

            return Tuple.Create(left, right);
        }

        /// <summary>
        /// Count tokens in this line.
        /// </summary>
        /// <returns>tokens count</returns>
        public int GetTokensCount()
        {
            return Text?.Split(TOKENS_SEPARATORS).Length ?? 0;
        }

        /// <summary>
        /// Get tokens in this line.
        /// </summary>
        /// <returns>array of tokens</returns>
        public string[] GetTokens()
        {
            return Text != null ? Text.Split(TOKENS_SEPARATORS) : new string[0];
        }

        /// <summary>
        /// Count the tokens in the specified text.
        /// </summary>
        /// <param name="text">text to analyze</param>
        /// <returns>tokens count</returns>
        public static int CountTokens(string text)
        {
            return String.IsNullOrEmpty(text) ? 0 : text.Split(TOKENS_SEPARATORS).Length;
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Y:00000}: {Text}";
        }
    }
}

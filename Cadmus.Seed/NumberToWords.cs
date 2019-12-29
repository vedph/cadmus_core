using System;

namespace Cadmus.Seed
{
    // https://stackoverflow.com/questions/2729752/converting-numbers-in-to-words-c-sharp

    /// <summary>
    /// Number to English words utility.
    /// </summary>
    public static class NumberToWords
    {
        /// <summary>
        /// Converts the specified number into the corresponding English word.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The word</returns>
        public static string Convert(int number)
        {
            if (number == 0) return "zero";
            if (number < 0)
                return "minus " + Convert(Math.Abs(number));

            string words = "";

            if (number / 1000000 > 0)
            {
                words += Convert(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += Convert(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += Convert(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words.Length > 0) words += "and ";

                string[] unitsMap = new[]
                {
                    "zero", "one", "two", "three", "four", "five", "six",
                    "seven", "eight", "nine", "ten", "eleven", "twelve",
                    "thirteen", "fourteen", "fifteen", "sixteen", "seventeen",
                    "eighteen", "nineteen" };
                string[] tensMap = new[]
                {
                    "zero", "ten", "twenty", "thirty", "forty", "fifty",
                    "sixty", "seventy", "eighty", "ninety"
                };

                if (number < 20)
                {
                    words += unitsMap[number];
                }
                else
                {
                    words += tensMap[number / 10];
                    if (number % 10 > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }
}

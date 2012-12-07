/******************************************************************************
 * Copyright (c) 2012 ABB Group
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Patrick Francis (ABB Group) - C# implementation and documentation
 *    Emily Hill (Univ. of Delaware) - Original design and implementation
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ABB.Swum
{
    /// <summary>
    /// Splits an identifier on non-alphabetic characters and easy camelcase transitions (lowercase to uppercase).
    /// </summary>
    public class ConservativeIdSplitter : IdSplitter
    {
        private readonly char[] NonAlphabeticCharacters = { '_', '@', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        /// <summary>
        /// Splits an identifier on non-alphabetic characters and easy camelcase transitions (lowercase to uppercase).
        /// </summary>
        /// <param name="identifier">The identifier to split</param>
        /// <returns>An array of the words resulting from splitting the identifier.</returns>
        public override string[] Split(string identifier)
        {
            //TODO: this splits on digits individually, and throws them away as meaningless word separators
            //      Should this be changed to group successive digits together and treat them as potentially-meaningful word?
            //      For example, in Special123Certificate, 123 might be meaningful to the developer and a useful search term

            List<string> splitWords = new List<string>();
            foreach(string word in Regex.Split(identifier, @"[^\d\p{L}]+"))
            {
                if (!string.IsNullOrEmpty(word))
                {
                    splitWords.AddRange(SplitOnLowercaseToUppercase(word));
                }
            }
            //foreach (string word in identifier.Split(NonAlphabeticCharacters, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    splitWords.AddRange(SplitOnLowercaseToUppercase(word));
            //}

            return splitWords.ToArray();
        }

        /// <summary>
        /// Splits a word where a lowercase letter is followed by an uppercase letter. The word is split at all locations where this occurs.
        /// </summary>
        /// <param name="word">The word to be split.</param>
        /// <returns>An array of the subwords resulting from the splits.</returns>
        private string[] SplitOnLowercaseToUppercase(string word)
        {
            List<string> splitWords = new List<string>();
            int currentStartIndex = 0; //the index of the beginning of the current subword
            for (int i = 0; i < word.Length - 1; i++)
            {
                if ((char.IsLower(word, i) && char.IsUpper(word, i + 1))
                    || (char.IsDigit(word, i) && !char.IsDigit(word, i + 1))
                    || (!char.IsDigit(word, i) && char.IsDigit(word, i + 1)))
                {
                    //found split point, add left word to list
                    splitWords.Add(word.Substring(currentStartIndex, i - currentStartIndex + 1));
                    currentStartIndex = i + 1;
                }
            }
            //add substring remaining after the last split point
            splitWords.Add(word.Substring(currentStartIndex));

            return splitWords.ToArray();
        }
    }
}

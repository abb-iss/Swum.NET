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

namespace ABB.Swum {
    /// <summary>
    /// Splits an identifier on non-alphabetic characters and easy camelcase transitions (lowercase to uppercase).
    /// </summary>
    public class ConservativeIdSplitter : IdSplitter {
        private Regex nonWord = new Regex(@"[^\d\p{L}]", RegexOptions.Compiled);

        private Regex numLet = new Regex(@"(\p{L})(\d)", RegexOptions.Compiled);
        private Regex numLet2 = new Regex(@"(\d)(\p{L})", RegexOptions.Compiled);

        private Regex lowerUpper = new Regex(@"(\p{Ll})(\p{Lu})", RegexOptions.Compiled);

        private Regex upperLower = new Regex(@"(\p{Lu})(\p{Lu}\p{Ll})", RegexOptions.Compiled);
 

        /// <summary>
        /// Splits an identifier on non-alphabetic characters and easy camelcase transitions (lowercase to uppercase).
        /// </summary>
        /// <param name="identifier">The identifier to split</param>
        /// <returns>An array of the words resulting from splitting the identifier.</returns>
        public override string[] Split(string identifier) {



            //remove any non-word or non-digit characters
            var id = nonWord.Replace(identifier, " ");                

            //split numbers from letters
            id = numLet.Replace(id, "$1 $2");
            id = numLet2.Replace(id, "$1 $2");

            //split lowercase to uppercase
            id = lowerUpper.Replace(id, "$1 $2");

            //split uppercase to lowercase
            //final uppercase letter is put with lowercase ones
            id = upperLower.Replace(id, "$1 $2");


            return id.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Splits a word where a lowercase letter is followed by an uppercase letter. The word is split at all locations where this occurs.
        /// </summary>
        /// <param name="word">The word to be split.</param>
        /// <returns>An array of the subwords resulting from the splits.</returns>
        private string[] SplitOnLowercaseToUppercase(string word) {
            List<string> splitWords = new List<string>();
            int currentStartIndex = 0; //the index of the beginning of the current subword
            for(int i = 0; i < word.Length - 1; i++) {
                if((char.IsLower(word, i) && char.IsUpper(word, i + 1))
                   || (char.IsDigit(word, i) && !char.IsDigit(word, i + 1))
                   || (!char.IsDigit(word, i) && char.IsDigit(word, i + 1))) {
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

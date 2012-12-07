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
using ABB.Swum.Utilities;

namespace ABB.Swum
{
    /// <summary>
    /// Determines the stem for a given word by reading from a file containing precomputed word/stem pairs
    /// </summary>
    public class FileStemmer : Stemmer
    {
        /// <summary>
        /// A Dictionary mapping words to their stems. This is populated from the file supplied to the constructor.
        /// </summary>
        protected Dictionary<string, string> Stems;

        /// <summary>
        /// Creates a new FileStemmer using the file specified.
        /// </summary>
        /// <param name="stemFilePath">The path to the file containing the word/stem pairs to use.</param>
        public FileStemmer(string stemFilePath)
        {
            Stems = LibFileLoader.ReadStemFile(stemFilePath);
        }

        /// <summary>
        /// Returns the stem for the given word.
        /// </summary>
        /// <param name="word">The word to stem.</param>
        /// <returns>The stem of the given word.</returns>
        public override string Stem(string word)
        {
            if (Stems.ContainsKey(word.ToLower()))
                return Stems[word.ToLower()];
            else
                return word;
        }
    }
}

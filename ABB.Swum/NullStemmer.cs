/******************************************************************************
 * Copyright (c) 2012 ABB Group
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Patrick Francis (ABB Group) - initial implementation and documentation
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABB.Swum
{
    /// <summary>
    /// This is a "null" stemmer. It does not actually do any stemming, but just returns the original word unaltered.
    /// </summary>
    public class NullStemmer : Stemmer
    {
        /// <summary>
        /// Returns the supplied word unchanged.
        /// </summary>
        /// <param name="word">The word to stem.</param>
        /// <returns>The "stem" of the word. This is identical to the word itself.</returns>
        public override string Stem(string word)
        {
            return word;
        }
    }
}

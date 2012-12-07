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

namespace ABB.Swum
{
    /// <summary>
    /// Determines the stem of a given word.
    /// </summary>
    public abstract class Stemmer
    {
        /// <summary>
        /// Returns the stem of the given word.
        /// </summary>
        /// <param name="word">The word to stem.</param>
        /// <returns>The stem of the given word.</returns>
        public abstract string Stem(string word);
    }
}

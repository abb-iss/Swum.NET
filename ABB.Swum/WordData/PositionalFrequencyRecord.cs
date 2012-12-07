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

namespace ABB.Swum.WordData
{
    /// <summary>
    /// The positional frequency data for a given word.
    /// </summary>
    public struct PositionalFrequencyRecord
    {
        /// <summary>
        /// A count of how often the word appears at the beginning of an identifier.
        /// </summary>
        public int First;
        /// <summary>
        /// A count of how often the word appears in the middle of an identifier, i.e. not first or last.
        /// </summary>
        public int Middle;
        /// <summary>
        /// A count of how often the word appears at the end of an identifier.
        /// </summary>
        public int Last;
        /// <summary>
        /// A count of how often the word appears by itself in an identifier.
        /// </summary>
        public int Only;
        /// <summary>
        /// The total number of times the word appears in identifiers. This must be a sum of First, Middle, Last, and Only.
        /// </summary>
        public int Total;
    }
}

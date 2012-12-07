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
    /// Splits an identifier into its constituent words.
    /// </summary>
    public abstract class IdSplitter
    {
        /// <summary>
        /// Splits an identifier into its constituent words.
        /// </summary>
        /// <param name="identifier">The identifier to split.</param>
        /// <returns>An array of the consituent words of the identifer.</returns>
        public abstract string[] Split(string identifier);
    }
}

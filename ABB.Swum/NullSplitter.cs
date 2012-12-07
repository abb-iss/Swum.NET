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
    /// This is a null implementation of an IdSplitter. It does not split, but simply returns the original word intact.
    /// </summary>
    public class NullSplitter : IdSplitter
    {
        /// <summary>
        /// Does not split the identifier, it simply returns it.
        /// </summary>
        public override string[] Split(string identifier)
        {
            return new string[] { identifier };
        }
    }
}

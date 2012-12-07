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
    /// Represents a location within a program. This is used in conjunction with Node objects.
    /// </summary>
    public enum Location
    {
        /// <summary>
        /// No location.
        /// </summary>
        None = 0,
        /// <summary>
        /// A program identifier.
        /// </summary>
        Name,
        /// <summary>
        /// A formal parameter to a method/function.
        /// </summary>
        Formal,
        /// <summary>
        /// A member of a class.
        /// </summary>
        OnClass,
        /// <summary>
        /// A return type.
        /// </summary>
        Return
    }
}

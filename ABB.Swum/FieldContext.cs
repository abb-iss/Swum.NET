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
    /// Encapsulates the program context relevent to a field.
    /// </summary>
    public class FieldContext : IdContext
    {
        /// <summary>
        /// Creates a new FieldContext.
        /// </summary>
        /// <param name="idType">The type of the field.</param>
        /// <param name="idTypeIsPrimitive">Whether the type of the field is a primitive data type.</param>
        /// <param name="declaringClass">The class that the field is part of.</param>
        public FieldContext(string idType, bool idTypeIsPrimitive, string declaringClass) : base(idType, idTypeIsPrimitive, declaringClass) { }

        /// <summary>
        /// Creates a new empty FieldContext.
        /// </summary>
        public FieldContext() : this(string.Empty, false, string.Empty) { }
    }
}

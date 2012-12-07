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
    /// Encapsulates additional program context relevent to an identifier.
    /// </summary>
    public abstract class IdContext
    {
        /// <summary>
        /// The type of the identifier (integer, boolean, etc.)
        /// </summary>
        public string IdType { get; set; }
        /// <summary>
        /// Indicates whether the ID Type is a primitive type.
        /// </summary>
        public bool IdTypeIsPrimitive { get; set; }
        /// <summary>
        /// The class that the identifier is part of, if any.
        /// </summary>
        public string DeclaringClass { get; set; }

        /// <summary>
        /// Creates a new IdContext.
        /// </summary>
        /// <param name="idType">The type of the identifier.</param>
        /// <param name="idTypeIsPrimitive">Whether the type of the identifier is a primitive data type.</param>
        /// <param name="declaringClass">The class that the identifier is part of.</param>
        protected IdContext(string idType, bool idTypeIsPrimitive, string declaringClass)
        {
            this.IdType = idType;
            this.IdTypeIsPrimitive = idTypeIsPrimitive;
            this.DeclaringClass = declaringClass;
        }

        /// <summary>
        /// Creates a new IdContext.
        /// </summary>
        protected IdContext() : this(string.Empty, false, string.Empty) { }
    }
}

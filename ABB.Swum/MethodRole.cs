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
    /// Indicates the semantic role of a method within a program.
    /// </summary>
    public enum MethodRole
    {
        /// <summary>
        /// Unknown method role.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// A general method that doesn't serve a more specific role.
        /// </summary>
        Function,
        /// <summary>
        /// A method that gets an object property.
        /// </summary>
        Getter,
        /// <summary>
        /// A method that check whether an object meets some criteria.
        /// </summary>
        Checker,
        /// <summary>
        /// A method that performs some action.
        /// </summary>
        Action, 
        /// <summary>
        /// A method that sets an object property.
        /// </summary>
        Setter, 
        /// <summary>
        /// A callback method.
        /// </summary>
        Callback, 
        /// <summary>
        /// A method that responds to some event.
        /// </summary>
        EventHandler, 
        /// <summary>
        /// A visitor method.
        /// </summary>
        Visitor
    }
    
}

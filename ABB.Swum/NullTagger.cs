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
using ABB.Swum.Nodes;

namespace ABB.Swum
{
    /// <summary>
    /// This is a null implementation of the abstract Tagger class. All of the methods do nothing; they simply return.
    /// </summary>
    public class NullTagger : Tagger
    {
        /// <summary>
        /// Does nothing; simply returns
        /// </summary>
        /// <param name="parsedName"></param>
        public override void TagVariableName(PhraseNode parsedName)
        {
            return;
        }

        /// <summary>
        /// Does nothing; simply returns
        /// </summary>
        /// <param name="parsedName"></param>
        public override void TagType(PhraseNode parsedName)
        {
            return;
        }
        
        /// <summary>
        /// Does nothing; simply returns
        /// </summary>
        /// <param name="nounPhrase"></param>
        public override void TagNounPhrase(PhraseNode nounPhrase)
        {
            return;
        }

        /// <summary>
        /// Does nothing; simply returns
        /// </summary>
        /// <param name="nounPhrase"></param>
        /// <param name="startIndex"></param>
        /// <param name="stopIndex"></param>
        public override void TagNounPhrase(PhraseNode nounPhrase, int startIndex, int stopIndex)
        {
            return;
        }

        /// <summary>
        /// Does nothing; simply returns;
        /// </summary>
        /// <param name="node"></param>
        public override void PreTag(ProgramElementNode node)
        {
            return;
        }
    }
}

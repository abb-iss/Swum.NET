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
using ABB.Swum.Nodes;

namespace ABB.Swum
{
    /// <summary>
    /// Assigns part-of-speech tags to the words in an identifier.
    /// </summary>
    public abstract class Tagger
    {
        /// <summary>
        /// Assigns part-of-speech tags to the word nodes in the given variable name.
        /// </summary>
        /// <param name="parsedName">The parsed variable name to tag.</param>
        public abstract void TagVariableName(PhraseNode parsedName);
        /// <summary>
        /// Assigns part-of-speech tags to the word nodes in the given type name.
        /// </summary>
        /// <param name="parsedName">The parsed type name to tag.</param>
        public abstract void TagType(PhraseNode parsedName);
        /// <summary>
        /// Assigns part-of-speech tags to the word nodes in the given phrase, assuming it is a noun phrase.
        /// </summary>
        /// <param name="nounPhrase">The noun phrase to tag.</param>
        public abstract void TagNounPhrase(PhraseNode nounPhrase);
        /// <summary>
        /// Assigns part-of-speech tags to the word nodes in the given phrase, assuming it is a noun phrase.
        /// Only the words between startIndex and stopIndex, inclusive, are tagged.
        /// </summary>
        /// <param name="nounPhrase">The noun phrase to tag.</param>
        /// <param name="startIndex">The index of the first word to tag.</param>
        /// <param name="stopIndex">The index of the last word to tag.</param>
        public abstract void TagNounPhrase(PhraseNode nounPhrase, int startIndex, int stopIndex);
        /// <summary>
        /// Performs various actions that should occur prior to further tagging. 
        /// This method tags any digits in the name, identifies and removes any preamble, and tags any prepositions remaining in the name.
        /// </summary>
        /// <param name="node">The node to be tagged.</param>
        public abstract void PreTag(ProgramElementNode node);
    }
}

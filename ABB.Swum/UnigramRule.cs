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
using ABB.Swum.WordData;
using ABB.Swum.Utilities;

namespace ABB.Swum
{
    /// <summary>
    /// An abstract rule using unigram part-of-speech data and tagging.
    /// </summary>
    public abstract class UnigramRule : SwumRule
    {
        /// <summary>
        /// The part-of-speech data to use.
        /// </summary>
        protected PartOfSpeechData PosData;
        /// <summary>
        /// The part-of-speech tagger to use.
        /// </summary>
        protected Tagger PosTagger;
        /// <summary>
        /// The identifier splitter to use.
        /// </summary>
        protected IdSplitter Splitter;

        /// <summary>
        /// Creates a new UnigramRule using default values.
        /// </summary>
        public UnigramRule()
        {
            this.PosData = new PCKimmoPartOfSpeechData();
            this.PosTagger = new UnigramTagger(this.PosData);
            this.Splitter = new ConservativeIdSplitter();
        }

        /// <summary>
        /// Creates a new UnigramRule.
        /// </summary>
        /// <param name="posData">The part-of-speech data to use.</param>
        /// <param name="tagger">The part-of-speech tagger to use.</param>
        /// <param name="splitter">The identifier splitter to use.</param>
        public UnigramRule(PartOfSpeechData posData, Tagger tagger, IdSplitter splitter)
        {
            this.PosData = posData;
            this.PosTagger = tagger;
            this.Splitter = splitter;
        }
    }
}

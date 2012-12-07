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

namespace ABB.Swum
{
    /// <summary>
    /// Constructs Software Word Use Models using unigram-based rules.
    /// </summary>
    public class UnigramSwumBuilder : SwumBuilder
    {
        /// <summary>
        /// The part-of-speech data to use.
        /// </summary>
        public PartOfSpeechData PosData { get; set; }
        /// <summary>
        /// The part-of-speech tagger to use.
        /// </summary>
        public Tagger Tagger { get; set; }
        /// <summary>
        /// The identifier splitter to use.
        /// </summary>
        public IdSplitter Splitter { get; set; }

        /// <summary>
        /// Creates a new UnigramSwumBuilder with the predefined set of rules.
        /// </summary>
        public UnigramSwumBuilder()
            : base()
        {
            this.PosData = new PCKimmoPartOfSpeechData();
            this.Tagger = new UnigramTagger();
            this.Splitter = new ConservativeIdSplitter();
        }

        /// <summary>
        /// Initializes the builder's rule list to a defined set of Rule objects.
        /// </summary>
        protected override void DefineRuleSet()
        {
            if (this.PosData == null) { throw new InvalidOperationException("PosData must not be null when calling this method."); }
            if (this.Tagger == null) { throw new InvalidOperationException("Tagger must not be null when calling this method."); }
            if (this.Splitter == null) { throw new InvalidOperationException("Splitter must not be null when calling this method."); }

            var ruleList = new List<SwumRule>();
            ruleList.Add(new FieldRule(this.PosData, this.Tagger, this.Splitter));
            ruleList.Add(new ConstructorRule(this.PosData, this.Tagger, this.Splitter)); //must be first because rest process preamble?
            ruleList.Add(new DestructorRule(this.PosData, this.Tagger, this.Splitter));
            ruleList.Add(new EmptyNameRule(this.PosData, this.Tagger, this.Splitter));
            ruleList.Add(new CheckerRule(this.PosData, this.Tagger, this.Splitter)); //for now follows base verb rule
            ruleList.Add(new SpecialCaseRule(this.PosData, this.Tagger, this.Splitter)); //for now follows base verb rule
            ruleList.Add(new ReactiveRule(this.PosData, this.Tagger, this.Splitter));
            ruleList.Add(new EventHandlerRule(this.PosData, this.Tagger, this.Splitter));
            ruleList.Add(new NounPhraseRule(this.PosData, this.Tagger, this.Splitter));
            ruleList.Add(new LeadingPrepositionRule(this.PosData, this.Tagger, this.Splitter));
            ruleList.Add(new NonBaseVerbRule(this.PosData, this.Tagger, this.Splitter)); //for now follows base verb rule
            ruleList.Add(new DefaultBaseVerbRule(this.PosData, this.Tagger, this.Splitter));

            this.Rules = ruleList;
        }
    }
}

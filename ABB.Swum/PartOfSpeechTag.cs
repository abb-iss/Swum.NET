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
    /// The part-of-speech of a given word.
    /// </summary>
    public enum PartOfSpeechTag {
        /// <summary>
        /// An unknown part-of-speech.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// A noun.
        /// </summary>
        Noun, 
        /// <summary>
        /// A noun that's ignorable, i.e. an ignorable head word.
        /// </summary>
        NounIgnorable, // for ignorable head words
        /// <summary>
        /// A plural noun.
        /// </summary>
        NounPlural, // for plural nouns TODO
        /// <summary>
        /// A word modifying a noun, i.e. an adjective or another noun.
        /// </summary>
        NounModifier, // noun or adj
        /// <summary>
        /// An adjective, adverb, or noun, occurring before the verb in a name.
        /// </summary>
        NonVerb, // adj, adv, noun occurring BEFORE verb in name
        /// <summary>
        /// An ignorable verb.
        /// </summary>
        VerbIgnorable,
        /// <summary>
        /// A verb.
        /// </summary>
        Verb,
        /// <summary>
        /// A verb in past participle tense, e.g. -ed, -en
        /// </summary>
        PastParticiple, // -ed -en
        /// <summary>
        /// A verb in third-person singular tense.
        /// </summary>
        Verb3PS, // -s
        /// <summary>
        /// A verb ending in -ing, i.e. present participle.
        /// </summary>
        VerbIng, // -ing
        /*VerbBase { // infinitive
            public String toString() { return "VB"; }
        },*/
        /// <summary>
        /// A word modifying a verb, i.e. an adverb.
        /// </summary>
        VerbModifier, // adv
        /// <summary>
        /// A verb particle. For example, in "tear down", "tear" is a verb and "down" is a verb particle.
        /// </summary>
        VerbParticle, // tear down, 
        /// <summary>
        /// A preposition.
        /// </summary>
        Preposition,
        /// <summary>
        /// A "word" that is actually a number.
        /// </summary>
        Digit,
        /// <summary>
        /// A determiner, e.g. the, this, that.
        /// </summary>
        Determiner,
        /// <summary>
        /// A pronoun.
        /// </summary>
        Pronoun,
        /// <summary>
        /// A word that is prefixed on an identifier, but doesn't have any relevent part-of-speech.
        /// </summary>
        Preamble
    }
}

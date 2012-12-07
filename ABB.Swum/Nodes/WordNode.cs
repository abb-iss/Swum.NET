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
using System.Text.RegularExpressions;

namespace ABB.Swum.Nodes {
    /// <summary>
    /// A node representing a single word.
    /// </summary>
    public class WordNode : Node {
        /// <summary>
        /// The actual text of the word.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The word's part-of-speech.
        /// </summary>
        public PartOfSpeechTag Tag { get; set; }
        /// <summary>
        /// A rating of the confidence of the part-of-speech tagging for this word.
        /// </summary>
        public double Confidence { get; set; }
        private bool AllCaps = false;

        /// <summary>
        /// Creates a new WordNode.
        /// </summary>
        /// <param name="text">The actual text of the word.</param>
        /// <param name="tag">The part-of-speech of the word.</param>
        /// <param name="confidence">A rating of the confidence of the part-of-speech tagging for this word.</param>
        public WordNode(String text, PartOfSpeechTag tag, double confidence) {
            if(text == null) { throw new ArgumentNullException("text"); }
            this.Text = text;
            this.Tag = tag;
            this.Confidence = confidence;
            if(text == text.ToUpper()) { this.AllCaps = true; }
        }

        /// <summary>
        /// Creates a new WordNode. The confidence is set to the default value of 0.0.
        /// </summary>
        /// <param name="text">The actual text of the word.</param>
        /// <param name="tag">The part-of-speech of the word.</param>
        public WordNode(String text, PartOfSpeechTag tag) : this(text, tag, 0.0) { }

        /// <summary>
        /// Creates a new WordNode. The part-of-speech is set to the default value of Unknown. The confidence is set to the default value of 0.0. 
        /// </summary>
        /// <param name="text">The actual text of the word.</param>
        public WordNode(String text) : this(text, PartOfSpeechTag.Unknown) { }

        /// <summary>
        /// Creates a new WordNode with empty text. The part-of-speech is set to the default value of Unknown. The confidence is set to the default value of 0.0. 
        /// </summary>
        public WordNode() : this("") { }

        /// <summary>
        /// The node's location in the program. For a WordNode, this is always Location.None.
        /// </summary>
        public override Location Location {
            get { return Location.None; }
        }

        /// <summary>
        /// Reports whether the word is in all caps.
        /// </summary>
        /// <returns>True if the word is all caps, False otherwise.</returns>
        public bool IsAllCaps() {
            return AllCaps;
        }

        /// <summary>
        /// Converts the WordNode to a string representation, containing the text and part-of-speech.
        /// </summary>
        /// <returns>A string representation of the WordNode.</returns>
        public override string ToString() {
            return string.Format("{0}({1})", Text, Tag);
        }

        /// <summary>
        /// Returns the text of the word, with no added markup. 
        /// </summary>
        public override string ToPlainString() {
            return this.Text;
        }

        /// <summary>
        /// Creates a WordNode by parsing the given string representation.
        /// </summary>
        /// <param name="source">The string to parse the WordNode from.</param>
        /// <returns>A new WordNode.</returns>
        public static WordNode Parse(string source) {
            if(source == null) {
                throw new ArgumentNullException("source");
            }
            
            string trimmedSource = source.Trim();
            var m = Regex.Match(trimmedSource, @"^(\w*)\((\w*)\)$"); //matches <text>(<tag>)
            if(!m.Success) {
                throw new FormatException("Provided string is not a valid WordNode string representation.");
            }
            string text = m.Groups[1].Value;
            PartOfSpeechTag tag;
            if(!Enum.TryParse<PartOfSpeechTag>(m.Groups[2].Value, out tag)) {
                throw new FormatException("Invalid part-of-speech tag in string.");
            }

            return new WordNode(text, tag);
        }

        /// <summary>
        /// Returns a PhraseNode containing this WordNode.
        /// </summary>
        public override PhraseNode GetParse() {
            return new PhraseNode(new[] {this}, Location, false);
        }

        /// <summary>
        /// Returns a new empty node of the same type.
        /// </summary>
        public virtual WordNode GetNewWord() {
            return new WordNode();
        }

        /// <summary>
        /// Returns a new node of the same type, containing the given text and part-of-speech tag.
        /// </summary>
        /// <param name="text">The text of the new word.</param>
        /// <param name="tag">the part-of-speech tag of the new word.</param>
        public virtual WordNode GetNewWord(string text, PartOfSpeechTag tag) {
            return new WordNode(text, tag);
        }
    }
}

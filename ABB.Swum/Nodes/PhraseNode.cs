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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ABB.Swum;

namespace ABB.Swum.Nodes {
    /// <summary>
    /// Represents a phrase, consisting of one or more words.
    /// </summary>
    public class PhraseNode : Node {
        private bool ContainsPrepostion;
        private List<WordNode> Words;

        /// <summary>
        /// Creates a new empty PhraseNode.
        /// </summary>
        public PhraseNode() {
            this.ContainsPrepostion = false;
            this.Location = Location.None;
            this.Words = new List<WordNode>();
        }

        /// <summary>
        /// Creates a new PhraseNode, containing the given words.
        /// </summary>
        /// <param name="words">The words in the phrase.</param>
        public PhraseNode(IEnumerable<string> words)
            : this() {
            if(words != null) {
                InitWords(words);
            }
        }

        /// <summary>
        /// Creates a new PhraseNode that contains the words resulting from splitting the given identifier.
        /// </summary>
        /// <param name="id">A program identifier.</param>
        /// <param name="splitter">An IdSplitter to split the given identifier into words.</param>
        public PhraseNode(string id, IdSplitter splitter)
            : this() {
            string[] words;
            if(splitter != null) {
                words = splitter.Split(id);
            } else {
                words = new string[] { id };
            }

            InitWords(words);
        }

        /// <summary>
        /// Creates a new PhraseNode that contains the given words.
        /// </summary>
        /// <param name="words">A list of the words in the phrase.</param>
        /// <param name="location">The program location of the phrase.</param>
        /// <param name="containsPreposition">Whether the phrase contains any prepositions or not.</param>
        public PhraseNode(IEnumerable<WordNode> words, Location location, bool containsPreposition) {
            if(words != null) {
                this.Words = new List<WordNode>(words);
            } else {
                this.Words = new List<WordNode>();
            }
            this.Location = location;
            this.ContainsPrepostion = containsPreposition;
        }

        /// <summary>
        /// Initializes the list of WordNodes by creating a new node for each word string.
        /// </summary>
        /// <param name="words">String versions of the words in the phrase.</param>
        protected virtual void InitWords(IEnumerable<string> words) {
            foreach(string word in words) {
                this.Words.Add(new WordNode(word));
            }
        }

        /// <summary>
        /// Creates a string representation of the PhraseNode, containing a string version of each word.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString() {
            return string.Join<WordNode>(" ", this.Words);
        }

        /// <summary>
        /// Return a string representation of the PhraseNode without any added SWUM markup.
        /// </summary>
        public override string ToPlainString() {
            StringBuilder sb = new StringBuilder();
            if(this.Words != null && this.Words.Count > 0) {
                for(int i = 0; i < Words.Count - 1; i++) {
                    sb.AppendFormat("{0} ", Words[i].ToPlainString());
                }
                sb.Append(Words.Last().ToPlainString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates a PhraseNode by parsing the given string representation.
        /// </summary>
        /// <param name="source">The string to parse the PhraseNode from.</param>
        /// <returns>A new PhraseNode.</returns>
        public static PhraseNode Parse(string source) {
            if(source == null) {
                throw new ArgumentNullException("source");
            }

            var words = source.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var pn = new PhraseNode();
            foreach(var word in words) {
                pn.Words.Add(WordNode.Parse(word));
            }
            return pn;
        }

        /// <summary>
        /// Returns a PhraseNode containing the node's parsed name, i.e. returns itself.
        /// </summary>
        public override PhraseNode GetParse() {
            return this;
        }

        /// <summary>
        /// Sets the program location of the node.
        /// </summary>
        /// <param name="location">The location of the node.</param>
        public void SetLocation(Location location) {
            this.Location = location;
        }

        /// <summary>
        /// Returns a list of the words in the phrase.
        /// </summary>
        /// <returns>A list of the words in the phrase.</returns>
        public Collection<WordNode> GetPhrase() {
            return new Collection<WordNode>(this.Words);
        }

        /// <summary>
        /// Returns the word within the phrase at the given index.
        /// </summary>
        /// <param name="wordIndex">The index of the word to get.</param>
        /// <returns>The word within the phrase at the given index.</returns>
        public WordNode GetWord(int wordIndex) {
            return Words[wordIndex];
        }

        /// <summary>
        /// Returns the word within the phrase at the given index.
        /// </summary>
        /// <param name="wordIndex">The index of the word to get.</param>
        /// <returns>The word within the phrase at the given index.</returns>
        public WordNode this[int wordIndex] {
            get { return Words[wordIndex]; }
        }

        /// <summary>
        /// Returns the first word in the phrase.
        /// </summary>
        /// <returns>The first word in the phrase.</returns>
        public WordNode FirstWord() {
            return Words[0];
        }

        /// <summary>
        /// Returns the last word in the phrase.
        /// </summary>
        /// <returns>The last word in the phrase.</returns>
        public WordNode LastWord() {
            return Words.Last();
        }

        /// <summary>
        /// Returns the number of words in the phrase.
        /// </summary>
        /// <returns>The number of words in the phrase.</returns>
        public int Size() {
            return Words.Count;
        }

        /// <summary>
        /// Reports whether there are any words in the phrase.
        /// </summary>
        /// <returns>True if the phrase is empty (contains no words), False otherwise</returns>
        public bool IsEmpty() {
            return (Words.Count == 0);
        }

        /// <summary>
        /// Adds the given word to the end of the phrase.
        /// </summary>
        /// <param name="word">The word to add.</param>
        public void Add(WordNode word) {
            if(word != null) {
                Words.Add(word);
            }
        }

        /// <summary>
        /// Concatenates the supplied phrase to the end of the current phrase.
        /// </summary>
        /// <param name="phrase">The phrase to add.</param>
        public void Add(PhraseNode phrase) {
            if(phrase != null) {
                Words.AddRange(phrase.GetPhrase());
            }
        }

        /// <summary>
        /// Removes the word from the phrase at the given index.
        /// </summary>
        /// <param name="wordIndex">The index of the word to remove.</param>
        public void RemoveWord(int wordIndex) {
            Words.RemoveAt(wordIndex);
        }

        /// <summary>
        /// Creates a new empty node of the same type.
        /// </summary>
        /// <returns>A new empty PhraseNode.</returns>
        public virtual PhraseNode GetNewEmpty() {
            return new PhraseNode();
        }
    }
}

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
using ABB.Swum.Utilities;
using System.IO;

namespace ABB.Swum.WordData
{
    /// <summary>
    /// Encapsulates positional frequency data for words. That is, how often a word appears in a specific postition within an identifier.
    /// </summary>
    public class PositionalFrequencies
    {
        private Dictionary<string, PositionalFrequencyRecord> frequencies;

        /// <summary>
        /// Creates a new PositionalFrequencies object, using the positional frequency data in the default file.
        /// </summary>
        public PositionalFrequencies() : this(SwumConfiguration.GetFileSetting("PositionalFrequencies.FrequenciesFile")) { }

        /// <summary>
        /// Creates a new PostionalFrequencies object, using the positional frequency data in the supplied file.
        /// The file should contain a single word entry per line, in the format [word] [first-freq] [middle-freq] [last-freq] [only-freq]
        /// </summary>
        /// <param name="filePath">The path to the file with the positional frequency data.</param>
        public PositionalFrequencies(string filePath)
        {
            this.frequencies = new Dictionary<string, PositionalFrequencyRecord>();
            using (StreamReader file = new StreamReader(filePath))
            {
                string entry;
                while (!file.EndOfStream)
                {
                    entry = file.ReadLine().Trim();
                    if (!entry.StartsWith("#") && !entry.StartsWith("//")) //ignore commented lines
                    {
                        //the format of an entry is <word> <first> <middle> <last> <only>
                        string[] parts = entry.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 5)
                        {
                            //found a valid entry
                            string word = parts[0];
                            try
                            {
                                int first = int.Parse(parts[1]);
                                int middle = int.Parse(parts[2]);
                                int last = int.Parse(parts[3]);
                                int only = int.Parse(parts[4]);
                                this.AddFrequency(word, first, middle, last, only);
                            }
                            catch (FormatException e)
                            {
                                Console.Error.WriteLine("Invalid entry found in {0}: {1}", filePath, entry);
                                Console.Error.WriteLine(e.ToString());
                            }
                        }
                        else
                        {
                            //not a valid entry
                            Console.Error.WriteLine("Invalid entry found in {0}: {1}", filePath, entry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stores the given frequency data for the given word. This will overwrite any existing data for that word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="first">A count of how often the word appears at the beginning of an identifier.</param>
        /// <param name="middle">A count of how often the word appears in the middle of an identifier, i.e. not first or last.</param>
        /// <param name="last">A count of how often the word appears at the end of an identifier.</param>
        /// <param name="only">A count of how often the word appears by itself in an identifier.</param>
        public void AddFrequency(string word, int first, int middle, int last, int only)
        {
            PositionalFrequencyRecord fr = new PositionalFrequencyRecord { First = first, Middle = middle, Last = last, Only = only, Total = first + middle + last + only };
            this.AddFrequency(word, fr);
        }

        /// <summary>
        /// Stores the given frequency data for the given word. This will overwrite any existing data for that word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="record">The frequency data for the word.</param>
        public void AddFrequency(string word, PositionalFrequencyRecord record)
        {
            frequencies[word] = record;
        }

        /// <summary>
        /// Returns how often the given word appears at the beginning of an identifier.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>A count of how often the given word appears at the beginning of an identifier.</returns>
        public int GetFirstFrequency(string word)
        {
            if (!frequencies.ContainsKey(word))
            {
                return 0;
            }
            else
            {
                return frequencies[word].First;
            }
        }

        /// <summary>
        /// Returns how often the given word appears in the middle of an identifier, i.e. not first or last.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>A count of how often the given word appears in the middle of an identifier.</returns>
        public int GetMiddleFrequency(string word)
        {
            if (!frequencies.ContainsKey(word))
            {
                return 0;
            }
            else
            {
                return frequencies[word].Middle;
            }
        }

        /// <summary>
        /// Returns how often the given word appears at the end of an identifier.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>A count of how often the given word appears at the end of an identifier.</returns>
        public int GetLastFrequency(string word)
        {
            if (!frequencies.ContainsKey(word))
            {
                return 0;
            }
            else
            {
                return frequencies[word].Last;
            }
        }

        /// <summary>
        /// Returns how often the given word appears by itself in an identifier
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>A count of how often the given word appears by itself in an identifier.</returns>
        public int GetOnlyFrequency(string word)
        {
            if (!frequencies.ContainsKey(word))
            {
                return 0;
            }
            else
            {
                return frequencies[word].Only;
            }
        }

        /// <summary>
        /// Returns the total number of times the given word appears in an identifier.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>A count of the total number of times the given word appears in an identifier.</returns>
        public int GetTotalFrequency(string word)
        {
            if (!frequencies.ContainsKey(word))
            {
                return 0;
            }
            else
            {
                return frequencies[word].Total;
            }
        }

        /// <summary>
        /// Calculates the probability that the given word is a verb, based on its positional frequencies.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The probability that the given word is a verb.</returns>
        public double GetVerbProbability(string word)
        {
            double prob = 0d;
            int total = GetTotalFrequency(word);
            if (total > 0)
            {
                prob = 100d * (GetFirstFrequency(word) + GetOnlyFrequency(word)) / total;
            }
            return prob;
        }

        /// <summary>
        /// Calculates the probability that the given word is a noun, based on its positional frequencies.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The probability that the given word is a noun.</returns>
        public double GetNounProbability(string word)
        {
            double prob = 0d;
            int total = GetTotalFrequency(word);
            if (total > 0)
            {
                prob = 100d * (GetMiddleFrequency(word) + GetLastFrequency(word)) / total;
            }
            return prob;
        }
    }
}

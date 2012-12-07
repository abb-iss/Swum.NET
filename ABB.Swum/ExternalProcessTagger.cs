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
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ABB.Swum
{
    /// <summary>
    /// A part-of-speech tagger
    /// </summary>
    public class ExternalProcessTagger
    {
        private string ProgramFileName;
        private string ProgramArguments;

        /// <summary>
        /// Creates a new ExternalProcessTagger with the default program path and arguments.
        /// </summary>
        public ExternalProcessTagger()
        {
//TODO: this should probably read from some central properties file, or similar
//      Furthermore, the ktagger source and executable should be moved into the NLPA solution, so they can be distributed together
            this.ProgramFileName = @"..\..\..\External\pc-parse-20051207\ktagger\Release\ktagger32.exe";
            this.ProgramArguments = @"-x ..\XML.ctl ";
        }

        /// <summary>
        /// Creates a new ExternalProcessTagger with the specified program path and arguments.
        /// </summary>
        /// <param name="programFileName">The path to the part-of-speech tagger executable.</param>
        /// <param name="programArguments">Command-line arguments to be passed to the part-of-speech tagger, not including input and output arguments.</param>
        public ExternalProcessTagger(string programFileName, string programArguments)
        {
            this.ProgramFileName = programFileName;
            this.ProgramArguments = programArguments;
        }

        /// <summary>
        /// Calls an external part-of-speech tagger and runs it on each of the supplied words.
        /// 
        /// Note that each call of this method creates (and cleans up) a separate process running the POS tagger.
        /// For performance reasons, it is better to tag words in large batches, rather than small, frequent batches.
        /// </summary>
        /// <param name="words">A collection of words to be tagged</param>
        /// <returns>A Dictionary that maps a word to the list of part-of-speech records for that word</returns>
        public Dictionary<string, List<WordRecord>> Tag(ICollection<string> words)
        {
            //create temp files for the input and output to the POS tagger process
            string inputFileName = Path.GetTempFileName();
            string outputFileName = Path.GetTempFileName();

            //the part-of-speech dictionary to be returned
            Dictionary<string, List<WordRecord>> posDict = new Dictionary<string, List<WordRecord>>();

            try
            {
                //write the input words to the temp file, one word per line
                using (StreamWriter inputFile = new StreamWriter(inputFileName))
                {
                    foreach (string word in words)
                    {
                        inputFile.WriteLine(word);
                    }
                }

                //start the POS Tagger process
                Process TaggerProcess = new Process();
                ProgramArguments = string.Format("{0} -i {1} -o \"{2}\"", ProgramArguments, inputFileName, outputFileName);
                Console.WriteLine("ProgramArguments: {0}", ProgramArguments);
                ProcessStartInfo tsi = new ProcessStartInfo(ProgramFileName, ProgramArguments);
                tsi.WorkingDirectory = Path.GetDirectoryName(ProgramFileName);
                tsi.UseShellExecute = false; //necessary in order to redirect standard input/output
                TaggerProcess.StartInfo = tsi;
                TaggerProcess.Start();

                //wait for the process to finish, before we start trying to read the output file
                TaggerProcess.WaitForExit();
                TaggerProcess.Close();

                //read the output file
                var taggerOutput = XElement.Load(outputFileName);
                XmlSerializer xs = new XmlSerializer(Type.GetType("ABB.Swum.WordRecord"));

                //query to get the valid WordRecord elements in the output and deserialize them to WordRecord objects
                //WordRecords that don't have sub-elements correspond to input words that the tagger couldn't tag. These are ignored.
                var validWordRecordQuery = from wr in taggerOutput.Elements("WordRecord")
                                           where wr.HasElements
                                           select (WordRecord)xs.Deserialize(wr.CreateReader());

                //place each WordRecord in the dictionary, mapped to its word
                foreach (WordRecord wr in validWordRecordQuery)
                {
                    List<WordRecord> list;
                    if (!posDict.TryGetValue(wr.word, out list))
                    {
                        //word is not already in the dictionary, so create a new list of word records
                        list = new List<WordRecord>();
                    }

                    //add new word record to the list for the word
                    list.Add(wr);
                    posDict[wr.word] = list;
                }

            }
            finally
            {
                //clean up the temporary files that we created
                File.Delete(inputFileName);
                File.Delete(outputFileName);
            }
            
            return posDict;
        }

    }

    /// <summary>
    /// A structure containing the output from the Ktagger POS tagger for a given word
    /// </summary>
    public struct WordRecord
    {
        /// <summary>
        /// The word that was tagged.
        /// </summary>
        public string word;
        /// <summary>
        /// 
        /// </summary>
        public string lex;
        /// <summary>
        /// 
        /// </summary>
        public string gloss;
        /// <summary>
        /// 
        /// </summary>
        public string head_pos;
        /// <summary>
        /// 
        /// </summary>
        public string root;
        /// <summary>
        /// 
        /// </summary>
        public string root_pos;
    }
}

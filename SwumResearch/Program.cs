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
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using Microsoft.Test.CommandLineParsing;
using ABB.SrcML;
using ABB.Swum;
using ABB.Swum.Nodes;
using ABB.Swum.Utilities;
using System.IO;

namespace SwumResearch
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintCommands();
                return;
            }

            Command c = null;
            if (string.Compare(args[0], "split", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                c = new SplitCommand();
            }
            else if (string.Compare(args[0], "expand", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                c = new ExpandCommand();
            }
            else if (string.Compare(args[0], "stem", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                c = new StemCommand();
            }
            else if (string.Compare(args[0], "scratch", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                c = new ScratchCommand();
            }
            else if (string.Equals(args[0], "GetIdentifiers", StringComparison.InvariantCultureIgnoreCase))
            {
                c = new GetIdentifiersCommand();
            }
            else if (string.Equals(args[0], "AnalyzeFunctions", StringComparison.InvariantCultureIgnoreCase))
            {
                c = new AnalyzeFunctionsCommand();
            }
            else if (string.Equals(args[0], "CountProgramWords", StringComparison.InvariantCultureIgnoreCase))
            {
                c = new CountProgramWordsCommand();
            }
            else if (string.Equals(args[0], "Random", StringComparison.InvariantCultureIgnoreCase))
            {
                c = new RandomCommand();
            } else if(string.Equals(args[0], "swum", StringComparison.InvariantCultureIgnoreCase)) {
                c = new SwumCommand();
            } else {
                PrintCommands();
            }

            if (c == null) { return; }
            else
            {
                try
                {
                    c.ParseArguments(args.Skip(1));
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    CommandLineParser.PrintUsage(c);
                    return;
                }

                c.Execute();
            }

            return;

        }

        static void PrintCommands()
        {
            List<Command> cl = new List<Command>();
            cl.Add(new SplitCommand());
            cl.Add(new ExpandCommand());
            cl.Add(new StemCommand());
            cl.Add(new ScratchCommand());
            cl.Add(new GetIdentifiersCommand());
            cl.Add(new AnalyzeFunctionsCommand());
            cl.Add(new CountProgramWordsCommand());
            cl.Add(new RandomCommand());
            cl.Add(new SwumCommand());
            CommandLineParser.PrintCommands(cl);
        }
    }

    [Description("Splits the supplied identifier.")]
    class SplitCommand : Command
    {
        [Description("The identifier to split")]
        public string Identifier { get; set; }

        [Description("A file containing identifiers to split, one per line")]
        public string IdentifierFile { get; set; }

        [Description("The path to the SWUM model")]
        public string Model { get; set; }

        [Description("Print a trace of the splitting process.")]
        public bool Trace { get; set; }

        public override void Execute()
        {
            //set default values for optional arguments
            if (Model == null)
            {
                Model = "NotepadPlusPlus6.2_words.count";
            }

            if (Identifier == null && IdentifierFile == null)
            {
                Console.Error.WriteLine("Must specify either Identifier or IdentifierFile");
                return;
            }

            SamuraiIdSplitter splitter = new SamuraiIdSplitter(Model);

            if (Identifier != null)
            {
                string[] words = splitter.Split(Identifier, Trace);
                Console.WriteLine("{0} -> {1}", Identifier, string.Join(" ", words));
            }

            if (IdentifierFile != null)
            {
                using (StreamReader file = new StreamReader(IdentifierFile))
                {
                    string word;
                    while (!file.EndOfStream)
                    {
                        word = file.ReadLine();
                        string[] splitWords = splitter.Split(word, Trace);
                        Console.WriteLine("{0} -> {1}", word, string.Join(" ", splitWords));
                    }
                }
            }
        }
    }

    [Description("Expands an abbreviation")]
    class ExpandCommand : Command
    {
        [Required]
        [Description("The short form to expand.")]
        public string Word { get; set; }

        [Required]
        [Description("The srcML file")]
        public string File { get; set; }

        public override void Execute()
        {
            SrcMLFile testFile = new SrcMLFile(this.File);
            XElement firstFile = testFile.FileUnits.First();

            //get all the functions
            var containers = new System.Collections.ObjectModel.Collection<XName>() { SRC.Function, SRC.Constructor, SRC.Destructor };
            var funcs = from func in firstFile.Descendants()
                        where containers.Any(c => c == func.Name)
                        select func;

            Dictionary<XElement, string> functionComments = new Dictionary<XElement, string>();


            //grab the comment block for each function
            foreach (var func in funcs)
            {
                StringBuilder functionComment = new StringBuilder();

                var prevElements = func.ElementsBeforeSelf().Reverse();
                foreach (var element in prevElements)
                {
                    if (element.Name == SRC.Comment)
                    {
                        //add comment to beginning of comment block
                        functionComment.Insert(0, element.Value + System.Environment.NewLine);
                    }
                    else
                    {
                        //found something besides a comment
                        break;
                    }
                }

                functionComments[func] = functionComment.ToString();
            }

            AbbreviationExpander ae = new AbbreviationExpander();
            ae.Expand(Word, functionComments.First().Key, functionComments.First().Value, "");

            //string patternRegex;
            //string shortForm = Word;
            //StringBuilder sb = new StringBuilder(@"\b");
            //for (int i = 0; i < shortForm.Length - 1; i++)
            //{
            //    sb.AppendFormat(@"{0}\w+\s+", shortForm[i]);
            //}
            //sb.AppendFormat(@"{0}\w+\b", shortForm[shortForm.Length - 1]);
            //patternRegex = sb.ToString();

            //Console.WriteLine("Acronym regex: {0}", patternRegex);

            //string methodComment = functionComments.First().Value;
            //Match m = Regex.Match(methodComment, string.Format("({0})", patternRegex), RegexOptions.IgnoreCase);
            //if (m.Success)
            //{
            //    Console.WriteLine("Found match: {0}", m.Groups[1].Value);
            //}
        }
    }

    [Description("Stems the supplied word")]
    class StemCommand : Command
    {
        [Required]
        [Description("The word to stem.")]
        public string Word { get; set; }

        [Description("The file containing the stem data")]
        public string StemFile { get; set; }

        public override void Execute()
        {
            //set defaults for optional parameters
            if (StemFile == null)
            {
                StemFile = @"lib\KStem.txt";
            }

            FileStemmer stemmer = new FileStemmer(StemFile);
            Console.WriteLine("{0} -> {1}", Word, stemmer.Stem(Word));
        }
    }

    [Description("This is a \"scratchpad\" command for running whatever.")]
    class ScratchCommand : Command
    {
        //[Description("The srcML file to use.")]
        //[Required]
        //public string File { get; set; }

        public override void Execute()
        {

            //string srcMLFormat;

            ////construct the necessary srcML wrapper unit tags
            //XmlNamespaceManager xnm = SrcML.SrcML.NamespaceManager;

            //StringBuilder namespaceDecls = new StringBuilder();
            //foreach (string prefix in xnm)
            //{
            //    if (prefix != string.Empty && !prefix.StartsWith("xml", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        if (prefix.Equals("src", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            namespaceDecls.AppendFormat("xmlns=\"{0}\" ", xnm.LookupNamespace(prefix));
            //        }
            //        else
            //        {
            //            namespaceDecls.AppendFormat("xmlns:{0}=\"{1}\" ", prefix, xnm.LookupNamespace(prefix));
            //        }
            //    }
            //}
            //srcMLFormat = string.Format("<unit {0}>{{0}}</unit>", namespaceDecls.ToString());

            //string testSrcML = "<class>class <name>foo</name> <block>{<private type=\"default\"><decl_stmt><decl><type><name>bool</name></type> <name>MyVariable</name></decl>;</decl_stmt></private>}</block>;</class>";
            //XElement xml = XElement.Parse(string.Format(srcMLFormat, testSrcML), LoadOptions.PreserveWhitespace);
            //FieldContext fc = ContextBuilder.BuildFieldContext(xml.Descendants(SRC.Declaration).First());

            //FieldDeclarationNode fdn = new FieldDeclarationNode("myVariable", fc);
            ////var splitter = new ConservativeIdSplitter();
            ////var tagger = new UnigramTagger();
            ////var posData = new PCKimmoPartOfSpeechData();

            //FieldRule rule = new FieldRule();
            //rule.ConstructSwum(fdn);
            //Console.WriteLine(fdn.ToString());

            Console.WriteLine(SwumConfiguration.GetSetting("PCKimmoPartOfSpeechData.TwoDictFile"));

        }
    }

    [Description("Prints the names of the identifiers in a srcML file.")]
    class GetIdentifiersCommand : Command
    {
        [Required]
        [Description("The srcML file.")]
        public string File { get; set; }

        [Description("Print parameter names")]
        public bool Parameters { get; set; }

        [Description("Print variable names")]
        public bool Variables { get; set; }

        [Description("Print all the identifiers found")]
        public bool PrintAll { get; set; }

        public override void Execute()
        {
            Console.WriteLine("Using file {0}", this.File);

            SrcMLFile testFile = new SrcMLFile(this.File);
            HashSet<string> sameCaseWords = new HashSet<string>();
            //ConservativeIdSplitter splitter = new ConservativeIdSplitter();

            var functionTypes = new List<XName>() { SRC.Function, SRC.Constructor, SRC.Destructor };
            foreach (XElement file in testFile.FileUnits)
            {
                Console.WriteLine("File {0}:", file.Attribute("filename").Value);

                //print all the function names
                //var funcs = from func in file.Descendants()
                //            where functionTypes.Any(c => c == func.Name)
                //            select func;
                foreach (var func in (from func in file.Descendants()
                                      where functionTypes.Any(c => c == func.Name)
                                      select func))
                {
                    string funcName = SrcMLHelper.GetNameForMethod(func).Value;
                    if (IsSameCase(funcName))
                    {
                        sameCaseWords.Add(funcName);
                    }
                    if (PrintAll) { Console.WriteLine("<{0}> {1}", func.Name.LocalName, funcName); }
                }

                //print all the parameter names
                if (this.Parameters)
                {
                    foreach (var param in file.Descendants(SRC.Parameter))
                    {
                        if (param.Element(SRC.Declaration) != null)
                        {
                            var paramName = param.Element(SRC.Declaration).Element(SRC.Name);
                            if (paramName != null)
                            {
                                if (IsSameCase(paramName.Value))
                                {
                                    sameCaseWords.Add(paramName.Value);
                                }
                                if (PrintAll) { Console.WriteLine("<{0}> {1}", param.Name.LocalName, paramName.Value); }
                            }
                        }
                    }
                }

                //print all the variable names
                if (this.Variables)
                {
                    foreach (var declStmt in file.Descendants(SRC.DeclarationStatement))
                    {
                        foreach (var decl in declStmt.Elements(SRC.Declaration))
                        {
                            var variableName = decl.Element(SRC.Name);
                            if (variableName != null)
                            {
                                //if a qualified name, grab the last name
                                var childNames = variableName.Elements(SRC.Name);
                                if (childNames.Count() > 1)
                                {
                                    variableName = childNames.Last();
                                }

                                if (IsSameCase(variableName.Value))
                                {
                                    sameCaseWords.Add(variableName.Value);
                                }
                                if (PrintAll) { Console.WriteLine("<{0}> {1}", declStmt.Name.LocalName, variableName.Value); }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("=== sameCaseWords: ===");
            foreach (string word in sameCaseWords)
            {
                Console.WriteLine(word);
            }
        }

        private bool IsSameCase(string identifier)
        {
            if (!identifier.Contains('_') && (identifier == identifier.ToLower() || identifier == identifier.ToUpper()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Description("Constructs the SWUM for each function in a srcML file.")]
    class AnalyzeFunctionsCommand : Command
    {
        [Required]
        [Description("The srcML file.")]
        public string File { get; set; }

        [Description("The word count file to use for the Samurai splitter.")]
        public string CountFile { get; set; }

        [Description("The percentage of methods to randomly sample and construct SWUM on.")]
        public int SamplePercent { get; set; }

        [Description("The number of methods to randomly sample and construct SWUM on. If both SamplePercent and SampleCount are specified, SamplePercent is ignored.")]
        public int SampleCount { get; set; }

        public override void Execute()
        {
            Console.WriteLine("Using srcML file {0}", this.File);

            UnigramSwumBuilder builder = new UnigramSwumBuilder();
            if (CountFile != null)
            {
                Console.WriteLine("Initializing SamuraiIdSplitter using word count file {0}", this.CountFile);
                builder.Splitter = new SamuraiIdSplitter(CountFile);
            }
            Console.WriteLine("SwumBuilder initialized");

            if (this.SamplePercent <= 0) { this.SamplePercent = 100; }
            Random rand = new Random();

            SrcMLFile testFile = new SrcMLFile(this.File);
            int methodCount = 0;
            var functionTypes = new XName[] { SRC.Function, SRC.Constructor, SRC.Destructor };
            foreach (XElement file in testFile.FileUnits)
            {
                Console.WriteLine("File {0}:", file.Attribute("filename").Value);

                //print all the function names
                foreach (var func in (from func in file.Descendants()
                                      where functionTypes.Any(c => c == func.Name) && !func.Ancestors(SRC.Declaration).Any() && (rand.Next(100) < this.SamplePercent)
                                      select func))
                {
                    string funcName = SrcMLHelper.GetNameForMethod(func).Value;
                    Console.WriteLine("<{0}> {1}", func.Name.LocalName, GetMethodSignature(func));

                    MethodDeclarationNode mdn = new MethodDeclarationNode(funcName, ContextBuilder.BuildMethodContext(func));
                    builder.ApplyRules(mdn);
                    Console.WriteLine(mdn.ToString() + Environment.NewLine);
                    methodCount++;
                }
            }
            Console.WriteLine("{0} functions analyzed", methodCount);
            
        }

        private string GetMethodSignature(XElement methodElement)
        {
            var blockElement = methodElement.Element(SRC.Block);
            StringBuilder sig = new StringBuilder();
            foreach (var n in blockElement.NodesBeforeSelf())
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    sig.Append(((XElement)n).Value);
                }
                else if (n.NodeType == XmlNodeType.Text || n.NodeType == XmlNodeType.Whitespace || n.NodeType== XmlNodeType.SignificantWhitespace)
                {
                    sig.Append(((XText)n).Value);
                }
            }
            return sig.ToString().TrimEnd();
        }
    }

    [Description("Counts the occurrences of words in program identifiers")]
    class CountProgramWordsCommand : Command
    {
        [Required]
        [Description("The srcML file to read from.")]
        public string File { get; set; }

        [Required]
        [Description("The file to write the output to.")]
        public string OutFile { get; set; }

        public override void Execute()
        {
            var obs = SamuraiIdSplitter.CountProgramWords(File);
            LibFileLoader.WriteWordCount(obs, OutFile);
        }
    }

    [Description("Generates a sequence of random numbers")]
    class RandomCommand : Command
    {
        [Required]
        [Description("The number of random numbers to generate")]
        public int Count { get; set; }

        [Description("The maximum value of the generated random numbers. The default is 100.")]
        public int MaxValue { get; set; }

        public override void Execute()
        {
            Random rand = new Random();
            if(MaxValue <= 0){MaxValue = 100;}
            for (int i = 0; i < Count; i++)
            {
                Console.WriteLine(rand.Next(MaxValue));
            }
            
        }
    }

    [Description("Computes SWUM for the methods and fields in the given srcML file.")]
    class SwumCommand : Command {
        [Required]
        [Description("The srcML file.")]
        public string File { get; set; }

        [Description("The word count file to use for the Samurai splitter.")]
        public string CountFile { get; set; }

        [Description("Pause before and after swum construction.")]
        public bool Pause { get; set; }

        [Description("Print out the generated SWUM for each method and field.")]
        public bool PrintSwum { get; set; }

        private Dictionary<string, MethodDeclarationNode> methodSwum;
        private Dictionary<string, FieldDeclarationNode> fieldSwum;

        public SwumCommand() {
            methodSwum = new Dictionary<string, MethodDeclarationNode>();
            fieldSwum = new Dictionary<string, FieldDeclarationNode>();
        }

        public override void Execute() {
            if(Pause) {
                Console.WriteLine("Ready to begin (press Enter)");
                Console.ReadLine();
            }

            Console.WriteLine("Using srcML file {0}", this.File);

            var builder = new UnigramSwumBuilder();
            if(!string.IsNullOrWhiteSpace(CountFile)) {
                Console.WriteLine("Initializing SamuraiIdSplitter using word count file {0}", this.CountFile);
                builder.Splitter = new SamuraiIdSplitter(CountFile);
            }
            Console.WriteLine("SwumBuilder initialized");

            int methodCount = 0, fieldCount = 0;

            {
                SrcMLFile testFile = new SrcMLFile(this.File);
                var functionTypes = new XName[] {SRC.Function, SRC.Constructor, SRC.Destructor};
                foreach(XElement file in testFile.FileUnits) {
                    string fileName = file.Attribute("filename").Value;
                    Console.WriteLine("File {0}:", fileName);

                    //compute SWUM on each function
                    foreach(var func in (from func in file.Descendants()
                                         where functionTypes.Contains(func.Name) && !func.Ancestors(SRC.Declaration).Any()
                                         select func)) {
                        var nameElement = SrcMLHelper.GetNameForMethod(func);
                        if(nameElement != null) {
                            string funcName = nameElement.Value;
                            string funcSignature = SrcMLElement.GetMethodSignature(func);
                            if(PrintSwum) {
                                Console.WriteLine("<{0}> {1}", func.Name.LocalName, funcSignature);
                            }

                            MethodDeclarationNode mdn = new MethodDeclarationNode(funcName, ContextBuilder.BuildMethodContext(func));
                            builder.ApplyRules(mdn);
                            methodSwum[string.Format("{0}:{1}", fileName, funcSignature)] = mdn;
                            if(PrintSwum) {
                                Console.WriteLine(mdn.ToString() + Environment.NewLine);
                            }

                            methodCount++;
                        }
                    }

                    //compute SWUM on each field
                    foreach(var fieldDecl in (from declStmt in file.Descendants(SRC.DeclarationStatement)
                                          where !declStmt.Ancestors().Any(n => functionTypes.Contains(n.Name))
                                          select declStmt.Element(SRC.Declaration))) {

                        int declPos = 1;
                        foreach(var nameElement in fieldDecl.Elements(SRC.Name)) {

                            string fieldName = nameElement.Elements(SRC.Name).Any() ? nameElement.Elements(SRC.Name).Last().Value : nameElement.Value;
                            if(PrintSwum) {
                                Console.WriteLine("Field: {0}, Name: {1}", fieldDecl.Value, fieldName);
                            }

                            FieldDeclarationNode fdn = new FieldDeclarationNode(fieldName, ContextBuilder.BuildFieldContext(fieldDecl));
                            builder.ApplyRules(fdn);
                            fieldSwum[string.Format("{0}:{1}:{2}", fileName, fieldDecl.Value, declPos)] = fdn;
                            if(PrintSwum) {
                                Console.WriteLine(fdn.ToString() + Environment.NewLine);
                            }

                            fieldCount++;
                            declPos++;
                        }
                    }
                }
                
            } 

            GC.Collect();

            Console.WriteLine("{0} functions analyzed", methodCount);
            Console.WriteLine("{0} functions in dictionary", methodSwum.Count);
            Console.WriteLine("{0} fields analyzed", fieldCount);
            Console.WriteLine("{0} fields in dictionary", fieldSwum.Count);

            if(Pause) {
                Console.WriteLine("Finished building SWUM (press Enter)");
                Console.ReadLine();
            }
        }

        private string GetMethodSignature(XElement methodElement) {
            var blockElement = methodElement.Element(SRC.Block);
            StringBuilder sig = new StringBuilder();
            foreach(var n in blockElement.NodesBeforeSelf()) {
                if(n.NodeType == XmlNodeType.Element) {
                    sig.Append(((XElement)n).Value);
                } else if(n.NodeType == XmlNodeType.Text || n.NodeType == XmlNodeType.Whitespace || n.NodeType == XmlNodeType.SignificantWhitespace) {
                    sig.Append(((XText)n).Value);
                }
            }
            return sig.ToString().TrimEnd();
        }

        private string GetVariableName(XElement declElement) {
            var nameElement = declElement.Element(SRC.Name);
            if(nameElement == null) {
                return string.Empty;
            }
            if(nameElement.Elements(SRC.Name).Any()) {
                return nameElement.Elements(SRC.Name).Last().Value;
            } else {
                return nameElement.Value;
            }
        }
    }
}

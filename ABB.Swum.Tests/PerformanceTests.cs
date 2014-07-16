using ABB.SrcML;
using ABB.Swum.Nodes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ABB.Swum.Tests
{
    [TestFixture]
    public class PerformanceTests
    {
        private readonly XName[] _functionTypes = new XName[] { SRC.Function, SRC.Constructor, SRC.Destructor };
        private IEnumerable<XElement> functions;
        private UnigramSwumBuilder builder;

        [SetUp]
        public void SetUp()
        {
            builder = new UnigramSwumBuilder { Splitter = new CamelIdSplitter() };
            var Generator = new SrcMLGenerator(@"..\..\..\External\SrcML");
            var file = Generator.GenerateSrcMLFileFromDirectory(@"..\..\..\", Path.GetTempFileName(), new List<string>(), Language.Any);

            functions = from func in file.GetXDocument().Descendants()
                            where _functionTypes.Contains(func.Name) && !func.Ancestors(SRC.Declaration).Any()
                            select func;
        }
        
        [Test]
        public void AddManyFiles()
        {

            for (int i = 0; i < 100; i++)
            {
                foreach (XElement methodElement in functions)
                {
                    string funcName = SrcMLElement.GetNameForMethod(methodElement).Value;
                    MethodContext mc = ContextBuilder.BuildMethodContext(methodElement);
                    MethodDeclarationNode mdn = new MethodDeclarationNode(funcName, mc);
                    builder.ApplyRules(mdn);
                }
            }
        }

        public class CamelIdSplitter : ConservativeIdSplitter
        {
            public override string[] Split(string identifier)
            {
                //do initial conservative split
                var words = base.Split(identifier);
                var result = new List<string>();
                //search for any words that start with two or more uppercase letters, followed by one or more lowercase letters
                foreach (var word in words)
                {
                    var m = Regex.Match(word, @"^(\p{Lu}+)(\p{Lu}\p{Ll}+)$");
                    if (m.Success)
                    {
                        //regex matches, split and add each part
                        result.Add(m.Groups[1].Value);
                        result.Add(m.Groups[2].Value);
                    }
                    else
                    {
                        result.Add(word);
                    }
                }

                return result.ToArray();
            }
        }

    }
}

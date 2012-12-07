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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ABB.Swum.Utilities.Tests
{
    [TestFixture]
    public class SwumConfigurationTests
    {
        [Test]
        public void TestGetSetting()
        {
            Assert.AreEqual("lib/dict.2.txt", SwumConfiguration.GetSetting("PCKimmoPartOfSpeechData.TwoDictFile"));
        }

        [Test]
        public void TestGetSetting_MissingSetting()
        {
            Assert.IsNull(SwumConfiguration.GetSetting("NonExistentSetting"));
        }

        [Test]
        public void TestGetFileSetting()
        {
            Console.WriteLine("With assembly path: {0}", SwumConfiguration.GetFileSetting("PCKimmoPartOfSpeechData.TwoDictFile"));
        }

        [Test]
        public void TestGetFileSetting_MissingSetting()
        {
            Assert.IsNull(SwumConfiguration.GetFileSetting("NonExistentSetting"));
        }
    }
}

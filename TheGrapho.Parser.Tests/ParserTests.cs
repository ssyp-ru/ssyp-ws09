// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace TheGrapho.Parser.Tests
{
    public class ParserTests
    {
        [Test]
        [TestCaseSource(typeof(TestSetSource), nameof(TestSetSource.TestCases))]
        public void Parsing([DisallowNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            TestUtilities.TestParsing(File.ReadAllText(path));
        }

        [Test]
        [TestCaseSource(typeof(TestSetSource), nameof(TestSetSource.TestCases))]
        public void Scanning([DisallowNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            TestUtilities.TestScanning(File.ReadAllText(path));
        }
    }
}
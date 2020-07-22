// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace TheGrapho.Parser.Tests
{
    public class ErrorHandlingTests
    {
        [Test]
        [TestCaseSource(typeof(ParseErrorsTestSetSource), nameof(TestSetSource.TestCases))]
        public void Parsing([DisallowNull] string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            TestUtilities.TestParsingErrors(input);
        }

        [Test]
        [TestCaseSource(typeof(ScanErrorsTestSetSource), nameof(TestSetSource.TestCases))]
        public void Scanning([DisallowNull] string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            TestUtilities.TestScanningErrors(input);
        }
    }
}
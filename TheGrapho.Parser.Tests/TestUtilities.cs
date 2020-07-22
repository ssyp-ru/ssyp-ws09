// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TheGrapho.Parser.Tests
{
    public static class TestUtilities
    {
        public static void TestScanning([DisallowNull] string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var target = new StringBuilder();

            new Scanner(input).ScanTillEnd().Select(it =>
                {
                    Console.WriteLine(it);
                    return it;
                })
                .ToList()
                .ForEach(it => it.Write(target));

            Assert.AreEqual(input, target.ToString());
        }

        public static void TestScanningErrors([DisallowNull] string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            Console.WriteLine(Assert.Throws<ScannerException>(() => { TestScanning(input); }).ToString());
        }

        public static void TestParsing([DisallowNull] string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var target = new StringBuilder();
            var tree = new Parser(new Scanner(input).ScanTillEnd().ToList()).Parse();
            Console.WriteLine(tree);
            tree.Write(target);
            Assert.AreEqual(input, target.ToString());
        }

        public static void TestParsingErrors([DisallowNull] string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            Console.WriteLine(Assert.Throws<ParserException>(() => { TestParsing(input); }));
        }
    }
}
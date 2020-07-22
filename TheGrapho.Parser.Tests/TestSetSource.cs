// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace TheGrapho.Parser.Tests
{
    public class TestSetSource
    {
        [NotNull] private const string SamplesName = "samples";

        [NotNull]
        public static IEnumerable TestCases
        {
            get
            {
                var testSuiteDirectory = FindTestSuiteDirectory();

                Debug.Assert(testSuiteDirectory != null);

                var testSuiteDirectoryInfo = new DirectoryInfo(testSuiteDirectory);

                foreach (var file in testSuiteDirectoryInfo.EnumerateFiles("*.gv"))
                    yield return new TestCaseData(file.FullName);
            }
        }

        [return: MaybeNull]
        private static string? FindTestSuiteDirectory()
        {
            var rootPath = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

            while (rootPath != null && !Directory.Exists(Path.Combine(rootPath.FullName, SamplesName)))
                rootPath = rootPath.Parent;

            return rootPath != null ? Path.Combine(rootPath.FullName, SamplesName) : null;
        }
    }
}
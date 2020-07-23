// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Tests
{
    public class ParseErrorsTestSetSource
    {
        [NotNull]
        public static IEnumerable TestCases => new[]
        {
            "foo",
            "123",
            "strict foo",
            "graph",
            "graph{",
            "graph{{}",
            "graph { foo= }",
            "graph{subgraph}",
            "graph{node}"
        };
    }

    public class ScanErrorsTestSetSource
    {
        [NotNull] public static IEnumerable TestCases => new[] {"&", "<", "\""};
    }
}
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TheGrapho.Parser.Syntax
{
    internal static class Grammar
    {
        [NotNull]
        public static Dictionary<string, SyntaxKind> Punctuation { get; } = new Dictionary<string, SyntaxKind>
        {
            {";", SyntaxKind.SemicolonToken}, {"{", SyntaxKind.LeftCurlyBracketToken},
            {"}", SyntaxKind.RightCurlyBracketToken}, {"[", SyntaxKind.LeftSquareBracketToken},
            {"]", SyntaxKind.RightSquareBracketToken}, {",", SyntaxKind.CommaToken}, {":", SyntaxKind.ColonToken},
            {"=", SyntaxKind.EqualsSignToken}, {"->", SyntaxKind.EdgeOpTokenArrow}, {"--", SyntaxKind.EdgeOpTokenBar}
        };

        [NotNull]
        public static Dictionary<string, SyntaxKind> Keywords { get; } = new Dictionary<string, SyntaxKind>
        {
            {"strict", SyntaxKind.StrictToken}, {"graph", SyntaxKind.GraphToken}, {"digraph", SyntaxKind.DigraphToken},
            {"node", SyntaxKind.NodeToken}, {"subgraph", SyntaxKind.SubgraphToken}, {"edge", SyntaxKind.EdgeToken}
        };

        public static int MaxKeywordLength { get; } = Keywords.Keys.Max(b => b.Length);
    }
}
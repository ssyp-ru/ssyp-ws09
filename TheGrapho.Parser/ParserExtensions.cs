// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TheGrapho.Parser.Syntax;

namespace TheGrapho.Parser
{
    internal static class ParserExtensions
    {
        [return: MaybeNull]
        internal static SyntaxToken RequireKind([DisallowNull] this SyntaxToken token,
            [DisallowNull] params SyntaxKind[] kind)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (kind == null) throw new ArgumentNullException(nameof(kind));
            return !kind.ToList().Contains(token.Kind) ? null : token;
        }
    }
}
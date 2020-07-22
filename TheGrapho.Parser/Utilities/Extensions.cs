// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TheGrapho.Parser.Syntax;

namespace TheGrapho.Parser.Utilities
{
    public static class Extensions
    {
        public static void WriteAll(
            [DisallowNull] this IEnumerable<SyntaxNode> source,
            [DisallowNull] StringBuilder target)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));
            foreach (var syntaxNode in source.Where(it => it != null)) syntaxNode.Write(target);
        }
    }
}
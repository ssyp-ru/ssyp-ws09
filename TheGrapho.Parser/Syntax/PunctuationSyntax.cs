// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TheGrapho.Parser.Utilities;

namespace TheGrapho.Parser.Syntax
{
    public sealed class PunctuationSyntax : SyntaxToken
    {
        public PunctuationSyntax(SyntaxKind kind, int start, int fullWidth, [DisallowNull] string value) :
            base(kind, start, fullWidth)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull] public string Value { get; }

        public override void Write([DisallowNull] StringBuilder target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            LeadingTrivia.WriteAll(target);
            target.Append(Value);
            TrailingTrivia.WriteAll(target);
        }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Value)}: {Value}";
    }
}
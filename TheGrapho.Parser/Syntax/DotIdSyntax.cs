// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotIdSyntax : DotSyntax
    {
        public DotIdSyntax([DisallowNull] StringSyntax value) : base(
            SyntaxKind.DotId,
            value?.Start ?? 0,
            value?.FullWidth ?? 0,
            new[] {value})
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull] public StringSyntax Value { get; }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Value)}: {Value}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitId(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitId(this);
        }
    }
}
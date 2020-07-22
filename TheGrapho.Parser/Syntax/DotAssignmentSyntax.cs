// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotAssignmentSyntax : DotSyntax
    {
        public DotAssignmentSyntax([DisallowNull] DotIdSyntax key,
            [DisallowNull] PunctuationSyntax equalsSign,
            [DisallowNull] DotIdSyntax value) : base(
            SyntaxKind.DotAssignment,
            key?.Start ?? 0,
            (key?.FullWidth ?? 0) + (equalsSign?.FullWidth ?? 0) + (value?.FullWidth ?? 0),
            new SyntaxNode?[] {key, equalsSign, value})
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            EqualsSign = equalsSign ?? throw new ArgumentNullException(nameof(equalsSign));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull] public DotIdSyntax Key { get; }
        [NotNull] public PunctuationSyntax EqualsSign { get; }
        [NotNull] public DotIdSyntax Value { get; }

        [return: NotNull]
        public override string ToString() =>
            $"{base.ToString()}, {nameof(Key)}: {Key}, {nameof(EqualsSign)}: {EqualsSign}, {nameof(Value)}: {Value}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitAssignment(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitAssignment(this);
        }
    }
}
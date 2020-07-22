// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotEdgeOperatorSyntax : DotSyntax
    {
        public DotEdgeOperatorSyntax([DisallowNull] PunctuationSyntax arrowOrBar) : base(
            SyntaxKind.DotEdgeOperator,
            arrowOrBar?.Start ?? 0,
            arrowOrBar?.FullWidth ?? 0,
            new[] {arrowOrBar})
        {
            ArrowOrBar = arrowOrBar ?? throw new ArgumentNullException(nameof(arrowOrBar));
        }

        [NotNull] public PunctuationSyntax ArrowOrBar { get; }
        public override string ToString() => $"{base.ToString()}, {nameof(ArrowOrBar)}: {ArrowOrBar}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitEdgeOperator(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitEdgeOperator(this);
        }
    }
}
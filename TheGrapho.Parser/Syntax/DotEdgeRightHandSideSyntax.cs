// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotEdgeRightHandSideSyntax : DotSyntax
    {
        public DotEdgeRightHandSideSyntax(
            [DisallowNull] IReadOnlyList<(DotEdgeOperatorSyntax, DotSyntax)> edges) :
            base(
                SyntaxKind.DotEdgeRightHandSide,
                edges?.First().Item1?.Start ?? 0,
                edges?.Sum(it => it.Item1.FullWidth + it.Item2.FullWidth) ?? 0,
                edges?.SelectMany(it => new SyntaxNode[] {it.Item1, it.Item2}).ToList())
        {
            Edges = edges ?? throw new ArgumentNullException(nameof(edges));
            if (edges.Count == 0) throw new IndexOutOfRangeException(nameof(edges));

            foreach (var (edgeOperator, syntax) in edges)
            {
                if (edgeOperator == null) throw new ArgumentNullException(nameof(edges));
                if (syntax == null) throw new ArgumentNullException(nameof(edges));
            }
        }

        [NotNull] public IReadOnlyList<(DotEdgeOperatorSyntax EdgeOperator, DotSyntax NodeIdOrSubgraph)> Edges { get; }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Edges)}: {Edges}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitEdgeRightHandSide(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitEdgeRightHandSide(this);
        }
    }
}
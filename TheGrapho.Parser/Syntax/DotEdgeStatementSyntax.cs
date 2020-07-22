// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotEdgeStatementSyntax : DotSyntax
    {
        public DotEdgeStatementSyntax([DisallowNull] DotSyntax nodeIdOrSubgraph,
            [DisallowNull] DotEdgeRightHandSideSyntax edgeRhs,
            [AllowNull] DotAttributeListSyntax? attributeList) : base(
            SyntaxKind.DotEdgeStatement,
            nodeIdOrSubgraph?.Start ?? 0,
            (nodeIdOrSubgraph?.FullWidth ?? 0) + (edgeRhs?.FullWidth ?? 0) + (attributeList?.FullWidth ?? 0),
            new[] {nodeIdOrSubgraph, edgeRhs, attributeList})
        {
            NodeIdOrSubgraph = nodeIdOrSubgraph ?? throw new ArgumentNullException(nameof(nodeIdOrSubgraph));
            EdgeRhs = edgeRhs ?? throw new ArgumentNullException(nameof(edgeRhs));
            AttributeList = attributeList;
        }

        [NotNull] public DotSyntax NodeIdOrSubgraph { get; }
        [NotNull] public DotEdgeRightHandSideSyntax EdgeRhs { get; }
        [MaybeNull] public DotAttributeListSyntax? AttributeList { get; }

        [return: NotNull]
        public override string ToString() =>
            $"{base.ToString()}, {nameof(NodeIdOrSubgraph)}: {NodeIdOrSubgraph}, {nameof(EdgeRhs)}: {EdgeRhs}, {nameof(AttributeList)}: {AttributeList}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitEdgeStatement(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitEdgeStatement(this);
        }
    }
}
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotNodeStatementSyntax : DotSyntax
    {
        public DotNodeStatementSyntax([DisallowNull] DotNodeIdSyntax nodeId,
            [AllowNull] DotAttributeListSyntax? attributeList) : base(
            SyntaxKind.DotNodeStatement,
            nodeId?.Start ?? 0,
            (nodeId?.FullWidth ?? 0) + (attributeList?.FullWidth ?? 0),
            new DotSyntax?[] {nodeId, attributeList})
        {
            NodeId = nodeId ?? throw new ArgumentNullException(nameof(nodeId));
            AttributeList = attributeList;
        }

        [NotNull] public DotNodeIdSyntax NodeId { get; }
        [MaybeNull] public DotAttributeListSyntax? AttributeList { get; }

        [return: NotNull]
        public override string ToString() =>
            $"{base.ToString()}, {nameof(NodeId)}: {NodeId}, {nameof(AttributeList)}: {AttributeList}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitNodeStatement(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitNodeStatement(this);
        }
    }
}
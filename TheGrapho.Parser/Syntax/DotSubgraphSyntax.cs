// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotSubgraphSyntax : DotSyntax
    {
        public DotSubgraphSyntax([AllowNull] (KeywordSyntax, DotIdSyntax)? subgraph,
            [DisallowNull] PunctuationSyntax leftCurlyBracket,
            [DisallowNull] DotStatementListSyntax statementList,
            [DisallowNull] PunctuationSyntax rightCurlyBracket) : base(
            SyntaxKind.DotSubgraph,
            subgraph?.Item1.Start ?? leftCurlyBracket.Start,
            (subgraph?.Item1?.FullWidth ?? 0) + (subgraph?.Item2?.FullWidth ?? 0) + (leftCurlyBracket?.FullWidth ?? 0) +
            (statementList?.FullWidth ?? 0) + (rightCurlyBracket?.FullWidth ?? 0),
            new SyntaxNode?[] {subgraph?.Item1, subgraph?.Item2, leftCurlyBracket, statementList, rightCurlyBracket})
        {
            if (leftCurlyBracket == null) throw new ArgumentNullException(nameof(leftCurlyBracket));
            if (statementList == null) throw new ArgumentNullException(nameof(statementList));
            if (rightCurlyBracket == null) throw new ArgumentNullException(nameof(rightCurlyBracket));
            Subgraph = subgraph;

            if (subgraph.HasValue)
                if (subgraph.Value.Item1 == null)
                    throw new ArgumentNullException(nameof(subgraph));

            LeftCurlyBracket = leftCurlyBracket;
            StatementList = statementList;
            RightCurlyBracket = rightCurlyBracket;
        }

        [MaybeNull] public (KeywordSyntax, DotIdSyntax)? Subgraph { get; }
        [NotNull] public PunctuationSyntax LeftCurlyBracket { get; }
        [NotNull] public DotStatementListSyntax StatementList { get; }
        [NotNull] public PunctuationSyntax RightCurlyBracket { get; }

        [return: NotNull]
        public override string ToString() =>
            $"{base.ToString()}, {nameof(Subgraph)}: {Subgraph}, {nameof(LeftCurlyBracket)}: {LeftCurlyBracket}, {nameof(StatementList)}: {StatementList}, {nameof(RightCurlyBracket)}: {RightCurlyBracket}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitSubgraph(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitSubgraph(this);
        }
    }
}
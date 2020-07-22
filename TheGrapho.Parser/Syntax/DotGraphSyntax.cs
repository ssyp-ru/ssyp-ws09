// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotGraphSyntax : DotSyntax
    {
        public DotGraphSyntax([AllowNull] KeywordSyntax? strict,
            [DisallowNull] KeywordSyntax graphOrDigraph,
            [AllowNull] DotIdSyntax? id,
            [DisallowNull] PunctuationSyntax leftCurlyBracket,
            [DisallowNull] DotStatementListSyntax statementList,
            [DisallowNull] PunctuationSyntax rightCurlyBracket) : base(
            SyntaxKind.DotGraph,
            strict?.Start ?? graphOrDigraph?.Start ?? 0,
            (strict?.FullWidth ?? 0) + (graphOrDigraph?.FullWidth ?? 0) + (id?.FullWidth ?? 0) +
            (leftCurlyBracket?.FullWidth ?? 0) +
            (statementList?.FullWidth ?? 0) +
            (rightCurlyBracket?.FullWidth ?? 0),
            new SyntaxNode?[] {strict, graphOrDigraph, id, leftCurlyBracket, statementList, rightCurlyBracket})
        {
            Strict = strict;
            GraphOrDigraph = graphOrDigraph ?? throw new ArgumentNullException(nameof(graphOrDigraph));
            Id = id;
            OpeningBracket = leftCurlyBracket ?? throw new ArgumentNullException(nameof(leftCurlyBracket));
            StatementList = statementList ?? throw new ArgumentNullException(nameof(statementList));
            ClosingBracket = rightCurlyBracket ?? throw new ArgumentNullException(nameof(rightCurlyBracket));
        }

        [MaybeNull] public KeywordSyntax? Strict { get; }
        [NotNull] public KeywordSyntax GraphOrDigraph { get; }
        [MaybeNull] public DotIdSyntax? Id { get; }
        [NotNull] public PunctuationSyntax OpeningBracket { get; }
        [NotNull] public DotStatementListSyntax StatementList { get; }
        [NotNull] public PunctuationSyntax ClosingBracket { get; }

        [return: NotNull]
        public override string ToString() =>
            $"{base.ToString()}, {nameof(Strict)}: {Strict}, {nameof(GraphOrDigraph)}: {GraphOrDigraph}, {nameof(Id)}: {Id}, {nameof(OpeningBracket)}: {OpeningBracket}, {nameof(StatementList)}: {StatementList}, {nameof(ClosingBracket)}: {ClosingBracket}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitGraph(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitGraph(this);
        }
    }
}
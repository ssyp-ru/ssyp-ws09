// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotStatementListSyntax : DotSyntax
    {
        public DotStatementListSyntax(
            [DisallowNull] IReadOnlyList<(DotStatementSyntax, PunctuationSyntax?)> statements) : base(
            SyntaxKind.DotStatementList,
            statements?.FirstOrDefault().Item1?.Start ?? 0,
            statements?.Sum(it => it.Item1?.FullWidth ?? 0 + it.Item2?.FullWidth ?? 0) ?? 0,
            statements?.SelectMany(it => new SyntaxNode?[] {it.Item1, it.Item2}).ToList())
        {
            Statements = statements ?? throw new ArgumentNullException(nameof(statements));

            if (statements.Any(valueTuple => valueTuple.Item1 == null))
                throw new ArgumentNullException(nameof(statements));
        }

        [NotNull] public IReadOnlyList<(DotStatementSyntax Statement, PunctuationSyntax? Comma)> Statements { get; }

        [return: NotNull]
        public override string ToString() =>
            $"{base.ToString()}, {nameof(Statements)}: [{string.Join(", ", Statements)}]";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitStatementList(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitStatementList(this);
        }
    }
}
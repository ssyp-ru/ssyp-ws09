// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotStatementSyntax : DotSyntax
    {
        public DotStatementSyntax([DisallowNull] DotSyntax statement) : base(
            SyntaxKind.DotStatement,
            statement?.Start ?? 0,
            statement?.FullWidth ?? 0,
            new[] {statement})
        {
            Statement = statement ?? throw new ArgumentNullException(nameof(statement));
        }

        [NotNull] public DotSyntax Statement { get; }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Statement)}: {Statement}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitStatement(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitStatement(this);
        }
    }
}
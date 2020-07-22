// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotAssignmentListSyntax : DotSyntax
    {
        public DotAssignmentListSyntax(
            [DisallowNull] IReadOnlyList<(DotAssignmentSyntax, PunctuationSyntax?)> assignments) :
            base(
                SyntaxKind.DotAssignmentList,
                assignments?.First().Item1?.Start ?? 0,
                assignments?.Sum(it => (it.Item1?.FullWidth ?? 0) + (it.Item2?.FullWidth ?? 0)) ?? 0,
                assignments?.SelectMany(it => new List<SyntaxNode?> {it.Item1, it.Item2}).ToList())
        {
            Assignments = assignments ?? throw new ArgumentNullException(nameof(assignments));
            if (assignments.Count == 0) throw new IndexOutOfRangeException(nameof(assignments));
        }

        [NotNull]
        public IReadOnlyList<(DotAssignmentSyntax Assignment, PunctuationSyntax? CommaOrSemicolon)> Assignments { get; }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Assignments)}: {Assignments}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitAssignmentList(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitAssignmentList(this);
        }
    }
}
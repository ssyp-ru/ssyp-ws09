// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotAttributeListSyntax : DotSyntax
    {
        public DotAttributeListSyntax(
            [DisallowNull] IReadOnlyList<(PunctuationSyntax, DotAssignmentListSyntax?, PunctuationSyntax)> attributes) :
            base(
                SyntaxKind.DotAttributeList,
                attributes?.First().Item1?.FullWidth ?? 0,
                attributes?.Sum(it =>
                    (it.Item1?.FullWidth ?? 0) + (it.Item2?.FullWidth ?? 0) + (it.Item3?.FullWidth ?? 0)) ?? 0,
                attributes?.SelectMany(it => new SyntaxNode?[] {it.Item1, it.Item2, it.Item3}).ToList())
        {
            Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
            if (attributes.Count == 0) throw new IndexOutOfRangeException(nameof(attributes));

            foreach (var (leftBracket, _, rightBracket) in attributes)
            {
                if (leftBracket == null) throw new ArgumentNullException(nameof(attributes));
                if (rightBracket == null) throw new ArgumentNullException(nameof(attributes));
            }
        }

        [NotNull]
        public IReadOnlyList<(PunctuationSyntax LeftSquareBracket, DotAssignmentListSyntax? AssignmentList,
            PunctuationSyntax RightSquareBracket)> Attributes { get; }

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitAttributeList(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitAttributeList(this);
        }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Attributes)}: {Attributes}";
    }
}
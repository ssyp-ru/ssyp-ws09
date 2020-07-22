// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotPortSyntax : DotSyntax
    {
        public DotPortSyntax([DisallowNull] PunctuationSyntax colon,
            [DisallowNull] DotIdSyntax id,
            [AllowNull] (PunctuationSyntax, DotIdSyntax)? colonAndCompassPt) : base(
            SyntaxKind.DotPort,
            colon?.Start ?? 0,
            (colon?.FullWidth ?? 0) + (id?.FullWidth ?? 0) + (colonAndCompassPt?.Item1?.FullWidth ?? 0) +
            (colonAndCompassPt?.Item2?.FullWidth ?? 0),
            new SyntaxNode?[] {colon, id, colonAndCompassPt?.Item1, colonAndCompassPt?.Item2})
        {
            Colon = colon ?? throw new ArgumentNullException(nameof(colon));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ColonAndCompassPt = colonAndCompassPt;

            if (!colonAndCompassPt.HasValue) return;
            if (colonAndCompassPt.Value.Item1 == null) throw new ArgumentNullException(nameof(colonAndCompassPt));
            if (colonAndCompassPt.Value.Item2 == null) throw new ArgumentNullException(nameof(colonAndCompassPt));
        }

        [NotNull] public PunctuationSyntax Colon { get; }
        [NotNull] public DotIdSyntax Id { get; }
        [MaybeNull] public (PunctuationSyntax Colon, DotIdSyntax CompassPt)? ColonAndCompassPt { get; }

        [return: NotNull]
        public override string ToString() =>
            $"{base.ToString()}, {nameof(Colon)}: {Colon}, {nameof(Id)}: {Id}, {nameof(ColonAndCompassPt)}: {ColonAndCompassPt}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitPort(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitPort(this);
        }
    }
}
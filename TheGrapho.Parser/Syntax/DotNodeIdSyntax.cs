// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public sealed class DotNodeIdSyntax : DotSyntax
    {
        public DotNodeIdSyntax([DisallowNull] DotIdSyntax id, [AllowNull] DotPortSyntax? port) : base(
            SyntaxKind.DotNodeId,
            id?.Start ?? 0,
            (id?.FullWidth ?? 0) + (port?.FullWidth ?? 0),
            new SyntaxNode?[] {id, port})
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Port = port;
        }

        [NotNull] public DotIdSyntax Id { get; }
        [MaybeNull] public DotPortSyntax? Port { get; }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Id)}: {Id}, {nameof(Port)}: {Port}";

        [return: MaybeNull]
        public override TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            return syntaxVisitor.VisitNodeId(this);
        }

        public override void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor)
        {
            if (syntaxVisitor == null) throw new ArgumentNullException(nameof(syntaxVisitor));
            syntaxVisitor.VisitNodeId(this);
        }
    }
}
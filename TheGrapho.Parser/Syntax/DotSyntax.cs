// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TheGrapho.Parser.Utilities;

namespace TheGrapho.Parser.Syntax
{
    public abstract class DotSyntax : SyntaxNode
    {
        protected DotSyntax(SyntaxKind kind, int start, int fullWidth,
            [AllowNull] IReadOnlyList<SyntaxNode?>? children) :
            base(kind, start, fullWidth)
        {
            children ??= new SyntaxNode[] { };
            Children = children.Where(it => it != null).ToList()!;
            foreach (var it in Children) it.Parent = this;
        }

        public override bool IsTrivia => false;

        [NotNull] public IReadOnlyCollection<SyntaxNode> Children { get; }

        public override void Write([DisallowNull] StringBuilder target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            Children.WriteAll(target);
        }

        [return: MaybeNull]
        public abstract TResult Accept<TResult>([DisallowNull] DotSyntaxVisitor<TResult> syntaxVisitor);

        public abstract void Accept([DisallowNull] DotSyntaxVisitor syntaxVisitor);
    }
}
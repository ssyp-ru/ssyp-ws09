// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using TheGrapho.Parser.Utilities;

namespace TheGrapho.Parser.Syntax
{
    public abstract class SyntaxNode
    {
        protected SyntaxNode(SyntaxKind kind, int start, int fullWidth)
        {
            Kind = kind;
            Parent = null;
            Start = start;
            FullWidth = fullWidth;
        }

        public SyntaxKind Kind { get; }
        [MaybeNull] public SyntaxNode? Parent { get; internal set; }
        internal int FullWidth { get; set; }
        internal int Start { get; set; }
        public TextSpan FullSpan => new TextSpan(Start, FullWidth);

        public abstract bool IsTrivia { get; }

        public abstract void Write([DisallowNull] StringBuilder target);

        [return: NotNull]
        public override string ToString() =>
            $"{nameof(Kind)}: {Kind}";
    }
}
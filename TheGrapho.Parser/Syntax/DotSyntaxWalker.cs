// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using TheGrapho.Parser.Utilities;

namespace TheGrapho.Parser.Syntax
{
    public abstract class DotSyntaxWalker : DotSyntaxVisitor
    {
        public enum SyntaxWalkerDepth : byte
        {
            Syntax = 0,
            Token = 1,
            Trivia = 2
        }

        public virtual SyntaxWalkerDepth Depth => SyntaxWalkerDepth.Syntax;

        public int RecursionDepth { get; set; }

        public new void Visit([DisallowNull] DotSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            RecursionDepth++;
            StackGuard.EnsureSufficientExecutionStack(RecursionDepth);
            syntax.Accept(this);
            RecursionDepth--;
        }

        protected override void DefaultVisit([DisallowNull] DotSyntax syntax)
        {
            foreach (var child in syntax.Children)
                switch (child)
                {
                    case DotSyntax childSyntax when Depth >= SyntaxWalkerDepth.Syntax:
                        Visit(childSyntax);
                        break;
                    case SyntaxToken childToken when Depth >= SyntaxWalkerDepth.Token:
                        VisitToken(childToken);
                        break;
                }
        }

        public virtual void VisitToken([DisallowNull] SyntaxToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (Depth < SyntaxWalkerDepth.Trivia) return;
            VisitLeadingTrivia(token);
            VisitTrailingTrivia(token);
        }

        public virtual void VisitLeadingTrivia([DisallowNull] SyntaxToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (token.LeadingTrivia.Count == 0) return;
            foreach (var tr in token.LeadingTrivia) VisitTrivia(tr);
        }

        public virtual void VisitTrailingTrivia([DisallowNull] SyntaxToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (token.TrailingTrivia.Count == 0) return;
            foreach (var tr in token.TrailingTrivia) VisitTrivia(tr);
        }

        public virtual void VisitTrivia([DisallowNull] SyntaxTrivia trivia)
        {
            if (trivia == null) throw new ArgumentNullException(nameof(trivia));
        }
    }
}
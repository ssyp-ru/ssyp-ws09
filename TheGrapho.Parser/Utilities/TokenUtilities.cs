// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TheGrapho.Parser.Syntax;

namespace TheGrapho.Parser.Utilities
{
    public static class TokenUtilities
    {
        private static void RecomputeSize([DisallowNull] this SyntaxToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (token.LeadingTrivia.Count >= 1) token.Start = token.LeadingTrivia.First().Start;

            token.FullWidth = token.TrailingTrivia.Sum(it => it.FullWidth) +
                              token.LeadingTrivia.Sum(it => it.FullWidth) + token.FullWidth;
        }

        [return: NotNull]
        public static IEnumerable<SyntaxToken> MergeTriviaToTokens([DisallowNull] this IEnumerable<SyntaxNode> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var syntaxNodes = source as SyntaxNode[] ?? source.ToArray();
            var tokens = new List<SyntaxToken>(syntaxNodes.Count(it => it is SyntaxToken));
            SyntaxToken? currentToken = null;
            var allTrivia = new List<SyntaxTrivia>();

            foreach (var syntaxNode in syntaxNodes)
                switch (syntaxNode)
                {
                    case SyntaxToken token when currentToken == null:
                        token.LeadingTrivia = allTrivia;
                        allTrivia = new List<SyntaxTrivia>();
                        currentToken = token;
                        currentToken.TrailingTrivia = allTrivia;
                        tokens.Add(currentToken);
                        continue;
                    case SyntaxToken token2:
                        allTrivia = new List<SyntaxTrivia>();
                        token2.TrailingTrivia = allTrivia;
                        currentToken = token2;
                        tokens.Add(currentToken);
                        continue;
                    case SyntaxTrivia trivia2:
                        allTrivia.Add(trivia2);
                        continue;
                }

            tokens.ForEach(RecomputeSize);
            return tokens;
        }
    }
}
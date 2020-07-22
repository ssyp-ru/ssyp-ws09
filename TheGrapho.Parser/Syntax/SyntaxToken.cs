// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TheGrapho.Parser.Utilities;

namespace TheGrapho.Parser.Syntax
{
    public abstract class SyntaxToken : SyntaxNode
    {
        protected SyntaxToken(SyntaxKind kind, int start, int fullWidth) : base(kind, start, fullWidth)
        {
        }

        public override bool IsTrivia => false;

        public TextSpan Span
        {
            get
            {
                var start = Start;
                var width = FullWidth;
                var precedingWidth = LeadingTrivia.Sum(it => it.FullWidth);
                start += precedingWidth;
                width -= precedingWidth;
                width -= TrailingTrivia.Sum(it => it.FullWidth);
                return new TextSpan(start, width);
            }
        }

        [NotNull]
        public IReadOnlyList<SyntaxTrivia> TrailingTrivia { get; internal set; } = Array.Empty<SyntaxTrivia>();

        [NotNull] public IReadOnlyList<SyntaxTrivia> LeadingTrivia { get; internal set; } = Array.Empty<SyntaxTrivia>();

        [return: NotNull]
        public override string ToString()
        {
            var properties = new List<object> {$"{base.ToString()}"};
            
            if (TrailingTrivia.Count != 0)
                properties.Add($"{nameof(TrailingTrivia)}: [{string.Join(", ", TrailingTrivia)}]");
            
            if (LeadingTrivia.Count != 0)
                properties.Add($"{nameof(LeadingTrivia)}: [{string.Join(", ", LeadingTrivia)}]");

            return string.Join(", ", properties);
        }
    }
}
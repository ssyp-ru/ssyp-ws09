// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TheGrapho.Parser.Syntax
{
    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    public sealed class SyntaxTrivia : SyntaxNode
    {
        public SyntaxTrivia(
            SyntaxKind kind,
            int start,
            int fullWidth,
            [DisallowNull] string value) : base(
            kind, start, fullWidth)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull] public string Value { get; }

        public override bool IsTrivia => true;

        public override void Write([DisallowNull] StringBuilder target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            switch (Kind)
            {
                case SyntaxKind.PreprocessorTrivia:
                    target.Append('#');
                    target.Append(Value);
                    return;
                case SyntaxKind.WhitespaceTrivia:
                    target.Append(Value);
                    return;
                case SyntaxKind.BlockCommentTrivia:
                    target.Append("/*");
                    target.Append(Value);
                    target.Append("*/");
                    return;
                case SyntaxKind.LineCommentTrivia:
                    target.Append("//");
                    target.Append(Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Value)}: {Value}";
    }
}
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TheGrapho.Parser.Utilities;

namespace TheGrapho.Parser.Syntax
{
    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    public sealed class StringSyntax : SyntaxToken
    {
        public StringSyntax(SyntaxKind kind, int start, int fullWidth,
            [NotNull] string value) : base(
            kind, start, fullWidth)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull] public string Value { get; }

        public override void Write([DisallowNull] StringBuilder target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            LeadingTrivia.WriteAll(target);

            switch (Kind)
            {
                case SyntaxKind.NumberToken:
                    target.Append(Value);
                    break;
                case SyntaxKind.StringToken:
                    target.Append('"');
                    target.Append(Value.Replace("\"", "\\\""));
                    target.Append('"');
                    break;
                case SyntaxKind.IdToken:
                    target.Append(Value);
                    break;
                case SyntaxKind.HtmlStringToken:
                    target.Append('<');
                    target.Append(Value);
                    target.Append('>');
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            TrailingTrivia.WriteAll(target);
        }

        [return: NotNull]
        public override string ToString() => $"{base.ToString()}, {nameof(Value)}: {Value}";
    }
}
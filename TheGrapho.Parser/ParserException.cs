// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TheGrapho.Parser.Syntax;

namespace TheGrapho.Parser
{
    public class ParserException : Exception
    {
        [NotNull] public IReadOnlyCollection<(string Message, SyntaxToken? Token)> Errors { get; }

        public ParserException([DisallowNull] IReadOnlyCollection<(string Message, SyntaxToken? Token)> errors) : base(
            $"[{string.Join("; ", errors.Select(it => it.Message))}]")
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }
}
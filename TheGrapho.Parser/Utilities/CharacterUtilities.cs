// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TheGrapho.Parser.Utilities
{
    internal static class CharacterUtilities
    {
        [NotNull] private static readonly char[] WhitespaceChars = {' ', '\n', '\r', '\t'};
        public static bool IsWhitespace(char c) => WhitespaceChars.Contains(c);
        public static bool IsDigitLetterOrUnderscore(char c) => char.IsLetterOrDigit(c) || c == '_';
    }
}
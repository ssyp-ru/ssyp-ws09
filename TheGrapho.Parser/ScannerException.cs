// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser
{
    public sealed class ScannerException : Exception
    {
        public ScannerException(int offset, [AllowNull] string? message) : base($"{offset} - {message}")
        {
        }
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the README.md file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TheGrapho.Parser.Utilities
{
    internal static class StackGuard
    {
        public const int MaxUncheckedRecursionDepth = 20;

        [DebuggerStepThrough]
        public static void EnsureSufficientExecutionStack(int recursionDepth)
        {
            if (recursionDepth > MaxUncheckedRecursionDepth) RuntimeHelpers.EnsureSufficientExecutionStack();
        }
    }
}
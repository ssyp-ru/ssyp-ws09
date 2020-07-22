// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using TheGrapho.Parser.Syntax;

namespace TheGrapho.Parser.SimpleModel
{
    public static class SimpleModel
    {
        public static Graph ConvertToSimpleModel([NotNull] this DotGraphSyntax source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new SimpleModelBuilder().Build(source);
        }
    }
}
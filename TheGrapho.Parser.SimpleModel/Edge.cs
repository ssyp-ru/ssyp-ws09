// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.SimpleModel
{
    public readonly struct Edge
    {
        public Edge(Node source, Node target, bool isDirect)
        {
            Source = source;
            Target = target;
            IsDirect = isDirect;
        }

        public Edge(Node source, Node target)
        {
            Source = source;
            Target = target;
            IsDirect = false;
        }

        public bool Equals(Edge other) => Source.Equals(other.Source) && Target.Equals(other.Target);

        public override bool Equals([AllowNull] object? obj) => obj is Edge other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Source, Target);

        [return: NotNull]
        public override string ToString() => $"{nameof(Source)}: {Source}, {nameof(Target)}: {Target}";

        public Node Source { get; }
        public Node Target { get; }
        public bool IsDirect { get; }
    }
}
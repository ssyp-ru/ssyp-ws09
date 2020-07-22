// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TheGrapho.Parser.SimpleModel
{
    public readonly struct Graph
    {
        public bool Equals(Graph other) => Nodes.SequenceEqual(other.Nodes) && Edges.SequenceEqual(other.Edges);

        public override bool Equals([AllowNull] object? obj) => obj is Graph other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Nodes, Edges);

        [return: NotNull]
        public override string ToString() =>
            $"{nameof(Nodes)}: [{string.Join("; ", Nodes)}], {nameof(Edges)}: [{string.Join("; ", Edges)}]";

        public Graph([DisallowNull] ICollection<Node> nodes, [DisallowNull] ICollection<Edge> edges)
        {
            Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
            Edges = edges ?? throw new ArgumentNullException(nameof(edges));
        }

        [NotNull] public ICollection<Node> Nodes { get; }
        [NotNull] public ICollection<Edge> Edges { get; }
    }
}
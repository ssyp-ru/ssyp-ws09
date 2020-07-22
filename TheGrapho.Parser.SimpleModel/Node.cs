// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.SimpleModel
{
    public readonly struct Node
    {
        public Node([DisallowNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public bool Equals(Node other) => Name == other.Name;

        [return: NotNull]
        public override string ToString() => $"{nameof(Name)}: {Name}";

        public override bool Equals([AllowNull] object? obj) => obj is Node other && Equals(other);

        public override int GetHashCode() => Name.GetHashCode();

        [NotNull] public string Name { get; }
    }
}
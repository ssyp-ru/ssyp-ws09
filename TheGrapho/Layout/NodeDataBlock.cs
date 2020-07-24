// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TheGrapho.Layout
{
    internal class NodeDataBlock : IDisposable
    {
        public NodeDataBlock(int index, Node control)
        {
            Index = index;
            Control = control ?? throw new ArgumentNullException(nameof(control));
        }

        public int Index { get; }
        public Node Control { get; private set; }

        public IReadOnlyList<int> IncomingEdgeIndexes => _incomingEdges;

        public IReadOnlyList<int> OutgoingEdgeIndexes => _outgoingEdges;

        public void Assign(LayoutEngine layout)
        {
            _incomingEdges ??= layout.Edges.Where(x => x.TargetIndex == Index).Select(x => x.Index).ToArray();
            _outgoingEdges ??= layout.Edges.Where(x => x.SourceIndex == Index).Select(x => x.Index).ToArray();
        }

        public Point? Position { get; set; }

        public (int Width, int Height) Size => _size ??= ((int) Math.Ceiling(Control.Size.Width),
            (int) Math.Ceiling(Control.Size.Height));

        private int[] _incomingEdges;
        private int[] _outgoingEdges;
        private (int Width, int Height)? _size;

        public void Dispose()
        {
            Control = null;
        }
    }
}
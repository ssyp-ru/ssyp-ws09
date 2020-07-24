// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Linq;
using System.Windows;

namespace TheGrapho.Layout
{
    internal class EdgeDataBlock : IDisposable
    {
        private int? _sourceIndex;
        private int? _targetIndex;

        public EdgeDataBlock(int index, Edge control)
        {
            Index = index;
            Control = control ?? throw new ArgumentNullException(nameof(control));
        }

        public int Index { get; }
        public Edge Control { get; private set; }

        public int SourceIndex => _sourceIndex.Value;
        public int TargetIndex => _targetIndex.Value;

        public void Assign(LayoutEngine layout)
        {
            _sourceIndex ??= layout.Nodes.Single(x => x.Control == Control.Source).Index;
            _targetIndex ??= layout.Nodes.Single(x => x.Control == Control.Target).Index;
        }

        public Point[] Points { get; set; }

        public void Dispose()
        {
            Control = null;
        }
    }
}
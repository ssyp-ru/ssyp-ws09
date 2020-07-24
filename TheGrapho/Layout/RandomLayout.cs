// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Windows;

namespace TheGrapho.Layout
{
    internal sealed class RandomLayout : IGraphLayout
    {
        public Rect Bounds { get; }
        public int Seed { get; }

        public RandomLayout(Rect bounds, int seed)
        {
            Bounds = bounds;
            Seed = seed;
        }

        public void Execute(LayoutEngine target)
        {
            var boundsWidth = (int) Bounds.Width;
            var boundsHeight = (int) Bounds.Height;
            var seed = Seed;
            var rnd = new Random(seed);

            foreach (var item in target.Nodes)
            {
                if (item.Control.HasValidLayout)
                    continue;

                var x = (int) Bounds.X;
                var y = (int) Bounds.Y;
                item.Position = new Point(rnd.Next(x, x + boundsWidth - 45), rnd.Next(y, y + boundsHeight - 45));
            }
        }
    }
}
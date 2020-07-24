// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Linq;
using System.Windows;

namespace TheGrapho.Layout
{
    internal sealed class CircleLayout : IGraphLayout
    {
        public void Execute(LayoutEngine target)
        {
            var perimeter = 0.0;
            var usableNodes = target.Nodes;
            var halfSize = new double[usableNodes.Length];
            var i = 0;

            foreach (var s in usableNodes.Select(v => new Size(45.0, 45.0)))
            {
                halfSize[i] = Math.Sqrt(s.Width * s.Width + s.Height * s.Height) * 0.5;
                perimeter += halfSize[i] * 2;
                i++;
            }

            var radius = perimeter / (2 * Math.PI);
            var angle = 0.0;
            double a;
            i = 0;

            foreach (var v in usableNodes)
            {
                a = Math.Sin(halfSize[i] * 0.5 / radius) * 2;
                angle += a;
                v.Position = new Point(Math.Cos(angle) * radius + radius, Math.Sin(angle) * radius + radius);
                angle += a;
            }

            radius = angle / (2 * Math.PI) * radius;
            angle = 0;
            i = 0;

            foreach (var v in usableNodes)
            {
                a = Math.Sin(halfSize[i] * 0.5 / radius) * 2;
                angle += a;
                v.Position =
                    new Point(Math.Cos(angle) * radius + radius, Math.Sin(angle) * radius + radius);
                angle += a;
            }
        }
    }
}
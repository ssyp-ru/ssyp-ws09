// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Immutable;

namespace TheGrapho.Layout
{
    class LayoutEngine
    {
        public LayoutEngine(NewItemsControl graphControl, IGraphLayout graphLayout)
        {
            if (graphControl == null) throw new ArgumentNullException(nameof(graphControl));

            GraphLayout = graphLayout;

            var count = graphControl.Items.Count;
            var nodeBuilder = ImmutableArray.CreateBuilder<NodeDataBlock>(count);
            var edgeBuilder = ImmutableArray.CreateBuilder<EdgeDataBlock>(count);

            foreach (var item in graphControl.Items)
            {
                switch (item)
                {
                    case Node node:
                        nodeBuilder.Add(CreateInstance<NodeDataBlock>(nodeBuilder.Count, node));
                        break;
                    case Edge edge:
                        edgeBuilder.Add(CreateInstance<EdgeDataBlock>(edgeBuilder.Count, edge));
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            Nodes = nodeBuilder.Capacity == nodeBuilder.Count
                ? nodeBuilder.MoveToImmutable()
                : nodeBuilder.ToImmutable();
            Edges = edgeBuilder.Capacity == edgeBuilder.Count
                ? edgeBuilder.MoveToImmutable()
                : edgeBuilder.ToImmutable();

            foreach (var edge in Edges) edge.Assign(this);
            foreach (var node in Nodes) node.Assign(this);
        }

        private static T CreateInstance<T>(params object[] paramArray)
        {
            return (T) Activator.CreateInstance(typeof(T), paramArray);
        }

        public ImmutableArray<NodeDataBlock> Nodes { get; }
        public ImmutableArray<EdgeDataBlock> Edges { get; }

        public IGraphLayout GraphLayout { get; }

        public void Layout()
        {
            GraphLayout.Execute(this);

            foreach (var node in Nodes)
            {
                if (!(node.Position is { } position))
                    continue;

                node.Control.X = position.X;
                node.Control.Y = position.Y;
                node.Control.HasValidLayout = true;
            }

            foreach (var edge in Edges)
            {
                // edge.Control.Points = edge.Points;
                edge.Control.HasValidLayout = true;
            }
        }
    }
}
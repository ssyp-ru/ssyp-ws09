// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TheGrapho.Parser.Syntax;

namespace TheGrapho.Parser.SimpleModel
{
    internal static class DictionaryExtensions
    {
        public delegate T Supplier<out T>();

        public static TValue GetOrPut<TKey, TValue>([DisallowNull] this IDictionary<TKey, TValue> receiver,
            [NotNull] TKey key,
            [NotNull] Supplier<TValue> defaultValue)
            where TKey : notnull
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (defaultValue == null) throw new ArgumentNullException(nameof(defaultValue));
            if (receiver.ContainsKey(key)) return receiver[key];
            receiver[key] = defaultValue();
            return receiver[key];
        }
    }

    internal class SimpleModelBuilder
    {
        [NotNull] public ICollection<string> Nodes { get; } = new HashSet<string>();

        [NotNull] public IList<(string, string)> Edges { get; } = new List<(string, string)>();

        private void Add([DisallowNull] DotStatementListSyntax statements, [DisallowNull] out HashSet<string> newNodes)
        {
            newNodes = new HashSet<string>();

            foreach (var statement in statements.Statements.Select(it => it.Statement.Statement))
                switch (statement)
                {
                    case DotSubgraphSyntax subgraph:
                        Add(subgraph.StatementList, out _);
                        break;
                    case DotNodeStatementSyntax node:
                        Nodes.Add(node.NodeId.Id.Value.Value);
                        newNodes.Add(node.NodeId.Id.Value.Value);
                        break;
                    case DotEdgeStatementSyntax edge:
                        var nodesOrSubgraphsSequence =
                            new[] {edge.NodeIdOrSubgraph}.Union(edge.EdgeRhs.Edges.Select(it => it.NodeIdOrSubgraph));

                        var sequence = new LinkedList<IEnumerable<string>>();

                        foreach (var syntax in nodesOrSubgraphsSequence)
                            switch (syntax)
                            {
                                case DotNodeIdSyntax node:
                                    Nodes.Add(node.Id.Value.Value);
                                    newNodes.Add(node.Id.Value.Value);
                                    sequence.AddLast(new[] {node.Id.Value.Value});
                                    break;
                                case DotSubgraphSyntax subgraph:
                                    Add(subgraph.StatementList, out var newNodes1);
                                    foreach (var newNode in newNodes1) Nodes.Add(newNode);
                                    sequence.AddLast(newNodes1);
                                    break;
                            }

                        for (LinkedListNode<IEnumerable<string>>? a = sequence.First, b = a?.Next;
                            a != null && b != null;
                            a = a.Next, b = b.Next)
                            foreach (var s in a.Value)
                            foreach (var s1 in b.Value)
                                Edges.Add((s, s1));

                        break;
                }
        }

        [return: NotNull]
        public Graph Build([DisallowNull] DotGraphSyntax graph)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            Add(graph.StatementList, out _);

            return new Graph(Nodes.Select(it => new Node(it)).ToList(),
                Edges.Select(it => new Edge(new Node(it.Item1), new Node(it.Item2))).ToHashSet());
        }
    }
}
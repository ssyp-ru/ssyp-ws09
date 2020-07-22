// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TheGrapho.Parser.SimpleModel.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var tree = new Parser(new Scanner("graph { a; b; {a} -- {b} }").ScanTillEnd().ToList())
                .Parse()
                .ConvertToSimpleModel();

            Console.WriteLine(tree);

            Assert.AreEqual(
                new Graph(
                    new HashSet<Node> {new Node("a"), new Node("b")},
                    new HashSet<Edge> {new Edge(new Node("a"), new Node("b"))}),
                tree);
        }
    }
}
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheGrapho
{
    public class Node : BaseItem
    {
        //public List<Edge> Edges = new List<Edge>();
        static int id = 0;
        public static readonly DependencyProperty NameProperty;
        public Size Size { get; set; }
        string Name { get { return GetValue(NameProperty).ToString(); } set { SetValue(NameProperty, value); } }

        public Node(string Name) 
        {
            this.Name = Name;
            id++;
            this.ZIndex = 1;
        }
        public Node(Point position) : this($"Node{id}") 
        {
            X = position.X;
            Y = position.Y;
        }
        static Node()
        {
            NameProperty = DependencyProperty.Register(
                "Name",
                typeof(string),
                typeof(Node)
                );
        }
    }
}

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
        public static readonly DependencyProperty AreDependentRoutesValidProperty = DependencyProperty.Register(
            nameof(AreDependentRoutesValid), typeof(bool),
            typeof(Node), new PropertyMetadata(false)
        );

        static int id = 0;
        public static int Selected = 0;
        public static readonly DependencyProperty NameProperty;
        private Size _size;
        private bool _isAddedManually;

        public Size Size
        {
            get => _size;
            set
            {
                if (_size == value)
                    return;

                _size = value;

                if (CanSizeChangeInvalidateLayout)
                    HasValidLayout = false;
                else
                    AreDependentRoutesValid = false;
            }
        }

        public string Name { get { return GetValue(NameProperty).ToString(); } set { SetValue(NameProperty, value); } }

        public bool AreDependentRoutesValid
        {
            get => (bool)GetValue(AreDependentRoutesValidProperty);
            set => SetValue(AreDependentRoutesValidProperty, value);
        }

        public Node(string Name) 
        {
            this.Name = Name;
            id++;
            this.ZIndex = 1;
        }

        private bool CanSizeChangeInvalidateLayout => !_isAddedManually;

        protected override void OnPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            AreDependentRoutesValid = false;
            _isAddedManually = false;
        }

        public Node(Point position) : this($"Node{id}")
        {
            X = position.X;
            Y = position.Y;
            _isAddedManually = true;
            HasValidLayout = true;
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

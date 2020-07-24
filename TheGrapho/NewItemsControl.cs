// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Net;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using TheGrapho.Layout;

namespace TheGrapho
{
    public class NewItemsControl : ItemsControl
    {
        public static readonly DependencyProperty IsLayoutValidProperty = DependencyProperty.Register(
            nameof(IsLayoutValid), typeof(bool),
            typeof(NewItemsControl), new PropertyMetadata(true, OnLayoutValidChanged)
        );

        public bool IsLayoutValid
        {
            get => (bool) GetValue(IsLayoutValidProperty);
            set => SetValue(IsLayoutValidProperty, value);
        }

        public static readonly DependencyProperty AreEdgeRoutesValidProperty = DependencyProperty.Register(
            nameof(AreEdgeRoutesValid), typeof(bool),
            typeof(NewItemsControl), new PropertyMetadata(true, OnEdgeRoutesValidChanged)
        );

        public bool AreEdgeRoutesValid
        {
            get => (bool)GetValue(AreEdgeRoutesValidProperty);
            set => SetValue(AreEdgeRoutesValidProperty, value);
        }

        private static readonly ChangeValidationPropertyConverter ChangeValidationPropertyConverterValue = new ChangeValidationPropertyConverter();

        public Point? SelectionStartingPoint;
        public NewItemsControl() : base()
        {
            ((INotifyCollectionChanged)Items).CollectionChanged += CreateMultiBinding;
            PreviewMouseDown += StartOfSelection;
            PreviewMouseUp += EndofSelection;
            PreviewMouseMove += Selecting;
            MouseLeave += ((a, b) => StopSelection());
        }

        private void CreateMultiBinding(object sender, NotifyCollectionChangedEventArgs e)
        {
            var layoutValidBinding = new MultiBinding
            {
                Mode = BindingMode.OneWay,
                FallbackValue = true,
                Converter = ChangeValidationPropertyConverterValue,
            };

            var routesValidBinding = new MultiBinding
            {
                Mode = BindingMode.OneWay,
                FallbackValue = true,
                Converter = ChangeValidationPropertyConverterValue,
            };

            foreach (var item in Items)
            {
                layoutValidBinding.Bindings.Add(
                    new Binding
                    {
                        Path = new PropertyPath(BaseItem.HasValidLayoutProperty),
                        TargetNullValue = false,
                        FallbackValue = false,
                        Source = item
                    }
                );

                if (item is Node)
                {
                    routesValidBinding.Bindings.Add(
                        new Binding
                        {
                            Path = new PropertyPath(Node.AreDependentRoutesValidProperty),
                            TargetNullValue = false,
                            FallbackValue = false,
                            Source = item
                        }
                    );
                }
            }

            BindingOperations.SetBinding(this, IsLayoutValidProperty, layoutValidBinding);
            BindingOperations.SetBinding(this, AreEdgeRoutesValidProperty, routesValidBinding);
        }

        private class ChangeValidationPropertyConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                return values.Cast<bool>().All(e => e);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }

        private static void OnLayoutValidChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as NewItemsControl)?.UpdateNodes();
        }

        private static void OnEdgeRoutesValidChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as NewItemsControl)?.UpdateEdges();
        }

        private bool _isLayoutInProgress;
        private bool _hasNodesLayoutInProgressBeenHit;
        private bool _hasEdgesLayoutInProgressBeenHit;

        public enum LayoutAlgorithm
        {
            Random,
            Circular,
        }

        public LayoutAlgorithm Algorithm { get; set; } = LayoutAlgorithm.Random;

        private void RunRandom()
        {
            var bounds = new Rect(new Point(20, 20), new Size(ActualWidth, ActualHeight));
            var next = new Random().Next();
            new LayoutEngine(this, new RandomLayout(bounds, next)).Layout();
        }
        private void RunCircular()
        {
            new LayoutEngine(this, new CircleLayout()).Layout();
        }

        private void RunLayout()
        {
            switch (Algorithm)
            {
                case LayoutAlgorithm.Random:
                    RunRandom();
                    break;
                case LayoutAlgorithm.Circular:
                    RunCircular();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UpdateNodes()
        {
            if (_isLayoutInProgress)
            {
                _hasNodesLayoutInProgressBeenHit = true;
                return;
            }

            _isLayoutInProgress = true;
            _hasNodesLayoutInProgressBeenHit = false;

            RunLayout();

            _isLayoutInProgress = false;

            UpdateEdges();
        }

        public void UpdateEdges()
        {
            if (_isLayoutInProgress)
            {
                _hasEdgesLayoutInProgressBeenHit = true;
                return;
            }

            _isLayoutInProgress = true;
            _hasEdgesLayoutInProgressBeenHit = false;

            foreach (var nodeItem in Items.OfType<Node>())
            {
                if (nodeItem.AreDependentRoutesValid)
                    continue;

                foreach (var edge in Items.OfType<Edge>())
                    if (edge.Source == nodeItem || edge.Target == nodeItem)
                        edge.HasValidLayout = false;

                nodeItem.AreDependentRoutesValid = true;
            }

            foreach (var edge in Items.OfType<Edge>())
            {
                if (edge.HasValidLayout)
                    continue;

                edge.DrawLine();
                edge.HasValidLayout = true;
            }

            _isLayoutInProgress = false;

            if (_hasEdgesLayoutInProgressBeenHit)
                UpdateEdges();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var container = base.GetContainerForItemOverride();

            NewCanvasPanel.RegisterForInvalidations(container);

            return container;
        }
        public IEnumerable<BaseItem> GetSelectedItems()
        {
            return Items.Cast<BaseItem>().Where(e => e.PositionOfSelection != null);
        }
        public IEnumerable<BaseItem> GetSelectedEdges()
        {
            return Items.Cast<BaseItem>().Where(e => e.PositionOfSelection == -1);
        }
        public IEnumerable<BaseItem> GetSelectedNodes()
        {
            return Items.Cast<BaseItem>().Where(e => e.PositionOfSelection > -1);
        }
        public void RemoveSelection()
        {
            Node.Selected = 0;
            foreach(var item in Items)
            {
                ((BaseItem)item).PositionOfSelection = null;
                ((BaseItem)item).Deselect();
            }
        }
        public void DeleteSelectedItems()
        {
            var window = Window.GetWindow(this) as MainWindow ?? throw new ArgumentNullException();
            foreach(var item in GetSelectedItems().ToArray())
            {
                window.Items.Remove(item);
            }
            foreach(var item in Items.Cast<BaseItem>().ToArray())
            {
                if(item is Edge)
                {
                    var temp = item as Edge;
                    if (temp.Source.PositionOfSelection != null || temp.Target.PositionOfSelection != null)
                    {
                        window.Items.Remove(temp);
                    }
                }
            }
            RemoveSelection();
        }
        public void AddEdgesAsChain()
        {
            var selected = GetSelectedNodes().OrderBy(e => e.PositionOfSelection).ToArray();
            if(selected.Length > 1)
            {
                var window = Window.GetWindow(this) as MainWindow ?? throw new ArgumentNullException();
                for(var i = 1; i < selected.Count(); i++)
                {
                    window.Items.Add(new Edge(selected[i - 1] as Node, selected[i] as Node, false));
                }
            }
            RemoveSelection();
        }
        public void AddEdgesAsMason()
        {
            var selected = GetSelectedNodes().OrderBy(e => e.PositionOfSelection).ToArray();
            if(selected.Length > 1)
            {
                if (selected.Length > 2 && selected.First().PositionOfSelection == int.MaxValue)
                    MessageBox.Show("Choose Start Node", "Warning!");
                else 
                {
                    var first = selected.First() as Node;
                    var window = Window.GetWindow(this) as MainWindow ?? throw new ArgumentNullException();
                    for (var i = 1; i < selected.Count(); i++)
                    {
                        window.Items.Add(new Edge(first, selected[i] as Node, false));
                    }
                    RemoveSelection();
                }
            }
        }
        private void StartOfSelection(object sender, MouseButtonEventArgs e)
        {
            SelectionStartingPoint = e.GetPosition(this);
        }
        private void Selecting(object sender, MouseEventArgs e)
        {
            if (SelectionStartingPoint != null)
            {
                RectangleGeometry temp = Template.FindName("Selection", this) as RectangleGeometry;
                temp.Rect = new Rect(e.GetPosition(this), (Point)SelectionStartingPoint);
            }
        }
        private void StopSelection()
        {
            RectangleGeometry temp = Template.FindName("Selection", this) as RectangleGeometry;
            temp.Rect = new Rect();
            SelectionStartingPoint = null;
        }

        private void EndofSelection(object sender, MouseButtonEventArgs e)
        {
            if (SelectionStartingPoint != null)
            {
                var SelectionRect = new Rect((Point)SelectionStartingPoint, e.GetPosition(this));
                SelectionStartingPoint = null;
                foreach(var item in Items)
                {
                    if(item is Edge)
                    {
                        var temp = item as Edge;
                        if (SelectionRect.Contains(temp.Borders))
                        {
                            temp.PositionOfSelection = -1;
                            temp.Select();
                        }
                    }
                    else
                    {
                        var temp = item as Node;
                        if (SelectionRect.Contains(new Rect(temp.X, temp.Y, temp.Size.Width, temp.Size.Height)))
                        {
                            temp.PositionOfSelection = int.MaxValue;
                            temp.Select(Brushes.LightBlue);
                        }
                    }
                }
                StopSelection();
            }
        }
    }
}

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

namespace TheGrapho
{
    public class NewItemsControl : ItemsControl
    {
        public static readonly DependencyProperty IsLayoutValidProperty = DependencyProperty.Register(
            "IsLayoutValid",
            typeof(bool),
            typeof(NewItemsControl),
            new PropertyMetadata(OnLayoutValidChanged));
        public bool IsLayoutValid { get { return (bool)GetValue(IsLayoutValidProperty); } set { SetValue(IsLayoutValidProperty, value); } }

        MultiBinding bindings;
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
            bindings = new MultiBinding
            {
                Mode = BindingMode.OneWay,
                Converter = new ChangeValidationPropertyConverter(),
            };
            foreach (var item in Items)
            {
                bindings.Bindings.Add(
                    new Binding
                    {
                        Path = new PropertyPath("IsChildValid"),
                        TargetNullValue = true,
                        FallbackValue = true,
                        Source = item
                    }
                    );
            }
            BindingOperations.SetBinding(this, IsLayoutValidProperty, bindings);
        }
        public class ChangeValidationPropertyConverter : IMultiValueConverter
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
        public void UpdateNodes()
        {
            if (!IsLayoutValid) {
                var rnd = new Random();

                foreach (var item in Items)
                {
                    // TODO: Replace layouter
                    if (item is Node)
                    {
                        var node_item = (Node)item;
                        node_item.X = rnd.Next(20, Math.Max(20, Convert.ToInt32(ActualWidth) - 200));
                        node_item.Y = rnd.Next(20, Math.Max(20, Convert.ToInt32(ActualHeight) - 100));
                        node_item.IsChildvalid = true;
                    }
                }
            }
            UpdateEdges();
        }
        public void UpdateEdges()
        {
            foreach(var item in Items)
                if(item is Edge)
                {
                    var temp = item as Edge;
                    temp.IsChildvalid = true;
                    temp.DrawLine();
                }
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

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
        public NewItemsControl() : base()
        {
            ((INotifyCollectionChanged)Items).CollectionChanged += CreateMultiBinding;
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
                        node_item.X = rnd.Next(20, Convert.ToInt32(ActualWidth) - 200);
                        node_item.Y = rnd.Next(20, Convert.ToInt32(ActualHeight) - 100);
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

    }
}

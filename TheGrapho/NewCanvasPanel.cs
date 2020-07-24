// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Windows.Media;

namespace TheGrapho
{
    public class NewCanvasPanel : Canvas
    {
        private static readonly DependencyPropertyDescriptor TopPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(TopProperty, typeof(Canvas));

        private static readonly DependencyPropertyDescriptor LeftPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(LeftProperty, typeof(Canvas));

        public static void RegisterForInvalidations(DependencyObject control)
        {
            TopPropertyDescriptor.AddValueChanged(control, Handler);
            LeftPropertyDescriptor.AddValueChanged(control, Handler);
        }

        private static void Handler(object sender, EventArgs e)
        {
            if (sender is UIElement uie)
            {
                FindParentOfType<NewCanvasPanel>(uie)?.InvalidateMeasure();
            }
        }

        private static T FindParentOfType<T>(DependencyObject child) where T : DependencyObject
        {
            var parentDepObj = child;
            do
            {
                parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
                if (parentDepObj is T parent) return parent;
            }
            while (parentDepObj != null);
            return null;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var new_size = base.MeasureOverride(constraint);
            foreach (var e in this.InternalChildren)
            {
                var temp = e as ContentPresenter;
                if(temp.Content is Node)
                {
                    var tempNode = temp.Content as Node;
                    tempNode.Size = temp.DesiredSize;
                    if (new_size.Width<tempNode.X+temp.ActualWidth)
                    {
                        new_size.Width = tempNode.X + temp.ActualWidth;
                    }
                    if (new_size.Height < tempNode.Y + temp.ActualHeight)
                    {
                        new_size.Height = tempNode.Y + temp.ActualHeight;
                    }
                }
            }
            new_size.Height += 50;
            new_size.Width += 50;
            return new_size;
        }
    }
}

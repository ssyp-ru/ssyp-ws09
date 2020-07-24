// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TheGrapho
{
    public class BaseItem : DependencyObject
    {
        public static readonly DependencyProperty IsChildValidProperty = DependencyProperty.Register(
            "IsChildValid",
            typeof(bool),
            typeof(BaseItem),
            new PropertyMetadata(true));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y",
            typeof(double),
            typeof(BaseItem));
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X",
            typeof(double),
            typeof(BaseItem));
        public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register(
            "ZIndex",
            typeof(int),
            typeof(BaseItem));
        public static readonly DependencyProperty StyleProperty = DependencyProperty.Register(
            "CurrentStyle",
            typeof(Style),
            typeof(BaseItem));
        public double X { get { return (double)GetValue(XProperty); } set { SetValue(XProperty, value); } }
        public double Y { get { return (double)GetValue(YProperty); } set { SetValue(YProperty, value); } }
        public int ZIndex { get { return (int)GetValue(ZIndexProperty); } set { SetValue(ZIndexProperty, value); } }
        public bool IsChildvalid { get { return (bool)GetValue(IsChildValidProperty); } set { SetValue(IsChildValidProperty, value); } }
        public int? PositionOfSelection;
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke",
            typeof(Brush),
            typeof(BaseItem),
            new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness",
            typeof(int),
            typeof(BaseItem),
            new PropertyMetadata(2));
        protected int _Thickness = 2;
        protected Brush _Stroke = Brushes.Black;
        public int Thickness { get { return (int)GetValue(ThicknessProperty); } set { SetValue(ThicknessProperty, value); } }
        public Brush Stroke { get { return (Brush)GetValue(StrokeProperty); } set { SetValue(StrokeProperty, value); } }
        public virtual void Select(Brush color = null)
        {
            Thickness = 2;
            Stroke = color ?? Brushes.Blue;
        }
        public virtual void Deselect() 
        {
            Thickness = _Thickness;
            Stroke = _Stroke;
        }
    }
}

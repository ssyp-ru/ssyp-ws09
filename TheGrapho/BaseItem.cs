// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TheGrapho
{
    class BaseItem : DependencyObject
    {
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y",
            typeof(double),
            typeof(BaseItem));
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X",
            typeof(double),
            typeof(BaseItem));
        public double X { get { return (double)GetValue(XProperty); } set { SetValue(XProperty, value); } }
        public double Y { get { return (double)GetValue(YProperty); } set { SetValue(YProperty, value); } }

    }
}

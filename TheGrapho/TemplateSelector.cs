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

namespace TheGrapho
{
    public class TemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement el = container as FrameworkElement;
            if (item is Node)
                return el.FindName("NodeTemplate") as DataTemplate;
            else if (item is Edge)
                return el.FindName("EdgeTemplate") as DataTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}

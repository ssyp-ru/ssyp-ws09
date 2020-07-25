﻿// This Source Code Form is subject to the terms of the Mozilla Public
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
    class Node : BaseItem
    {
        public static readonly DependencyProperty NameProperty;
        string Name { get { return GetValue(NameProperty).ToString(); } set { SetValue(NameProperty, value); } }

        Node() 
        {
            
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

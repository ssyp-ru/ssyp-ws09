// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TheGrapho
{
    // TODO: Write this
    class EdgeControl : Control
    {
        public EdgeControl()
        {
            PreviewMouseDown += SelectEdge;
        }
        protected void SelectEdge(object obj, MouseButtonEventArgs e)
        {
            var temp = (BaseItem)DataContext;
            if (temp.PositionOfSelection == null)
            {
                temp.PositionOfSelection = -1;
                temp.Select();
            }
            else
            {
                temp.PositionOfSelection = null;
                temp.Deselect();
            }
        }
    }
}

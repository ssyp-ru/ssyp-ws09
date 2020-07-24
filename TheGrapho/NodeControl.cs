// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TheGrapho
{
    class NodeControl : Thumb
    {
        public NodeControl()
        {
            PreviewMouseDown += SelectItem;
            DragDelta += new DragDeltaEventHandler(OnThumbDragDelta);
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs args)
        {
            var window = Window.GetWindow(this) as MainWindow ?? throw new ArgumentNullException();
            if (window.AllowMove)
            {
                ((BaseItem)DataContext).X += args.HorizontalChange;
                ((BaseItem)DataContext).Y += args.VerticalChange;
                ((BaseItem)DataContext).PositionOfSelection = null;
                ((BaseItem)DataContext).Deselect();
                window.MainItemsControl.SelectionStartingPoint = null;
                
            }
        }
        private void SelectItem(object sender, MouseButtonEventArgs e)
        {
            var temp = DataContext as BaseItem;
            if (temp.PositionOfSelection == null || temp.PositionOfSelection == int.MaxValue)
            {
                Node.Selected++;
                temp.PositionOfSelection = Node.Selected;
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

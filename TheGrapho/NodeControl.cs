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
using System.Windows.Controls.Primitives;

namespace TheGrapho
{
    class NodeControl : Thumb
    {
        public NodeControl()
        {
            DragDelta += new DragDeltaEventHandler(OnThumbDragDelta);
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs args)
        {
            var window = Window.GetWindow(this) as MainWindow ?? throw new ArgumentNullException();
            if (window.AllowMove)
            {
                ((BaseItem)DataContext).X += args.HorizontalChange;
                ((BaseItem)DataContext).Y += args.VerticalChange;
                window.MainItemsControl.UpdateEdges();
#if false
                foreach(var item in ((Node)DataContext).Edges)
                {
                    //MessageBox.Show($"Source: {item.}");
                    item.DrawLine();
                }
#endif
            }
        }
    }
}

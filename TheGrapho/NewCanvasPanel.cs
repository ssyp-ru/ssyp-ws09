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

namespace TheGrapho
{
    class NewCanvasPanel : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            var new_size = base.MeasureOverride(constraint);
            foreach (var e in this.InternalChildren)
            {
                var temp = e as ContentPresenter;
                if(temp.Content is Node)
                {
                    (temp.Content as Node).Size = temp.DesiredSize;
                }
            }
            return new_size;
        }
    }
}

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TheGrapho
{
    public class ZoomBorder : Border
    {
        private UIElement _child;
        private Point _origin;
        private Point _start;

        private static TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private static ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (value != null && value != Child)
                    Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            _child = element;

            if (_child == null) return;

            _child.RenderTransform = new TransformGroup
            {
                Children =
                {
                    new ScaleTransform(),
                    new TranslateTransform()
                }
            };

            _child.RenderTransformOrigin = new Point(0.0, 0.0);
            MouseWheel += ChildMouseWheel;
            MouseMove += ChildMouseMove;
            PreviewMouseDown += ChildPreviewMouseDown;
            PreviewMouseUp += ChildPreviewMouseUp;
        }

        public void Reset(object sender, RoutedEventArgs e)
        {
            if (_child == null) return;

            // reset zoom
            var st = GetScaleTransform(_child);
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;

            // reset pan
            var tt = GetTranslateTransform(_child);
            tt.X = 0.0;
            tt.Y = 0.0;
        }

        #region Child Events

        private void ChildMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_child == null) return;

            var st = GetScaleTransform(_child);
            var tt = GetTranslateTransform(_child);

            var zoom = e.Delta > 0 ? .2 : -.2;
            if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                return;

            var relative = e.GetPosition(_child);

            var absoluteX = relative.X * st.ScaleX + tt.X;
            var absoluteY = relative.Y * st.ScaleY + tt.Y;

            var zoomCorrected = zoom * st.ScaleX;
            st.ScaleX += zoomCorrected;
            st.ScaleY += zoomCorrected;

            tt.X = absoluteX - relative.X * st.ScaleX;
            tt.Y = absoluteY - relative.Y * st.ScaleY;
        }

        private void ChildPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_child == null || e.ChangedButton != MouseButton.Middle) return;

            var tt = GetTranslateTransform(_child);
            _start = e.GetPosition(this);
            _origin = new Point(tt.X, tt.Y);
            Cursor = Cursors.SizeAll;
            _child.CaptureMouse();
        }

        private void ChildPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_child == null || e.ChangedButton != MouseButton.Middle) return;

            _child.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
        }

        private void ChildMouseMove(object sender, MouseEventArgs e)
        {
            if (_child == null) return;

            if (!_child.IsMouseCaptured) return;
            
            var tt = GetTranslateTransform(_child);
            var v = _start - e.GetPosition(this);
            tt.X = _origin.X - v.X;
            tt.Y = _origin.Y - v.Y;
        }

        #endregion
    }
}

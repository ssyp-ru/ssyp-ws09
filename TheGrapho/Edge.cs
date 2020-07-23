// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
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
    public class Edge : BaseItem
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "MainPath",
            typeof(PathGeometry),
            typeof(Edge));
        Node Source, Target;
        bool IsDirect;
        PathGeometry Path { get { return (PathGeometry)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
        public Edge(Node source, Node target, bool isDirect)
        {
            Source = source;
            Target = target;
#if false
            source.Edges.Add(this);
            target.Edges.Add(this);
#endif
            IsDirect = isDirect;
            Path = new PathGeometry();  // TODO: Move this to method
            DrawLine();
            this.ZIndex = 0;
        }
        public void DrawLine()
        {
            // TODO: Add arrows
            Path.Clear();
            var figure = new PathFigure();
            var (start_point, end_point) = FindOptimalCords();
            figure.Segments.Add(new LineSegment(start_point, true));
            figure.StartPoint = end_point;
            figure.IsClosed = true;
            Path.Figures.Add(figure);
        }
        private (Point, Point) FindOptimalCords()
        {
            double xa1 = 0, ya1 = 0, xa2 = 0, ya2 = 0;
            if ((xa1 = Source.X) > (xa2 = Target.X + Target.Size.Width)) { }
            else if ((xa1 = Source.X + Source.Size.Width) < (xa2 = Target.X)) { }
            else
            {
                xa1 = Source.X + Source.Size.Width / 2;
                xa2 = Target.X + Target.Size.Width / 2;
            }
            if ((ya1 = Source.Y) > (ya2 = Target.Y + Target.Size.Height)) { }
            else if ((ya1 = Source.Y + Source.Size.Height) < (ya2 = Target.Y)) { }
            else
            {
                ya1 = Source.Y + Source.Size.Height / 2;
                ya2 = Target.Y + Target.Size.Height / 2;
            }
            return (new Point(xa1, ya1), new Point(xa2, ya2));
        }
        static Edge()
        {

        }
    }
}

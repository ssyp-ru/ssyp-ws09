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
using System.Windows.Ink;
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
        public Node Source, Target;
        bool IsDirect;
        public Rect Borders;
        public PathGeometry Path { get { return (PathGeometry)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
        public Edge(Node source, Node target, bool isDirect, Style style = null)
        {
            Source = source;
            Target = target;
            IsDirect = isDirect;
            //DrawLine();
            this.ZIndex = 0;
        }
        public void DrawLine()
        {
            // TODO: Add arrows
            var figure = new PathFigure();
            var (start_point, end_point) = FindOptimalCords();
            figure.Segments.Add(new LineSegment(end_point, true));
            figure.StartPoint = start_point;
            figure.IsClosed = false;
            if (IsDirect)
            {
                double sin = 1, cos = 1;
                var vec = (start_point - end_point);
                vec.Normalize();
                double dx = vec.X, dy = vec.Y;
                figure.Segments.Add(
                    new LineSegment(
                        new Point(
                            end_point.X + (dx * cos + dy * -sin),
                            end_point.Y + (dx * sin + dy * cos)
                            ), 
                        true
                        )
                    );
                figure.Segments.Add(new LineSegment(end_point, true));
                figure.Segments.Add(
                    new LineSegment(
                        new Point(
                            end_point.X + (dx * cos + dy * sin),
                            end_point.Y + (dx * -sin + dy * cos)
                            ), 
                        true
                        )
                    );
                figure.Segments.Add(new LineSegment(end_point, true));
            }
            Borders = new Rect(start_point, end_point);
            Path = new PathGeometry();  // TODO: Move this to method
            Path.Figures.Add(figure);
            if (PositionOfSelection != null)
                Select();
        }
        public override void Select(Brush color = null)
        {
            base.Select(color);
            var rectgeom = new RectangleGeometry();
            rectgeom.Rect = Borders;
            Path.AddGeometry(rectgeom);
            //temp.CurrentStyle.Setters;
        }
        public override void Deselect()
        {
            base.Deselect();
            DrawLine();
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

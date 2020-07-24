// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.Win32;
using System.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TheGrapho.Parser;
using TheGrapho.Parser.SimpleModel;
using System.Windows.Controls.Primitives;
using System.Globalization;
using TheGrapho.Parser.Syntax;
using System.Drawing.Printing;

namespace TheGrapho
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScaleTransform scaleTransform = new ScaleTransform();
        private string currentFile;
        public static readonly DependencyProperty AllowMoveProperty = DependencyProperty.Register(
            "AllowMove",
            typeof(bool),
            typeof(MainWindow));
        public bool AllowMove { get { return (bool)GetValue(AllowMoveProperty); } set { SetValue(AllowMoveProperty, value); } }

        public static readonly DependencyProperty AllowRenameProperty = DependencyProperty.Register(
            "AllowRename",
            typeof(bool),
            typeof(MainWindow));
        public bool AllowRename { get { return (bool)GetValue(AllowRenameProperty); } set { SetValue(AllowRenameProperty, value); } }

        public ObservableCollection<BaseItem> Items { get; private set; } = new ObservableCollection<BaseItem>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            resetScale.Click += new RoutedEventHandler(zoomBorder.Reset);

            Node Item1 = new Node("Testing");
            Node Item = new Node("Testing coords");
            Node item2 = new Node("More testing") { X = 5, Y = 100 };
            Item.X = 100;
            Item.Y = 5;
            Items.Add(Item);
            Items.Add(Item1);
            Items.Add(item2);
            Items.Add(new Edge(Item, Item1, true));
            Items.Add(new Edge(Item1, item2, true));
            MainItemsControl.Loaded += DebugMethod;
        }

        private void DebugMethod(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Items.Add(
                new Node(
                    (Point)(Mouse.GetPosition(MainItemsControl) - Mouse.GetPosition(sender as MenuItem))
                    )
                );  // TODO: may be rewrite this
        }
        private ICommand _canvasOpenMenu;
        public ICommand CanvasOpenMenu
        {
            get
            {
                if(_canvasOpenMenu == null)
                {
                    _canvasOpenMenu = new RelayCommand(
                        e => true,
                        e => CanvasRightClick(e)
                        );
                }
                return _canvasOpenMenu;
            }
        }
        private void CanvasRightClick(object sender)
        {
            ContextMenu cm = this.FindResource("RightClickMenu") as ContextMenu;
            cm.PlacementTarget = sender as Canvas;
            cm.IsOpen = true;
        }

        public void LoadFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                currentFile = dialog.FileName;
                Graph graph = new Parser.Parser(new Scanner(File.ReadAllText(dialog.FileName)).ScanTillEnd().ToList())
                .Parse().ConvertToSimpleModel();
                Dictionary<string, Node> nodes = new Dictionary<string, Node>();
                foreach (var item in graph.Nodes)
                {
                    Node temp = new Node(item.Name);
                    nodes.Add(item.Name, temp);
                    Items.Add(temp);
                }
                foreach (var item in graph.Edges)
                {
                    Items.Add(new Edge(nodes[item.Source.Name], nodes[item.Target.Name], false));
                }
            }
            else
            {
                return;
            }
        }

        public void SaveFile(object sender, RoutedEventArgs e)
        {
            if (currentFile == null)
            {
                var dialog = new SaveFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    currentFile = dialog.FileName;
                }
                else return;
            }
            // Code to save to file
            // Everyone one can rewrite it
            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();
            foreach(var item in Items)
            {
                if (item is Node)
                {
                    nodes.Add((Node)item);
                }
                if (item is Edge)
                {
                    edges.Add((Edge)item);
                }
            }
            StreamWriter writer = new StreamWriter(currentFile, false);
            writer.WriteLine("digraph digraphName {");
            foreach(var item in nodes)
            {
                writer.WriteLine($"\t{item.Name};");
            }
            foreach(var item in edges)
            {
                writer.WriteLine($"\t{item.Source.Name} -- {item.Target.Name}");
            }
            writer.Write("}");
            writer.Flush();
            writer.Close();
        }

        public void ClearCanvas(object sender, RoutedEventArgs e)
        {
            Items.Clear();
            currentFile = null;
        }

        private void InvalidateAll(object sender, RoutedEventArgs e)
        {
            foreach (var item in Items)
                item.HasValidLayout = false;
        }

        private void InvalidateNodes(object sender, RoutedEventArgs e)
        {
            foreach (var item in Items.OfType<Node>())
                item.HasValidLayout = false;
        }

        private void InvalidateEdges(object sender, RoutedEventArgs e)
        {
            foreach (var item in Items.OfType<Edge>())
                item.HasValidLayout = false;
        }

        private void InvalidateRoutes(object sender, RoutedEventArgs e)
        {
            foreach (var item in Items.OfType<Node>())
                item.AreDependentRoutesValid = false;
        }

        private void DoFullLayout(object sender, RoutedEventArgs e)
        {
            MainItemsControl.UpdateNodes();
        }

        private void DoEdgeLayout(object sender, RoutedEventArgs e)
        {
            MainItemsControl.UpdateEdges();
        }

        private void SwitchToRandomLayout(object sender, RoutedEventArgs e)
        {
            MainItemsControl.Algorithm = NewItemsControl.LayoutAlgorithm.Random;
        }

        private void SwitchToCircularLayout(object sender, RoutedEventArgs e)
        {
            MainItemsControl.Algorithm = NewItemsControl.LayoutAlgorithm.Circular;
        }

        private void AddEdgesAsGod(object sender, RoutedEventArgs e)
        {
            MainItemsControl.AddEdgesAsMason();
        }
        private void AddEdgesAsChain(object sender, RoutedEventArgs e)
        {
            MainItemsControl.AddEdgesAsChain();
        }

        private void DeleteItems(object sender, RoutedEventArgs e)
        {
            MainItemsControl.DeleteSelectedItems();
        }

        private void RemoveSelection(object sender, RoutedEventArgs e)
        {
            MainItemsControl.RemoveSelection();
        }
    }

    //public class NegateConverter:IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return !(bool)value;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return !(bool)value;
    //    }
    //}
}

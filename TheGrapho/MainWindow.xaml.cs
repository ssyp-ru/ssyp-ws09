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

namespace TheGrapho
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

            Node Item1 = new Node("Testing");
            Node Item = new Node("Testing coords");
            Node item2 = new Node("More testing") { X = 5, Y = 100 };
            Item.X = 100;
            Item.Y = 5;
            Items.Add(Item);
            Items.Add(Item1);
            Items.Add(item2);
            Items.Add(new Edge(Item, Item1, false));
            Items.Add(new Edge(Item1, item2, false));
            MainItemsControl.Loaded += DebugMethod;
        }
        private void DebugMethod(object sender, RoutedEventArgs e)
        {
            UpdateGraph();
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
            List<DotNodeStatementSyntax> nodes = new List<DotNodeStatementSyntax>();
            List<DotEdgeStatementSyntax> edges = new List<DotEdgeStatementSyntax>();
            foreach(var item in Items)
            {
                if(item is Node)
                {
                    //nodes.Add(new DotNodeStatementSyntax())
                }
            }
        }

        public void ClearCanvas(object sender, RoutedEventArgs e)
        {
            Items.Clear();
        }

        private void UpdateGraphClick(object sender, RoutedEventArgs e)
        {
            UpdateGraph();
        }
        private void UpdateGraph()
        {
            Items.Last().IsChildvalid = false;
        }
        private void DrawEdges()
        {
            MainItemsControl.UpdateEdges();
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

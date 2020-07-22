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

namespace TheGrapho
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<BaseItem> Items { get; } = new ObservableCollection<BaseItem>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Node Item1 = new Node("Testing");
            Node Item = new Node("Testing coords");
            Item.X = 100;
            Item.Y = 5;
            Items.Add(Item);
            Items.Add(Item1);
            MainItemsControl.Loaded += DebugMethod;
        }
        private void DebugMethod(object sender, RoutedEventArgs e)
        {
            Items.Add(new Edge(Items.First() as Node, Items.Last() as Node, false));
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
                Graph graph = new Parser.Parser(new Scanner(File.ReadAllText(dialog.FileName)).ScanTillEnd().ToList())
                .Parse().ConvertToSimpleModel();
            }
            else
            {
                return;
            }
        }
    }
}

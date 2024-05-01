using System;
using System.Collections.Generic;
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
using mkllab;

namespace Wpfdop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        V2MainCollection maincollect = new V2MainCollection(2, 2);
        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = maincollect;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Binding mybind = new Binding();
            //mybind.Source = maincollect;
            //mybind.Path = new PropertyPath("thisClassInString");
            //MyListBox.SetBinding(TextBox.TextProperty, mybind);
        }
    }
}

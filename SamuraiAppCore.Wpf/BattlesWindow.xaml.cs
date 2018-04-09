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
using System.Windows.Shapes;

namespace SamuraiAppCore.Wpf
{
    /// <summary>
    /// Interaction logic for BattlesWindow.xaml
    /// </summary>
    public partial class BattlesWindow : Window
    {
        public BattlesWindow()
        {
            InitializeComponent();
        }

        //TODO

        private void GotoSamurais_Click(object sender, RoutedEventArgs e)
        {
            var samuraisWindow = new MainWindow();
            samuraisWindow.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource battleViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("battleViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // battleViewSource.Source = [generic data source]
        }

        private void startDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO
        }

        private void endDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO
        }

        private void battleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO
        }
    }
}

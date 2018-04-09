using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO
            System.Windows.Data.CollectionViewSource battleViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("battleViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // battleViewSource.Source = [generic data source]
        }

        private void battleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        #region AddSamuraiToBattle

        private void samuraisInBattle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TODO
        }

        private void samuraisInBattle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //TODO
        }

        private void samuraisInBattle_DragEnter(object sender, DragEventArgs e)
        {
            //TODO
        }

        private void samuraisInBattle_Drop(object sender, DragEventArgs e)
        {
            //TODO
        }

        #endregion AddSamuraiToBattle

        #region RemoveSamuraiFromBattle

        private void samuraisNotInBattle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TODO
        }

        private void samuraisNotInBattle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //TODO
        }

        private void samuraisNotInBattle_DragEnter(object sender, DragEventArgs e)
        {
            //TODO
        }

        private void samuraisNotInBattle_Drop(object sender, DragEventArgs e)
        {
            //TODO
        }

        #endregion RemoveSamuraiFromBattle

        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO
        }

        private void startDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO
        }

        private void endDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO
        }

        private void NoteSamuraiMove()
        {
            //TODO
        }

        private void GatherSelectedItemForMove(object sender, MouseEventArgs e)
        {
            //TODO
        }

        private static void IgnoreNonSamuraiItem(object sender, DragEventArgs e)
        {
            //TODO
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void GotoSamurais_Click(object sender, RoutedEventArgs e)
        {
            var samuraisWindow = new MainWindow();
            samuraisWindow.Show();
            Close();
        }
    }
}

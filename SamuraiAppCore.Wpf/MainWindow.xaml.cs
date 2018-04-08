using SamuraiAppCore.Data;
using System.Windows;
using System.Windows.Data;

namespace SamuraiAppCore.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly ConnectedData _repo = new ConnectedData();
        // TODO
        private bool _isLoading;
        private ObjectDataProvider _samuraiViewSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoading = true;
            samuraiListBox.ItemsSource = _repo.SamuraisListInMemory();
            //TODO _samuraiViewSource = (ObjectDataProvider)FindResource("SamuraiViewSource");
            _isLoading = false;
            samuraiListBox.SelectedIndex = 0;
            System.Windows.Data.CollectionViewSource samuraiViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("samuraiViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // samuraiViewSource.Source = [generic data source]
        }

        private void samuraiListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //TODO
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            //TDOO
        }

        private void realNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //TODO
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void nameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //TODO
        }

        private void nameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void quotesDataGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            //TODO
        }
    }
}

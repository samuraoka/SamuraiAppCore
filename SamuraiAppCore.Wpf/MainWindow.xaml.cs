using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SamuraiAppCore.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly ConnectedData _repo = new ConnectedData();
        private Samurai _currentSamurai;
        private bool _isListChanging;
        private bool _isLoading;
        private ObjectDataProvider _samuraiDataProvider;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoading = true;
            samuraiListBox.ItemsSource = _repo.SamuraisListInMemory();
            _samuraiDataProvider = (ObjectDataProvider)FindResource("samuraiDataProvider");
            _isLoading = false;
            samuraiListBox.SelectedIndex = 0;
        }

        private void samuraiListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_isLoading == false)
            {
                _isListChanging = true;
                _currentSamurai = _repo.LoadSamuraiGraph((int)samuraiListBox.SelectedValue);
                _samuraiDataProvider.ObjectInstance = _currentSamurai;
                _isListChanging = false;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            _repo.SaveChanges(_currentSamurai.GetType());
        }

        private void realNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_isLoading == false && _isListChanging == false)
            {
                if (_currentSamurai.SecretIdentity == null)
                {
                    _currentSamurai.SecretIdentity = new SecretIdentity();
                }
                _currentSamurai.SecretIdentity.RealName = ((TextBox)sender).Text;
                _currentSamurai.IsDirty = true;
            }
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            _currentSamurai = _repo.CreateNewSamurai();
            _samuraiDataProvider.ObjectInstance = _currentSamurai;
            samuraiListBox.SelectedItem = _currentSamurai;
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

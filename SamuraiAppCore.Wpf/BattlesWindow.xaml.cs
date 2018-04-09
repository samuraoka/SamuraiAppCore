using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SamuraiAppCore.Wpf
{
    /// <summary>
    /// Interaction logic for BattlesWindow.xaml
    /// </summary>
    public partial class BattlesWindow : Window
    {
        private readonly ConnectedData _repo = new ConnectedData();
        private List<Samurai> _availableSamurais;
        private ObjectDataProvider _battleViewSource;
        private Battle _currentBattle;
        private bool _isListChanging;
        private bool _isLoading;
        private Point _startPoint;

        public BattlesWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoading = true;
            battleListBox.ItemsSource = _repo.BattlesListInMemory();
            _battleViewSource = (ObjectDataProvider)FindResource("battleDataProvider");
            _isLoading = false;
            battleListBox.SelectedIndex = 0;
        }

        private void battleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentBattle != null && _currentBattle.IsDirty)
            {
                ShowSaveBattleMessageBox();
            }

            if (_isLoading == false)
            {
                _isListChanging = true;
                _currentBattle = _repo.LoadBattleGraph((int)battleListBox.SelectedValue);
                _battleViewSource.ObjectInstance = _currentBattle;
                _availableSamurais = _repo.SamuraisNotInBattle(_currentBattle.Id);
                samuraisNotInBattle.ItemsSource = _availableSamurais;
                _isListChanging = false;
            }
        }

        private void ShowSaveBattleMessageBox()
        {
            //TODO
            throw new NotImplementedException();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _repo.SaveChanges(_currentBattle.GetType());
        }

        #region AddSamuraiToBattle

        private void samuraisNotInBattle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void samuraisNotInBattle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            GatherSelectedItemForMove(sender, e);
        }

        private void samuraisNotInBattle_DragEnter(object sender, DragEventArgs e)
        {
            IgnoreNonSamuraiItem(sender, e);
        }

        private void AddDroppedSamuraiToBattle(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Samurai)))
            {
                var samurai = e.Data.GetData(typeof(Samurai)) as Samurai;
                var samuraiBattle = new SamuraiBattle
                {
                    Battle = _currentBattle,
                    Samurai = samurai,
                };
                _repo.AddSamuraiBattle(samuraiBattle);
                _availableSamurais.Remove(samurai);
                NoteSamuraiMove();
            }
        }

        #endregion AddSamuraiToBattle

        #region RemoveSamuraiFromBattle

        private void samuraisInBattle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TODO _startPoint = e.GetPosition(null);
        }

        private void samuraisInBattle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //TODO GatherSelectedItemForMove(sender, e);
        }

        private void samuraisInBattle_DragEnter(object sender, DragEventArgs e)
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
            if (_isLoading == false && _isListChanging == false)
            {
                _currentBattle.IsDirty = true;
            }
        }

        private void nameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            battleListBox.Items.Refresh();
        }


        private void startDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading == false && _isListChanging == false)
            {
                _currentBattle.IsDirty = true;
            }
        }

        private void endDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading == false && _isListChanging == false)
            {
                _currentBattle.IsDirty = true;
            }
        }

        private void NoteSamuraiMove()
        {
            samuraisNotInBattle.Items.Refresh();
            samuraisInBattle.Items.Refresh();
            _currentBattle.IsDirty = true;
            battleListBox.Items.Refresh();
        }

        private void GatherSelectedItemForMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(null);
            var diff = _startPoint - mousePos;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listBox = sender as ListBox;
                var listBoxItem = listBox.SelectedItem;
                if (listBoxItem != null)
                {
                    DragDrop.DoDragDrop(listBox, listBoxItem, DragDropEffects.Move);
                }
            }
        }

        private static void IgnoreNonSamuraiItem(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Samurai)) == false ||
                sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _isListChanging = true;
            _currentBattle = _repo.CreateNewBattle();
            _battleViewSource.ObjectInstance = _currentBattle;
            battleListBox.SelectedItem = _currentBattle;
            samuraisInBattle.ItemsSource = _currentBattle.SamuraiBattles;
            _isListChanging = false;
            _currentBattle.IsDirty = true;
        }

        private void GotoSamurais_Click(object sender, RoutedEventArgs e)
        {
            var samuraisWindow = new MainWindow();
            samuraisWindow.Show();
            Close();
        }
    }
}

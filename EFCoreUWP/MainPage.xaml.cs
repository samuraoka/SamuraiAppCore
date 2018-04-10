using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EFCoreUWP
{
    public sealed partial class MainPage : Page
    {
        private BingeViewModel _binge;

        public MainPage()
        {
            this.InitializeComponent();
            _binge = new BingeViewModel();
            NomText.Visibility = Visibility.Collapsed;
            // Refresh binge list after a new binge is added
            _binge.BingeCompleted += s => { ReloadHistory(); };
            BingePane.DataContext = _binge;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadHistory();
        }

        private void ReloadHistory()
        {
            BingeList.ItemsSource = BingeService.GetLast5Binges();
        }

        private void Play_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _binge.StartNewBinge();
        }

        private void WorthIt_Click(object sender, RoutedEventArgs e)
        {
            _binge.StoreBinge(true);
        }

        private void NotWorthIt_Click(object sender, RoutedEventArgs e)
        {
            _binge.StoreBinge(false);
        }

        private void CookieImage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_binge.Binging)
            {
                ((Image)sender).CapturePointer(e.Pointer);

                var pt = e.GetCurrentPoint(BingeGrid);
                NomText.Margin = new Thickness(pt.Position.X, pt.Position.Y, 0, 0);

                NomText.Visibility = Visibility.Visible;
                _binge.HandleClick();
                ReloadHistory();
            }
        }

        private void CookieImage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ((Image)sender).ReleasePointerCapture(e.Pointer);

            NomText.Visibility = Visibility.Collapsed;
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            BingeService.ClearHistory();
            ReloadHistory();
        }
    }
}

using System;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace EFCoreUWP
{
    public class BingeViewModel : INotifyPropertyChanged
    {
        public delegate void BingeNotedHandler(object sender);

        public delegate void NomVisibleHandler(object sender);

        private int _clickCount;
        private bool _playing;
        private Visibility _startControlsVisibility = Visibility.Visible;
        private Visibility _stopControlsVisibility = Visibility.Collapsed;

        public int ClickCount
        {
            get { return _clickCount; }
            set
            {
                _clickCount = value;
                OnPropertyChanged("ClickCount");
            }
        }

        public Visibility StartControlsVisibility
        {
            get { return _startControlsVisibility; }
            set
            {
                _startControlsVisibility = value;
                OnPropertyChanged("StartControlsVisibility");
            }
        }

        public Visibility StopControlsVisibility
        {
            get { return _stopControlsVisibility; }
            set
            {
                _stopControlsVisibility = value;
                OnPropertyChanged("StopControlsVisibility");
            }
        }

        public bool Binging
        {
            get { return _playing; }
            set
            {
                _playing = value;
                OnPropertyChanged("Playing");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event BingeNotedHandler BingeCompleted;

        private void OnPropertyChanged(string property)
        {
            //TODO
            throw new NotImplementedException();
        }

        //TODO

        public void StartNewBinge()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void HandleClick()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}

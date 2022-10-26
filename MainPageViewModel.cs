using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;

namespace avalonia_play
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Item> items;

        public MainPageViewModel()
        {
        }

        public ObservableCollection<Item> Items
        {
            get => this.items;
            set
            {
                this.items = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
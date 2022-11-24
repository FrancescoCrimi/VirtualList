using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Sample.Uwp.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CiccioSoft.VirtualList.Sample.Uwp
{
    public class MainViewModel : ObservableRecipient
    {
        private readonly ModelVirtualCollection items;
        //private readonly FakeCollection items;
        private IAsyncRelayCommand searchCommand;

        public MainViewModel()
        {
            SearchString = string.Empty;

            items = new ModelVirtualCollection();
            //items = new FakeCollection();
            items.PropertyChanged += Items_PropertyChanged;
        }

        private void Items_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Count")
            {
                OnPropertyChanged("Count");
            }
        }

        public async Task LoadAsync() => await items.LoadAsync();

        public int Count => items.Count;

        public IList<Model> Items => items;

        public string SearchString
        {
            get; set;
        }

        public IAsyncRelayCommand SearchCommand => searchCommand ??
            (searchCommand = new AsyncRelayCommand(async () =>
                await items.LoadAsync(SearchString)));
    }
}

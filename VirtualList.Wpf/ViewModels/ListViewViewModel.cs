using System.Collections.Generic;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Wpf.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CiccioSoft.VirtualList.Wpf.ViewModels
{
    public class ListViewViewModel : ObservableObject
    {
        private readonly ModelVirtualCollection items = new();
        //private readonly FakeCollection items = new();
        private string searchString = string.Empty;
        private AsyncRelayCommand? searchCommand;

        public async Task LoadAsync() => await items.LoadAsync();
        public async Task SearchAsync() => await items.LoadAsync(searchString);

        public IVirtualCollection<Model> Items => items;

        public string SearchString
        {
            get => searchString;
            set => SetProperty(ref searchString, value);
        }

        public IAsyncRelayCommand SearchCommand => searchCommand ??= new AsyncRelayCommand(async () =>
        {
            await items.LoadAsync(SearchString);
        });
    }
}
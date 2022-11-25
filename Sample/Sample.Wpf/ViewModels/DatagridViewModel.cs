using System.Collections.Generic;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Sample.Domain;
using CiccioSoft.VirtualList.Sample.Wpf.Collection;
using CiccioSoft.VirtualList.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CiccioSoft.VirtualList.Sample.Wpf.ViewModels
{
    public class DatagridViewModel : ObservableObject
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

        /// <summary>
        /// Non usato perche si sta usando il code behind.
        /// Per usare questa ICommand provare ad implementare o una "attached property" 
        /// o una "attached behavior"
        /// https://stackoverflow.com/questions/4793030/wpf-reset-listbox-scroll-position-when-itemssource-changes
        /// 
        /// </summary>
        public IAsyncRelayCommand SearchCommand => searchCommand ??= new AsyncRelayCommand(async () =>
        {
            await items.LoadAsync(SearchString);
        });
    }
}
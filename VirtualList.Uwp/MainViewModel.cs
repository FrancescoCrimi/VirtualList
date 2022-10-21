using CiccioSoft.VirtualList.Data.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public class MainViewModel : ObservableRecipient
    {
        //private readonly ModelVirtualCollection items;
        private readonly ModelVirtualRangeCollection items;
        //private readonly FakeCollection items;
        private IAsyncRelayCommand searchCommand;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            SearchString = string.Empty;

            //items = new ModelVirtualCollection();
            items = new ModelVirtualRangeCollection();
            //items = new FakeCollection();
            Task.Run(async () => await items.LoadAsync());
        }

        public IList<Model> Items => items;

        public string SearchString { get; set; }

        public IAsyncRelayCommand SearchCommand => searchCommand ??
            (searchCommand = new AsyncRelayCommand(async () =>
                await items.SearchAsync(SearchString)));
    }
}

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

        //private readonly FakeCollection fakeList;
        //private readonly ModelVirtualRangeCollection items;
        private readonly ModelVirtualRangeCollection items;
        private IAsyncRelayCommand reloadCommand;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            SearchString = string.Empty;
            //items = new ModelVirtualCollection();
            items = new ModelVirtualRangeCollection();
            Task.Run(async () => await items.LoadAsync());
            //fakeList = new FakeCollection();
            //Items = fakeList;
        }

        public IList<Model> Items => items;

        public string SearchString { get; set; }

        public IAsyncRelayCommand ReloadCommand => reloadCommand ??
            (reloadCommand = new AsyncRelayCommand(async () =>
                await items.Reload(SearchString)));
    }
}

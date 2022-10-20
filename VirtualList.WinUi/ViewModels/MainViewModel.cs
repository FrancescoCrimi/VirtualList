using CiccioSoft.VirtualList.Data.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualList.WinUi.Collection;

namespace VirtualList.WinUi.ViewModels
{
    internal class MainViewModel : ObservableRecipient
    {
        private readonly ModelVirtualCollection items;
        private IAsyncRelayCommand searchCommand;

        public MainViewModel()
        {
            items = new ModelVirtualCollection();
            Task.Run(async () => await items.LoadAsync());
        }

        public IList<Model> Items => items;

        public string SearchString { get; set; }

        public IAsyncRelayCommand SearchCommand => searchCommand ??= new AsyncRelayCommand(async () =>
                await Task.CompletedTask);
    }
}

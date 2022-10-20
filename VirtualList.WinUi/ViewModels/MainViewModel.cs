using CiccioSoft.VirtualList.Data.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualList.WinUi.ViewModels
{
    internal class MainViewModel : ObservableRecipient
    {
        private readonly IList<Model> items;
        private IAsyncRelayCommand searchCommand;

        public MainViewModel()
        {
        }

        public IList<Model> Items => items;

        public string SearchString { get; set; }

        public IAsyncRelayCommand SearchCommand => searchCommand ??= new AsyncRelayCommand(async () =>
                await Task.CompletedTask);
    }
}

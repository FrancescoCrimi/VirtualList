using CiccioSoft.VirtualList.Data.Domain;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public class MainViewModel : ObservableRecipient
    {

        //private readonly FakeCollection fakeList;
        //private readonly ModelVirtualRangeCollection items;
        private readonly ModelVirtualRangeCollection items;
        private ICommand submitCommand;
        private IAsyncRelayCommand clearCommand;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            //items = new ModelVirtualCollection();
            items = new ModelVirtualRangeCollection();
            //fakeList = new FakeCollection();
            //Items = fakeList;
        }

        public IList<Model> Items => items;

        public string SearchString { get; set; }


        public IAsyncRelayCommand ClearCommand => clearCommand ?? 
            (clearCommand = new AsyncRelayCommand(async () => await items.LoadAsync()));

        public ICommand SubmitCommand => submitCommand ?? (submitCommand = new RelayCommand(OnSubmitCommand));

 

        private void OnSubmitCommand()
        {
            //items.Load(SearchString);
        }
    }
}
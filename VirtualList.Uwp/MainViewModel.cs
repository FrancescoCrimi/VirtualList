using CiccioSoft.VirtualList.Data.Domain;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CiccioSoft.VirtualList.Uwp
{
    public class MainViewModel : ObservableRecipient
    {

        private readonly FakeCollection fakeList;
        private readonly ModelVirtualRangeCollection items;
        private ICommand submitCommand;
        private ICommand clearCommand;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            //Items = new ModelVirtualCollection();
            items = new ModelVirtualRangeCollection();
            //fakeList = new FakeCollection();
            //Items = fakeList;
            //Items = new IncrementalLoadingCollection<ModelIncrementalSource, Model>();
        }

        public IList<Model> Items => items;

        public ICommand ButtonCommand => new RelayCommand(() => fakeList.OnNotifyCollectionReset());


        public string SearchString { get; set; }



        public ICommand ClearCommand => clearCommand ?? (clearCommand = new RelayCommand(() => items.Load("")));
        public ICommand SubmitCommand => submitCommand ?? (submitCommand = new RelayCommand(onSubmitCommand));

        private void onSubmitCommand()
        {
            items.Load(SearchString);
        }
    }
}
using CiccioSoft.VirtualList.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CiccioSoft.VirtualList.Uwp
{
    public class MainViewModel : ObservableRecipient
    {

        private readonly FakeList fakeList;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            //Items = serviceProvider.GetRequiredService<ModelVirtualList>();
            //fakeList = new FakeList();
            //Items = fakeList;
            //Items = new IncrementalLoadingCollection<ModelIncrementalSource, Model>();
            Items = new ModelVirtualCollection();
        }

        public IList<Model> Items { get; set; }

        public ICommand ButtonCommand => new RelayCommand(() => fakeList.OnNotifyCollectionReset());
    }
}
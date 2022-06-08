using CiccioSoft.VirtualList.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CiccioSoft.VirtualList.Uwp
{
    public class MainViewModel : ObservableRecipient
    {
        private readonly ModelVirtualList modelVirtualList;
        private readonly NewModelVirtualList newModelVirtualList;
        private readonly FakeList fakeList;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            modelVirtualList = serviceProvider.GetRequiredService<ModelVirtualList>();
            //fakeList = new FakeList();
            //var list = new IncrementalLoadingCollection<ModelIncrementalSource, Model>();
            //newModelVirtualList = new NewModelVirtualList();
            Items = modelVirtualList;
        }

        public IList<Model> Items { get; set; }

        public ICommand ButtonCommand => new RelayCommand(() => fakeList.OnNotifyCollectionReset());
    }
}
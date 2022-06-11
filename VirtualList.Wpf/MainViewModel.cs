using CiccioSoft.VirtualList.Wpf.Collection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace CiccioSoft.VirtualList.Wpf
{
    public class MainViewModel : ObservableRecipient
    {
        public MainViewModel(IServiceProvider serviceProvider)
        {
            var list = new ModelVirtualCollection();
            var list2 = new ModelVirtualCollection();
            //Items3 = new DataGridCollectionView();
            Items = list;
            Items2 = list2;
        }

        public ModelVirtualCollection? Items { get; set; }
        public ModelVirtualCollection? Items2 { get; set; }
        public DataGridCollectionView? Items3 { get; set; }
    }
}
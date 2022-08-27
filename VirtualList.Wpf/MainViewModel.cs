using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Wpf.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace CiccioSoft.VirtualList.Wpf
{
    public class MainViewModel : ObservableRecipient
    {
        public MainViewModel(IServiceProvider serviceProvider)
        {
            Items = new ModelVirtualCollection();
            Items2 = new ModelVirtualCollection();
            //Items3 = new DataGridCollectionView();
        }

        public IList<Model>? Items { get; set; }
        public IList<Model>? Items2 { get; set; }
        public DataGridCollectionView? Items3 { get; set; }
    }
}
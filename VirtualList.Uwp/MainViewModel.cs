using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections;

namespace CiccioSoft.VirtualList.Uwp
{
    public class MainViewModel : ObservableRecipient
    {
        public MainViewModel(IServiceProvider serviceProvider)
        {
            //var list = serviceProvider.GetRequiredService<ModelVirtualList>();
            var list = new FakeList();
            //var list = new IncrementalLoadingCollection<ModelIncrementalSource, Model>();
            //var list = new NewModelVirtualList();
            Items = list;
        }

        public IList Items { get; set; }
    }
}
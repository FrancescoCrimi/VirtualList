using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CiccioSoft.VirtualList.Wpf
{
    public class MainViewModel : ObservableRecipient
    {
        private readonly ILogger<MainViewModel> logger;

        public MainViewModel(ILogger<MainViewModel> logger,
                             ILoggerFactory loggerFactory,
                             IServiceProvider serviceProvider)
        {
            this.logger = logger;
            var list = new ModelVirtualList(loggerFactory,
                                             serviceProvider);
            //var list2 = new ModelVirtualList(loggerFactory,
            //                                 serviceProvider);
            //Items3 = new DataGridCollectionView();
            Items = list;
            //Items2 = list2;
        }

        public ModelVirtualList? Items { get; set; }
        public ModelVirtualList? Items2 { get; set; }
        public DataGridCollectionView? Items3 { get; set; }
    }
}
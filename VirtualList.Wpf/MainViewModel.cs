using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CiccioSoft.VirtualList.Wpf
{
    public class MainViewModel : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (!(object.Equals(field, newValue)))
            {
                field = (newValue);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }
    }
}
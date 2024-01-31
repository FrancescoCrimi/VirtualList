using CiccioSoft.VirtualList.Sample.Uwp.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Uwp
{
    public class MainViewModel : ObservableRecipient
    {
        private AsyncRelayCommand<string> searchCommand;

        public MainViewModel()
        {
            Items = new ModelVirtualCollection();
        }

        public async Task LoadAsync()
            => await Items.LoadAsync("");

        public ModelVirtualCollection Items { get; private set; }

        public IAsyncRelayCommand<string> SearchCommand
            => searchCommand ?? (searchCommand = new AsyncRelayCommand<string>(async (searchString)
                => await Items.LoadAsync(searchString)));
    }
}

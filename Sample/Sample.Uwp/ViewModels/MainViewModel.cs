// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

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
            => Items = new ModelVirtualCollection();

        public ModelVirtualCollection Items { get; }

        public Task LoadAsync(string searchString = "")
        {
            var _searchString = searchString ?? string.Empty;
            return Items.LoadAsync(_searchString);
        }

        public IAsyncRelayCommand<string> SearchCommand =>       
            searchCommand ?? (searchCommand = new AsyncRelayCommand<string>(async (searchString) =>
            {
                var _searchString = searchString ?? string.Empty;
                await Items.LoadAsync(searchString);
            }));
    }
}

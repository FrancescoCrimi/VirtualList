using CiccioSoft.VirtualList.Sample.WinUi.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.WinUi.ViewModels;

public partial class ItemsViewViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string? _searchString;

    public ItemsViewViewModel()
    {
        Items = new FakeVirtualCollection();
        Task.Run(async () => await Items.LoadAsync());
    }

    public FakeVirtualCollection Items { get; }

    [RelayCommand]
    private async Task OnSearch()
        => await Items.LoadAsync();
}

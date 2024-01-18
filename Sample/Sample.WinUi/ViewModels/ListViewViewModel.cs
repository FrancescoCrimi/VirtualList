using CiccioSoft.VirtualList.Sample.WinUi.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.WinUi.ViewModels;

public partial class ListViewViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string? _searchString;

    public ListViewViewModel()
    {
        Items = new ModelVirtualCollection();
        Task.Run(async () => await Items.LoadAsync());
    }

    public ModelVirtualCollection Items { get; }

    [RelayCommand]
    private async Task OnSearch()
        => await Task.CompletedTask;
}

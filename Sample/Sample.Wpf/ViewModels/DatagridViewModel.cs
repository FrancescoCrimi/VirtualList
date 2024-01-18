using CiccioSoft.VirtualList.Sample.Wpf.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Wpf.ViewModels;

public partial class DatagridViewModel : ObservableObject
{
    private readonly ModelVirtualCollection items = new();

    public async Task LoadAsync()
        => await items.LoadAsync(SearchString);

    public ModelVirtualCollection Items => items;

    [ObservableProperty]
    private string? _searchString;

    /// <summary>
    /// Non usato perche si sta usando il code behind.
    /// Per usare questa ICommand provare ad implementare o una "attached property" 
    /// o una "attached behavior"
    /// https://stackoverflow.com/questions/4793030/wpf-reset-listbox-scroll-position-when-itemssource-changes
    /// </summary>
    [RelayCommand]
    private async Task OnSearch()
        => await items.LoadAsync(SearchString);
}

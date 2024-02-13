using CiccioSoft.VirtualList.Sample.WinUi.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.WinUi.ViewModels;

public partial class ListViewViewModel : ObservableRecipient
{
    public ListViewViewModel()
        => Items = new ModelVirtualCollection();

    public ModelVirtualCollection Items { get; }

    internal Task LoadAsync()
        => Items.LoadAsync("");

    [RelayCommand]
    private Task OnSearch(string searchString)
        => Items.LoadAsync(searchString);
}

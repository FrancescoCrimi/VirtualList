using CiccioSoft.VirtualList.Sample.Domain;
using CiccioSoft.VirtualList.Sample.Wpf.Collection;
using CiccioSoft.VirtualList.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Wpf.ViewModels;

public class ListViewViewModel : ObservableObject
{
    private readonly ModelVirtualCollection items = new();
    //private readonly FakeCollection items = new();
    private string searchString = string.Empty;
    private AsyncRelayCommand? searchCommand;

    public async Task LoadAsync()
        => await items.LoadAsync();

    public async Task SearchAsync()
        => await items.LoadAsync(searchString);

    public IVirtualCollection<Model> Items => items;

    public string SearchString
    {
        get => searchString;
        set => SetProperty(ref searchString, value);
    }

    public IAsyncRelayCommand SearchCommand => searchCommand ??= new AsyncRelayCommand(async () =>
    {
        await items.LoadAsync(SearchString);
    });
}
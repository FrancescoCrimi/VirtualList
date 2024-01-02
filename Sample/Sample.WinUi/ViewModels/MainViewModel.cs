using System.Collections.Generic;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Sample.Domain;
using CiccioSoft.VirtualList.Sample.WinUi.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CiccioSoft.VirtualList.Sample.WinUi.ViewModels;

internal class MainViewModel : ObservableRecipient
{
    private readonly ModelVirtualCollection items;
    private IAsyncRelayCommand? searchCommand;

    public MainViewModel()
    {
        items = new ModelVirtualCollection();
        Task.Run(async () => await items.LoadAsync());
    }

    public IList<Model> Items => items;

    public string? SearchString
    {
        get; set;
    }

    public IAsyncRelayCommand SearchCommand =>
        searchCommand ??= new AsyncRelayCommand(async () =>
            await Task.CompletedTask);
}

﻿using CiccioSoft.VirtualList.Sample.Wpf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CiccioSoft.VirtualList.Sample.Wpf.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private readonly Frame frame;
    private AsyncRelayCommand? openDataGridCommand;
    private AsyncRelayCommand? openListViewCommand;

    public MainViewModel(Frame frame)
    {
        this.frame = frame;
    }

    public IAsyncRelayCommand OpenDataGridCommand => openDataGridCommand ??= new AsyncRelayCommand(async () =>
    {
        frame.Navigate(new DatagridView());
        await Task.CompletedTask;
    });

    public IAsyncRelayCommand OpenListViewCommand => openListViewCommand ??= new AsyncRelayCommand(async () =>
    {
        frame.Navigate(new ListViewView());
        await Task.CompletedTask;
    });
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MusicLyricApp.Core.Service;
using MusicLyricApp.ViewModels;

namespace MusicLyricApp.Views;

public partial class MainWindow : Window, IWindowProvider
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this);
        
        SearchTextBox.PointerEntered += SearchTextBox_PointerEntered;
    }
    
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        if (DataContext is not MainWindowViewModel vm) return;
        
        // 阻止当前关闭流程，异步处理确认
        e.Cancel = true;
        
        var box = MessageBoxManager.GetMessageBoxStandard("退出提示", "你确定要退出吗？", ButtonEnum.YesNo);
        box.ShowAsync().ContinueWith(task =>
        {
            if (task.Result == ButtonResult.Yes)
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                {
                    vm.SaveConfig();
                    lifetime.Shutdown();
                }
            }
        });
    }

    public async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
    {
        return await StorageProvider.OpenFolderPickerAsync(options);
    }

    public async Task SetTextAsync(string? text)
    {
        await Clipboard?.SetTextAsync(text)!;
    }
    
    private async void SearchTextBox_PointerEntered(object? sender, PointerEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;

        if (!vm.SettingBean.Config.AutoReadClipboard) return;
        
        var message = await Clipboard?.GetTextAsync()!;
        if (message != null)
        {
            vm.SearchParamViewModel.SearchText = message;
        }
    }
}
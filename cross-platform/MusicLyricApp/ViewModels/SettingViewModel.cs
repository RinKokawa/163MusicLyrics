using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicLyricApp.Models;

namespace MusicLyricApp.ViewModels;

public partial class SettingViewModel : ViewModelBase
{
    [ObservableProperty] private string _settingTips = "这里显示设置说明或帮助信息";

    public ObservableCollection<LyricsTypeEnumModel> LyricsTypes { get; } = [];

    [ObservableProperty] private LyricsTypeEnumModel? _selectedLyricsTypeItem;

    public SettingParamViewModel SettingParamViewModel { get; } = new();

    private readonly SettingBean _settingBean;

    public SettingViewModel(SettingBean settingBean)
    {
        _settingBean = settingBean;
        SettingParamViewModel.Bind(_settingBean);

        InitLyricsTypes();
    }

    private void InitLyricsTypes()
    {
        var selected = _settingBean.Config.DeserializationOutputLyricsTypes();

        foreach (var e in selected)
        {
            LyricsTypes.Add(new LyricsTypeEnumModel(e)
            {
                IsSelected = true
            });
        }

        foreach (var e in Enum.GetValues<LyricsTypeEnum>())
        {
            if (!selected.Contains(e))
            {
                LyricsTypes.Add(new LyricsTypeEnumModel(e));
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanMoveUp))]
    private void MoveUp()
    {
        var index = LyricsTypes.IndexOf(SelectedLyricsTypeItem!);
        if (index <= 0) return;

        LyricsTypes.Move(index, index - 1);
        ForceRefreshLyrics();
    }

    [RelayCommand(CanExecute = nameof(CanMoveDown))]
    private void MoveDown()
    {
        var index = LyricsTypes.IndexOf(SelectedLyricsTypeItem!);
        if (index >= LyricsTypes.Count - 1 || index < 0) return;

        LyricsTypes.Move(index, index + 1);
        ForceRefreshLyrics();
    }

    private bool CanMoveUp() => SelectedLyricsTypeItem != null && LyricsTypes.IndexOf(SelectedLyricsTypeItem) > 0;

    private bool CanMoveDown() => SelectedLyricsTypeItem != null &&
                                  LyricsTypes.IndexOf(SelectedLyricsTypeItem) < LyricsTypes.Count - 1;

    private void ForceRefreshLyrics()
    {
        var copy = new ObservableCollection<LyricsTypeEnumModel>(LyricsTypes);
        LyricsTypes.Clear();
        foreach (var item in copy)
            LyricsTypes.Add(item);
    }

    public void OnClosing()
    {
        _settingBean.Config.OutputLyricTypes = string.Join(",", LyricsTypes
            .Where(x => x.IsSelected)
            .Select(x => x.Id));
    }

    [RelayCommand]
    private void TimestampTips()
    {
        SettingTips = Constants.HelpTips.GetContent(Constants.HelpTips.TypeEnum.TIME_STAMP_SETTING);
    }

    [RelayCommand]
    private void OutputTips()
    {
        SettingTips = Constants.HelpTips.GetContent(Constants.HelpTips.TypeEnum.OUTPUT_SETTING);
    }
}
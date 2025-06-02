using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MusicLyricApp.Core.Utils;
using MusicLyricApp.Models;

namespace MusicLyricApp.ViewModels;

public partial class SettingParamViewModel : ViewModelBase
{
    // 1. 毫秒截位规则
    public ObservableCollection<EnumDisplayHelper.EnumDisplayItem<DotTypeEnum>> DotTypes { get; } =
        EnumDisplayHelper.GetEnumDisplayCollection<DotTypeEnum>();
    
    [ObservableProperty]
    private EnumDisplayHelper.EnumDisplayItem<DotTypeEnum> _selectedDotTypeItem;

    public DotTypeEnum SelectedDotType => SelectedDotTypeItem?.Value ?? default;
    
    partial void OnSelectedDotTypeItemChanged(
        EnumDisplayHelper.EnumDisplayItem<DotTypeEnum>? oldValue,
        EnumDisplayHelper.EnumDisplayItem<DotTypeEnum>? newValue)
    {
        if (newValue != null) _settingBean.Config.DotType = newValue.Value;
    }
    
    // 2. 译文缺省规则
    public ObservableCollection<EnumDisplayHelper.EnumDisplayItem<TransLyricLostRuleEnum>> TransLyricLostRules { get; } =
        EnumDisplayHelper.GetEnumDisplayCollection<TransLyricLostRuleEnum>();
    
    [ObservableProperty]
    private EnumDisplayHelper.EnumDisplayItem<TransLyricLostRuleEnum> _selectedTransLyricLostRuleItem;

    public TransLyricLostRuleEnum SelectedTransLyricLostRule => SelectedTransLyricLostRuleItem?.Value ?? default;
    
    partial void OnSelectedTransLyricLostRuleItemChanged(
        EnumDisplayHelper.EnumDisplayItem<TransLyricLostRuleEnum>? oldValue,
        EnumDisplayHelper.EnumDisplayItem<TransLyricLostRuleEnum>? newValue)
    {
        if (newValue != null) _settingBean.Config.TransConfig.LostRule = newValue.Value;
    }
    
    // 3. 中文处理策略
    public ObservableCollection<EnumDisplayHelper.EnumDisplayItem<ChineseProcessRuleEnum>> ChineseProcessRules { get; } =
        EnumDisplayHelper.GetEnumDisplayCollection<ChineseProcessRuleEnum>();
    
    [ObservableProperty]
    private EnumDisplayHelper.EnumDisplayItem<ChineseProcessRuleEnum> _selectedChineseProcessRuleItem;

    public ChineseProcessRuleEnum SelectedChineseProcessRule => SelectedChineseProcessRuleItem?.Value ?? default;
    
    partial void OnSelectedChineseProcessRuleItemChanged(
        EnumDisplayHelper.EnumDisplayItem<ChineseProcessRuleEnum>? oldValue,
        EnumDisplayHelper.EnumDisplayItem<ChineseProcessRuleEnum>? newValue)
    {
        if (newValue != null) _settingBean.Config.ChineseProcessRule = newValue.Value;
    }
    
    // 5. LRC 时间戳
    [ObservableProperty] private string _lrcTimestampFormat;
    
    partial void OnLrcTimestampFormatChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.LrcTimestampFormat = newValue;
    }
    
    // 6. SRT 时间戳
    [ObservableProperty] private string _srtTimestampFormat;
    
    partial void OnSrtTimestampFormatChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.SrtTimestampFormat = newValue;
    }
    
    // 7. SRT 时间戳
    [ObservableProperty] private bool _ignoreEmptyLyric;
    
    partial void OnIgnoreEmptyLyricChanged(bool oldValue, bool newValue)
    {
        _settingBean.Config.IgnoreEmptyLyric = newValue;
    }
    
    // 8. SRT 时间戳
    [ObservableProperty] private bool _enableVerbatimLyric;
    
    partial void OnEnableVerbatimLyricChanged(bool oldValue, bool newValue)
    {
        _settingBean.Config.EnableVerbatimLyric = newValue;
    }
    
    // 9. 译文匹配精度
    [ObservableProperty] private int _matchPrecisionDeviation;
    
    partial void OnMatchPrecisionDeviationChanged(int oldValue, int newValue)
    {
        _settingBean.Config.TransConfig.MatchPrecisionDeviation = newValue;
    }

    // 10. 百度 APP ID
    [ObservableProperty] private string _baiduAppId;
    
    partial void OnBaiduAppIdChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.TransConfig.BaiduTranslateAppId = newValue;
    }
    
    // 11. 百度密钥
    [ObservableProperty] private string _baiduSecret;
    
    partial void OnBaiduSecretChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.TransConfig.BaiduTranslateSecret = newValue;
    }
    
    // 12. 彩云小译 Token
    [ObservableProperty] private string _caiYunToken;
    
    partial void OnCaiYunTokenChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.TransConfig.CaiYunToken = newValue;
    }
    
    // 13. 跳过纯音乐
    [ObservableProperty] private bool _ignorePureMusicInSave;
    
    partial void OnIgnorePureMusicInSaveChanged(bool oldValue, bool newValue)
    {
        _settingBean.Config.IgnorePureMusicInSave = newValue;
    }
    
    // 14. 独立歌词格式分文件保存
    [ObservableProperty] private bool _separateFileForIsolated;
    
    partial void OnSeparateFileForIsolatedChanged(bool oldValue, bool newValue)
    {
        _settingBean.Config.SeparateFileForIsolated = newValue;
    }
    
    // 15. 保存文件名
    [ObservableProperty] private string _outputFileNameFormat;
    
    partial void OnOutputFileNameFormatChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.OutputFileNameFormat = newValue;
    }
    
    // 16. 聚合模糊搜索
    [ObservableProperty] private bool _aggregatedBlurSearch;
    
    partial void OnAggregatedBlurSearchChanged(bool oldValue, bool newValue)
    {
        _settingBean.Config.AggregatedBlurSearch = newValue;
    }
    
    // 17. 自动读取剪切板
    [ObservableProperty] private bool _autoReadClipboard;
    
    partial void OnAutoReadClipboardChanged(bool oldValue, bool newValue)
    {
        _settingBean.Config.AutoReadClipboard = newValue;
    }
    
    // 18. 自动检查更新
    [ObservableProperty] private bool _autoCheckUpdate;
    
    partial void OnAutoCheckUpdateChanged(bool oldValue, bool newValue)
    {
        _settingBean.Config.AutoCheckUpdate = newValue;
    }
    
    // 19. QQ音乐 Cookie
    [ObservableProperty] private string _qqMusicCookie;
    
    partial void OnQqMusicCookieChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.QQMusicCookie = newValue;
    }
    
    // 20. 网易云 Cookie
    [ObservableProperty] private string _netEaseCookie;
    
    partial void OnNetEaseCookieChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.NetEaseCookie = newValue;
    }
    
    // 21. 歌手分隔符
    [ObservableProperty] private string _singerSeparator;
    
    partial void OnSingerSeparatorChanged(string? oldValue, string newValue)
    {
        _settingBean.Config.SingerSeparator = newValue;
    }
    
    private SettingBean _settingBean;

    public void Bind(SettingBean settingBean)
    {
        _settingBean = settingBean;
        
        SelectedDotTypeItem = DotTypes.First(item => Equals(item.Value, _settingBean.Config.DotType));
        SelectedTransLyricLostRuleItem = TransLyricLostRules.First(item => Equals(item.Value, _settingBean.Config.TransConfig.LostRule));
        SelectedChineseProcessRuleItem = ChineseProcessRules.First(item => Equals(item.Value, _settingBean.Config.ChineseProcessRule));
        LrcTimestampFormat = _settingBean.Config.LrcTimestampFormat;
        SrtTimestampFormat = _settingBean.Config.SrtTimestampFormat;
        IgnoreEmptyLyric = _settingBean.Config.IgnoreEmptyLyric;
        EnableVerbatimLyric = _settingBean.Config.EnableVerbatimLyric;
        MatchPrecisionDeviation = _settingBean.Config.TransConfig.MatchPrecisionDeviation;
        BaiduAppId = _settingBean.Config.TransConfig.BaiduTranslateAppId;
        BaiduSecret =  _settingBean.Config.TransConfig.BaiduTranslateSecret;
        CaiYunToken = _settingBean.Config.TransConfig.CaiYunToken;
        IgnorePureMusicInSave = _settingBean.Config.IgnorePureMusicInSave;
        SeparateFileForIsolated = _settingBean.Config.SeparateFileForIsolated;
        OutputFileNameFormat = _settingBean.Config.OutputFileNameFormat;
        AggregatedBlurSearch = _settingBean.Config.AggregatedBlurSearch;
        AutoReadClipboard = _settingBean.Config.AutoReadClipboard;
        AutoCheckUpdate = _settingBean.Config.AutoCheckUpdate;
        QqMusicCookie = _settingBean.Config.QQMusicCookie;
        NetEaseCookie = _settingBean.Config.NetEaseCookie;
        SingerSeparator = _settingBean.Config.SingerSeparator;
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using MusicLyricApp.Models;

namespace MusicLyricApp.ViewModels;

public partial class SignalLampViewModel : ObservableObject
{
    [ObservableProperty] private IBrush _lampColor = Brushes.Gray;

    [ObservableProperty] private string _details = "";

    public void UpdateLampInfo(Dictionary<string, ResultVo<SaveVo>> resDict, SettingBean settingBean)
    {
        var outputTypes = settingBean.Config.DeserializationOutputLyricsTypes();
        var supportTransliteration = outputTypes.Contains(LyricsTypeEnum.TRANSLITERATION);

        var outputDict = new Dictionary<string, int>
        {
            [LyricsTypeEnum.ORIGIN.ToDescription()] = 0,
            [LyricsTypeEnum.ORIGIN_TRANS.ToDescription()] = 0
        };
        if (supportTransliteration)
        {
            outputDict[LyricsTypeEnum.TRANSLITERATION.ToDescription()] = 0;
        }

        foreach (var lyricVo in from pair in resDict
                 select pair.Value
                 into vo
                 where vo.IsSuccess()
                 select vo.Data.LyricVo)
        {
            if (!string.IsNullOrEmpty(lyricVo.Lyric))
            {
                outputDict[LyricsTypeEnum.ORIGIN.ToDescription()]++;
            }

            if (!string.IsNullOrEmpty(lyricVo.TranslateLyric))
            {
                outputDict[LyricsTypeEnum.ORIGIN_TRANS.ToDescription()]++;
            }

            if (supportTransliteration && !string.IsNullOrEmpty(lyricVo.TransliterationLyric))
            {
                outputDict[LyricsTypeEnum.TRANSLITERATION.ToDescription()]++;
            }
        }

        var totalCnt = outputDict.Values.Sum();
        if (totalCnt == 0)
        {
            LampColor = Brushes.Red;
        }
        else if (totalCnt == outputDict.Keys.Count * resDict.Count)
        {
            LampColor = Brushes.LimeGreen;
        }
        else
        {
            LampColor = Brushes.Chocolate;
        }

        var sb = new StringBuilder();
        foreach (var pair in outputDict)
        {
            sb.Append($"{pair.Key}: {pair.Value}\t");
        }

        Details = sb.ToString();
    }
}
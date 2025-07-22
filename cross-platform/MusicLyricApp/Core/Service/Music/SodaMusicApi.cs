using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MusicLyricApp.Core.Utils;
using MusicLyricApp.Models;

namespace MusicLyricApp.Core.Service.Music;

public class SodaMusicApi : MusicCacheableApi
{
    protected override SearchSourceEnum Source0() => SearchSourceEnum.SODA_MUSIC;

    protected override ResultVo<PlaylistVo> GetPlaylistVo0(string playlistId)
    {
        return ResultVo<PlaylistVo>.Failure(ErrorMsgConst.FUNCTION_NOT_SUPPORT);
    }

    protected override ResultVo<AlbumVo> GetAlbumVo0(string albumId)
    {
        return ResultVo<AlbumVo>.Failure(ErrorMsgConst.FUNCTION_NOT_SUPPORT);
    }

    protected override Dictionary<string, ResultVo<SongVo>> GetSongVo0(string[] songIds)
    {
        // 只支持单曲，songIds[0] 为分享链接
        var result = new Dictionary<string, ResultVo<SongVo>>();
        foreach (var songId in songIds)
        {
            var info = GetSodaMusicInfo(songId);
            if (info != null)
            {
                result[songId] = new ResultVo<SongVo>(info.Item1);
            }
            else
            {
                result[songId] = ResultVo<SongVo>.Failure(ErrorMsgConst.SONG_NOT_EXIST);
            }
        }
        return result;
    }

    protected override ResultVo<string> GetSongLink0(string songId)
    {
        var info = GetSodaMusicInfo(songId);
        if (info != null)
        {
            return new ResultVo<string>(info.Item1.Pics); // 用 Pics 字段临时存放音频直链
        }
        return ResultVo<string>.Failure(ErrorMsgConst.SONG_URL_GET_FAILED);
    }

    protected override ResultVo<LyricVo> GetLyricVo0(string id, string displayId, bool isVerbatim)
    {
        var info = GetSodaMusicInfo(id);
        if (info != null)
        {
            return new ResultVo<LyricVo>(info.Item2);
        }
        return ResultVo<LyricVo>.Failure(ErrorMsgConst.LRC_NOT_EXIST);
    }

    protected override ResultVo<SearchResultVo> Search0(string keyword, SearchTypeEnum searchType)
    {
        // 只支持单曲分享链接
        var info = GetSodaMusicInfo(keyword);
        if (info != null)
        {
            var result = new SearchResultVo
            {
                SearchSource = SearchSourceEnum.SODA_MUSIC,
                SearchType = SearchTypeEnum.SONG_ID
            };
            result.SongVos.Add(new SearchResultVo.SongSearchResultVo
            {
                DisplayId = keyword,
                Title = info.Item1.Name,
                AuthorName = info.Item1.Singer,
                AlbumName = info.Item1.Album,
                Duration = info.Item1.Duration
            });
            return new ResultVo<SearchResultVo>(result);
        }
        return ResultVo<SearchResultVo>.Failure(ErrorMsgConst.SEARCH_RESULT_EMPTY);
    }

    /// <summary>
    /// 获取汽水音乐信息，返回 (SongVo, LyricVo)
    /// </summary>
    private static Tuple<SongVo, LyricVo> GetSodaMusicInfo(string shareUrl)
    {
        try
        {
            // 在线获取页面源码，模拟浏览器UA，自动重定向
            var handler = new System.Net.Http.HttpClientHandler { AllowAutoRedirect = true };
            using var client = new System.Net.Http.HttpClient(handler);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
            var html = client.GetStringAsync(shareUrl).Result;

            // 提取 _ROUTER_DATA JSON
            var match = Regex.Match(html, "_ROUTER_DATA\\s*=\\s*({.*?});", RegexOptions.Singleline);
            if (!match.Success) return null;
            var json = match.Groups[1].Value;
            var obj = JsonUtils.ToJObject(json);
            var option = obj["loaderData"]?["track_page"]?["audioWithLyricsOption"];
            if (option == null) return null;

            var songVo = new SongVo
            {
                Id = option["track_id"]?.ToString() ?? "",
                DisplayId = shareUrl,
                Name = option["trackName"]?.ToString() ?? "",
                Singer = new[] { option["artistName"]?.ToString() ?? "" },
                Album = option.Parent?["trackInfo"]?["album"]?["name"]?.ToString() ?? "",
                Duration = (long)((option["duration"]?.ToObject<double>() ?? 0) * 1000),
                Pics = option["url"]?.ToString() ?? ""
            };

            List<string> lyricLines = new List<string>();
            var sentences = option["lyrics"]?["sentences"];
            if (sentences != null)
            {
                foreach (var l in sentences)
                {
                    var text = l["text"]?.ToString();
                    if (!string.IsNullOrEmpty(text))
                        lyricLines.Add(text);
                }
            }
            var lyricVo = new LyricVo
            {
                SearchSource = SearchSourceEnum.SODA_MUSIC,
                Lyric = string.Join("\n", lyricLines),
                Duration = songVo.Duration
            };
            return Tuple.Create(songVo, lyricVo);
        }
        catch (Exception ex)
        {
            // 可选：输出详细日志，便于排查
            System.Diagnostics.Debug.WriteLine($"[SodaMusicApi] 获取失败: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }
} 
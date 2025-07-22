import requests
import re
import json

# 请求短链接，自动跟随跳转
url = "https://qishui.douyin.com/s/imf2hxgy/"
response = requests.get(url, allow_redirects=True)

# 提取 HTML 中 _ROUTER_DATA 的 JSON 字符串
match = re.search(r'_ROUTER_DATA\s*=\s*({.*?});', response.text, re.DOTALL)
if not match:
    print("未找到歌词数据")
    exit()

router_data = json.loads(match.group(1))

# 提取 audioWithLyricsOption 字段
audio_with_lyrics_option = router_data["loaderData"]["track_page"]["audioWithLyricsOption"]

# 只保留常用字段
fields = [
    "track_id", "trackName", "artistName", "duration", "url", "lyrics", "songMakerTeamSentences",
    "backgroundColor", "gradientBackgroundColor", "vid", "hasCopyright", "status_code", "artistIdStr", "kid"
]
filtered = {k: audio_with_lyrics_option[k] for k in fields if k in audio_with_lyrics_option}

print(json.dumps(filtered, ensure_ascii=False, indent=2))

# 如需只输出歌词，可取消注释以下代码
# lyrics = audio_with_lyrics_option["lyrics"]["sentences"]
# for line in lyrics:
#     print(line["text"])

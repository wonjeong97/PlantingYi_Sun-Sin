using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class WhatIsSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public VideoSetting video;
}

public class WhatIsPage : BasePage<WhatIsSetting>
{
    protected override string JsonPath => "JSON/WhatIsSetting.json";

    private GameObject videoPlayer;

    protected override async Task BuildContentAsync()
    {
        // 페이지 전용: 비디오 플레이어 생성
        videoPlayer = await UIManager.Instance.CreateVideoPlayerAsync(
            Setting.video, gameObject, default(CancellationToken));
    }

    private void OnEnable()
    {
        // 다시 활성화될 때 항상 첫 프레임부터 재생
        if (videoPlayer != null && videoPlayer.TryGetComponent<VideoPlayer>(out var vp))
        {
            StartCoroutine(VideoManager.Instance.RestartFromStart(vp));
        }
    }
}

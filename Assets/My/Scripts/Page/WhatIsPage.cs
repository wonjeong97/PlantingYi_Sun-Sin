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
      //    videoPlayer = await UIManager.Instance.CreateVideoPlayerAsync(
      //      setting.video, gameObject, CancellationToken.None);
    }

    private void OnEnable()
    {
        // 페이지 활성화 시 비디오를 첫 번째 프레임부터 시작함
        if (videoPlayer != null && videoPlayer.TryGetComponent<VideoPlayer>(out var vp))
        {
            StartCoroutine(VideoManager.Instance.RestartFromStart(vp));
        }
    }
}
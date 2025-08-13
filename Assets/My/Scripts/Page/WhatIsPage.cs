using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[Serializable]
public class WhatIsSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public VideoSetting video;
}

public class WhatIsPage : MonoBehaviour, IUICreate
{
    [NonSerialized] public WhatIsSetting whatIsSetting;

    [HideInInspector] public MenuPage menuPageInstance;

    private GameObject videoPlayer;

    private async void Start()
    {
        whatIsSetting = JsonLoader.Instance.LoadJsonData<WhatIsSetting>("JSON/WhatIsSetting.json");
        if (whatIsSetting != null)
        {
            try
            {
                await CreateUI();
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[WhatIsPage] Start canceled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[WhatIsPage] Start failed: {e}");
                throw;
            }
        }
    }

    public async Task CreateUI()
    {
        await UIManager.Instance.CreateBackgroundImageAsync(whatIsSetting.backgroundImage, gameObject, default);

        var (backToIdleButton, _) = await UIManager.Instance.CreateSingleButtonAsync(whatIsSetting.backToIdleButton, gameObject, default);
        if (backToIdleButton != null && backToIdleButton.TryGetComponent<Button>(out var idleBtn))
        {
            idleBtn.onClick.AddListener(async () =>
            {
                await UIManager.Instance.ClearAllDynamic();
            });
        }

        var (backButton, _) = await UIManager.Instance.CreateSingleButtonAsync(whatIsSetting.backButton, gameObject, default);
        if (backButton != null && backButton.TryGetComponent<Button>(out var backBtn))
        {
            backBtn.onClick.AddListener(async () =>
            {
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);
                if (menuPageInstance != null)
                {
                    menuPageInstance.gameObject.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }
            });
        }

        videoPlayer = await UIManager.Instance.CreateVideoPlayerAsync(whatIsSetting.video, gameObject, default);
    }

    private void OnEnable()
    {
        if (videoPlayer != null && videoPlayer.TryGetComponent<VideoPlayer>(out var vp))
        {   
            StartCoroutine(VideoManager.Instance.RestartFromStart(vp));            
        }
    }
}

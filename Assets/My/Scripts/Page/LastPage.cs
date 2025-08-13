using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LastPageSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting lastImage;
    public ImageSetting lastExplainImage;

    public ButtonSetting last_1;
    public ButtonSetting last_2;
    public ButtonSetting last_3;
    public ButtonSetting last_4;
    public ButtonSetting last_5;
    public ButtonSetting last_6;
    public ButtonSetting last_7;
    public ButtonSetting last_8;
    public ButtonSetting last_9;
}

public class LastPage : MonoBehaviour, IUICreate
{
    [NonSerialized] public LastPageSetting lastSetting;
    [HideInInspector] public MenuPage menuPageInstance;

    private async void Start()
    {
        lastSetting = JsonLoader.Instance.LoadJsonData<LastPageSetting>("JSON/LastSetting.json");
        if (lastSetting != null)
        {
            try
            {
                await CreateUI();
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[LastPage] Start canceled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[LastPage] Start failed: {e}");
                throw;
            }
        }
    }

    public async Task CreateUI()
    {
        await UIManager.Instance.CreateBackgroundImageAsync(lastSetting.backgroundImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(lastSetting.lastImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(lastSetting.lastExplainImage, gameObject, default);

        var (backToIdleButton, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.backToIdleButton, gameObject, default);
        if (backToIdleButton != null && backToIdleButton.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(async () =>
            {
                await UIManager.Instance.ClearAllDynamic();
            });
        }

        var (backButton, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.backButton, gameObject, default);
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

        await CreatePageButton();
    }

    private async Task CreatePageButton()
    {
        var (last_1, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_1, gameObject, default);
        if (last_1 != null && last_1.TryGetComponent<Button>(out var button1))
        {
            button1.onClick.AddListener(() =>
            {
                Debug.Log("Last 1 button clicked");
            });
        }

        var (last_2, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_2, gameObject, default);
        if (last_2 != null && last_2.TryGetComponent<Button>(out var button2))
        {
            button2.onClick.AddListener(() =>
            {
                Debug.Log("Last 2 button clicked");
            });
        }

        var (last_3, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_3, gameObject, default);
        if (last_3 != null && last_3.TryGetComponent<Button>(out var button3))
        {
            button3.onClick.AddListener(() =>
            {
                Debug.Log("Last 3 button clicked");
            });
        }

        var (last_4, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_4, gameObject, default);
        if (last_4 != null && last_4.TryGetComponent<Button>(out var button4))
        {
            button4.onClick.AddListener(() =>
            {
                Debug.Log("Last 4 button clicked");
            });
        }

        var (last_5, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_5, gameObject, default);
        if (last_5 != null && last_5.TryGetComponent<Button>(out var button5))
        {
            button5.onClick.AddListener(() =>
            {
                Debug.Log("Last 5 button clicked");
            });
        }

        var (last_6, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_6, gameObject, default);
        if (last_6 != null && last_6.TryGetComponent<Button>(out var button6))
        {
            button6.onClick.AddListener(() =>
            {
                Debug.Log("Last 6 button clicked");
            });
        }

        var (last_7, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_7, gameObject, default);
        if (last_7 != null && last_7.TryGetComponent<Button>(out var button7))
        {
            button7.onClick.AddListener(() =>
            {
                Debug.Log("Last 7 button clicked");
            });
        }

        var (last_8, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_8, gameObject, default);
        if (last_8 != null && last_8.TryGetComponent<Button>(out var button8))
        {
            button8.onClick.AddListener(() =>
            {
                Debug.Log("Last 8 button clicked");
            });
        }

        var (last_9, _) = await UIManager.Instance.CreateSingleButtonAsync(lastSetting.last_9, gameObject, default);
        if (last_9 != null && last_9.TryGetComponent<Button>(out var button9))
        {
            button9.onClick.AddListener(() =>
            {
                Debug.Log("Last 9 button clicked");
            });
        }
    }
}

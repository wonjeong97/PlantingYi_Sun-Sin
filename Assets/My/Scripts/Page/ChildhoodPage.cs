using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ChildhoodSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting childhoodImage;
    public ImageSetting childhoodExplainImage;

    public ButtonSetting childhood_1;
    public ButtonSetting childhood_2;
    public ButtonSetting childhood_3;
    public ButtonSetting childhood_4;
}

public class ChildhoodPage : MonoBehaviour, IUICreate
{
    [NonSerialized] public ChildhoodSetting childhoodSetting;
    [HideInInspector] public MenuPage menuPageInstance;

    private async void Start()
    {
        childhoodSetting = JsonLoader.Instance.LoadJsonData<ChildhoodSetting>("JSON/ChildhoodSetting.json");
        if (childhoodSetting != null)
        {
            try
            {
                await CreateUI();
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[ChildhoodPage] Start canceled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ChildhoodPage] Start failed: {e}");
                throw;
            }
        }
    }

    public async Task CreateUI()
    {
        await UIManager.Instance.CreateBackgroundImageAsync(childhoodSetting.backgroundImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(childhoodSetting.childhoodImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(childhoodSetting.childhoodExplainImage, gameObject, default);

        var (backToIdleButton, _) = await UIManager.Instance.CreateSingleButtonAsync(childhoodSetting.backToIdleButton, gameObject, default);
        if (backToIdleButton != null && backToIdleButton.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(async () =>
            {
                await UIManager.Instance.ClearAllDynamic();
            });
        }

        var (backButton, _) = await UIManager.Instance.CreateSingleButtonAsync(childhoodSetting.backButton, gameObject, default);
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
        var (button_1, _) = await UIManager.Instance.CreateSingleButtonAsync(childhoodSetting.childhood_1, gameObject, default);
        if (button_1 != null && button_1.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(() =>
            {
                Debug.Log("[ChildhoodPage] childhood_1 clicked.");
            });
        }

        var (button_2, _) = await UIManager.Instance.CreateSingleButtonAsync(childhoodSetting.childhood_2, gameObject, default);
        if (button_2 != null && button_2.TryGetComponent<Button>(out var button2))
        {
            button2.onClick.AddListener(() =>
            {
                Debug.Log("[ChildhoodPage] childhood_2 clicked.");
            });
        }

        var (button_3, _) = await UIManager.Instance.CreateSingleButtonAsync(childhoodSetting.childhood_3, gameObject, default);
        if (button_3 != null && button_3.TryGetComponent<Button>(out var button3))
        {
            button3.onClick.AddListener(() =>
            {
                Debug.Log("[ChildhoodPage] childhood_3 clicked.");
            });
        }

        var (button_4, _) = await UIManager.Instance.CreateSingleButtonAsync(childhoodSetting.childhood_4, gameObject, default);
        if (button_4 != null && button_4.TryGetComponent<Button>(out var button4))
        {
            button4.onClick.AddListener(() =>
            {
                Debug.Log("[ChildhoodPage] childhood_4 clicked.");
            });
        }
    }
}

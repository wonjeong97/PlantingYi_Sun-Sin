using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class YouthSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting youthImage;
    public ImageSetting youthExplainImage;

    public ButtonSetting youth_1;
    public ButtonSetting youth_2;
    public ButtonSetting youth_3;
}
public class YouthPage : MonoBehaviour, IUICreate
{
    [NonSerialized] public YouthSetting youthSetting;
    [HideInInspector] public MenuPage menuPageInstance;

    private async void Start()
    {
        youthSetting = JsonLoader.Instance.LoadJsonData<YouthSetting>("JSON/YouthSetting.json");
        if (youthSetting != null)
        {
            try
            {
                await CreateUI();
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[YouthPage] Start canceled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[YouthPage] Start failed: {e}");
                throw;
            }
        }
    }

    public async Task CreateUI()
    {
        await UIManager.Instance.CreateBackgroundImageAsync(youthSetting.backgroundImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(youthSetting.youthImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(youthSetting.youthExplainImage, gameObject, default);

        var (backToIdleButton, _) = await UIManager.Instance.CreateSingleButtonAsync(youthSetting.backToIdleButton, gameObject, default);
        if (backToIdleButton != null && backToIdleButton.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(async () =>
            {
                await UIManager.Instance.ClearAllDynamic();
            });
        }

        var (backButton, _) = await UIManager.Instance.CreateSingleButtonAsync(youthSetting.backButton, gameObject, default);
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
        var (youth_1, _) = await UIManager.Instance.CreateSingleButtonAsync(youthSetting.youth_1, gameObject, default);
        if (youth_1 != null && youth_1.TryGetComponent<Button>(out var button1))
        {
            button1.onClick.AddListener(() =>
            {
                Debug.Log("[YouthPage] youth_1 clicked.");
            });
        }

        var (youth_2, _) = await UIManager.Instance.CreateSingleButtonAsync(youthSetting.youth_2, gameObject, default);
        if (youth_2 != null && youth_2.TryGetComponent<Button>(out var button2))
        {
            button2.onClick.AddListener(() =>
            {
                Debug.Log("[YouthPage] youth_2 clicked.");
            });
        }

        var (youth_3, _) = await UIManager.Instance.CreateSingleButtonAsync(youthSetting.youth_3, gameObject, default);
        if (youth_3 != null && youth_3.TryGetComponent<Button>(out var button3))
        {
            button3.onClick.AddListener(() =>
            {
                Debug.Log("[YouthPage] youth_3 clicked.");
            });
        }
    }
}

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AfterDeathSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting afterDeathImage;
    public ImageSetting afterDeathExplainImage;

    public ButtonSetting afterDeath_1;
    public ButtonSetting afterDeath_2;
    public ButtonSetting afterDeath_3;
    public ButtonSetting afterDeath_4;
    public ButtonSetting afterDeath_5;
    public ButtonSetting afterDeath_6;
    public ButtonSetting afterDeath_7;
    public ButtonSetting afterDeath_8;
    public ButtonSetting afterDeath_9;
    public ButtonSetting afterDeath_10;
    public ButtonSetting afterDeath_11;
    public ButtonSetting afterDeath_12;
}

public class AfterDeathPage : MonoBehaviour, IUICreate
{
    [NonSerialized] public AfterDeathSetting afterDeathSetting;
    [HideInInspector] public MenuPage menuPageInstance;

    private async void Start()
    {
        afterDeathSetting = JsonLoader.Instance.LoadJsonData<AfterDeathSetting>("JSON/AfterDeathSetting.json");
        if (afterDeathSetting != null)
        {
            try
            {
                await CreateUI();
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[AfterDeathPage] Start canceled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AfterDeathPage] Start failed: {e}");
                throw;
            }
        }
    }

    public async Task CreateUI()
    {
        await UIManager.Instance.CreateBackgroundImageAsync(afterDeathSetting.backgroundImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(afterDeathSetting.afterDeathImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(afterDeathSetting.afterDeathExplainImage, gameObject, default);

        var (backToIdleButton, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.backToIdleButton, gameObject, default);
        if (backToIdleButton != null && backToIdleButton.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(async () =>
            {
                await UIManager.Instance.ClearAllDynamic();
            });
        }

        var (backButton, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.backButton, gameObject, default);
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
        var (afterDeath_1, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_1, gameObject, default);
        if (afterDeath_1 != null && afterDeath_1.TryGetComponent<Button>(out var button1))
        {
            button1.onClick.AddListener(() =>
            {
                Debug.Log("After Death 1 button clicked");
            });
        }
        
        var (afterDeath_2, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_2, gameObject, default);
        if (afterDeath_2 != null && afterDeath_2.TryGetComponent<Button>(out var button2))
        {
            button2.onClick.AddListener(() =>
            {
                Debug.Log("After Death 2 button clicked");
            });
        }

        var (afterDeath_3, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_3, gameObject, default);
        if (afterDeath_3 != null && afterDeath_3.TryGetComponent<Button>(out var button3))
        {
            button3.onClick.AddListener(() =>
            {
                Debug.Log("After Death 3 button clicked");
            });
        }

        var (afterDeath_4, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_4, gameObject, default);
        if (afterDeath_4 != null && afterDeath_4.TryGetComponent<Button>(out var button4))
        {
            button4.onClick.AddListener(() =>
            {
                Debug.Log("After Death 4 button clicked");
            });
        }

        var (afterDeath_5, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_5, gameObject, default);
        if (afterDeath_5 != null && afterDeath_5.TryGetComponent<Button>(out var button5))
        {
            button5.onClick.AddListener(() =>
            {
                Debug.Log("After Death 5 button clicked");
            });
        }

        var (afterDeath_6, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_6, gameObject, default);
        if (afterDeath_6 != null && afterDeath_6.TryGetComponent<Button>(out var button6))
        {
            button6.onClick.AddListener(() =>
            {
                Debug.Log("After Death 6 button clicked");
            });
        }

        var (afterDeath_7, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_7, gameObject, default);
        if (afterDeath_7 != null && afterDeath_7.TryGetComponent<Button>(out var button7))
        {
            button7.onClick.AddListener(() =>
            {
                Debug.Log("After Death 7 button clicked");
            });
        }

        var (afterDeath_8, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_8, gameObject, default);
        if (afterDeath_8 != null && afterDeath_8.TryGetComponent<Button>(out var button8))
        {
            button8.onClick.AddListener(() =>
            {
                Debug.Log("After Death 8 button clicked");
            });
        }

        var (afterDeath_9, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_9, gameObject, default);
        if (afterDeath_9 != null && afterDeath_9.TryGetComponent<Button>(out var button9))
        {
            button9.onClick.AddListener(() =>
            {
                Debug.Log("After Death 9 button clicked");
            });
        }

        var (afterDeath_10, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_10, gameObject, default);
        if (afterDeath_10 != null && afterDeath_10.TryGetComponent<Button>(out var button10))
        {
            button10.onClick.AddListener(() =>
            {
                Debug.Log("After Death 10 button clicked");
            });
        }

        var (afterDeath_11, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_11, gameObject, default);
        if (afterDeath_11 != null && afterDeath_11.TryGetComponent<Button>(out var button11))
        {
            button11.onClick.AddListener(() =>
            {
                Debug.Log("After Death 11 button clicked");
            });
        }

        var (afterDeath_12, _) = await UIManager.Instance.CreateSingleButtonAsync(afterDeathSetting.afterDeath_12, gameObject, default);
        if (afterDeath_12 != null && afterDeath_12.TryGetComponent<Button>(out var button12))
        {
            button12.onClick.AddListener(() =>
            {
                Debug.Log("After Death 12 button clicked");
            });
        }
    }
}

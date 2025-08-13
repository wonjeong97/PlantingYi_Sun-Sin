using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DeathSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting deathImage;
    public ImageSetting deathExplainImage;

    public ButtonSetting death_1;
    public ButtonSetting death_2;
    public ButtonSetting death_3;
    public ButtonSetting death_4;
    public ButtonSetting death_5;
    public ButtonSetting death_6;
    public ButtonSetting death_7;
    public ButtonSetting death_8; 
}

public class DeathPage : MonoBehaviour, IUICreate
{
    [NonSerialized] public DeathSetting deathSetting;
    [HideInInspector] public MenuPage menuPageInstance;

    private async void Start()
    {
        deathSetting = JsonLoader.Instance.LoadJsonData<DeathSetting>("JSON/DeathSetting.json");
        if (deathSetting != null)
        {
            try
            {
                await CreateUI();
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[DeathPage] Start canceled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DeathPage] Start failed: {e}");
                throw;
            }
        }
    }

    public async Task CreateUI()
    {
        await UIManager.Instance.CreateBackgroundImageAsync(deathSetting.backgroundImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(deathSetting.deathImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(deathSetting.deathExplainImage, gameObject, default);

        var (backToIdleButton, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.backToIdleButton, gameObject, default);
        if (backToIdleButton != null && backToIdleButton.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(async () =>
            {
                await UIManager.Instance.ClearAllDynamic();
            });
        }

        var (backButton, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.backButton, gameObject, default);
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
        var (death_1, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.death_1, gameObject, default);
        if (death_1 != null && death_1.TryGetComponent<Button>(out var button1))
        {
            button1.onClick.AddListener(() =>
            {
                Debug.Log("Death 1 button clicked");
            });
        }
        var (death_2, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.death_2, gameObject, default);
        if (death_2 != null && death_2.TryGetComponent<Button>(out var button2))
        {
            button2.onClick.AddListener(() =>
            {
                Debug.Log("Death 2 button clicked");
            });
        }

        var (death_3, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.death_3, gameObject, default);
        if (death_3 != null && death_3.TryGetComponent<Button>(out var button3))
        {
            button3.onClick.AddListener(() =>
            {
                Debug.Log("Death 3 button clicked");
            });
        }

        var (death_4, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.death_4, gameObject, default);
        if (death_4 != null && death_4.TryGetComponent<Button>(out var button4))
        {
            button4.onClick.AddListener(() =>
            {
                Debug.Log("Death 4 button clicked");
            });
        }

        var (death_5, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.death_5, gameObject, default);
        if (death_5 != null && death_5.TryGetComponent<Button>(out var button5))
        {
            button5.onClick.AddListener(() =>
            {
                Debug.Log("Death 5 button clicked");
            });
        }

        var (death_6, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.death_6, gameObject, default);
        if (death_6 != null && death_6.TryGetComponent<Button>(out var button6))
        {
            button6.onClick.AddListener(() =>
            {
                Debug.Log("Death 6 button clicked");
            });
        }

        var (death_7, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.death_7, gameObject, default);
        if (death_7 != null && death_7.TryGetComponent<Button>(out var button7))
        {
            button7.onClick.AddListener(() =>
            {
                Debug.Log("Death 7 button clicked");
            });
        }

        var (death_8, _) = await UIManager.Instance.CreateSingleButtonAsync(deathSetting.death_8, gameObject, default);
        if (death_8 != null && death_8.TryGetComponent<Button>(out var button8))
        {
            button8.onClick.AddListener(() =>
            {
                Debug.Log("Death 8 button clicked");
            });
        }
    }
}

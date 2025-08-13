using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PrimeOfLifeSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting primeOfLifeImage;
    public ImageSetting primeOfLifeExplainImage;

    public ButtonSetting primeOfLife_1;
    public ButtonSetting primeOfLife_2;
    public ButtonSetting primeOfLife_3;
    public ButtonSetting primeOfLife_4;
    public ButtonSetting primeOfLife_5;
    public ButtonSetting primeOfLife_6;
    public ButtonSetting primeOfLife_7;
    public ButtonSetting primeOfLife_8;
    public ButtonSetting primeOfLife_9;
    public ButtonSetting primeOfLife_10;
    public ButtonSetting primeOfLife_11;
    public ButtonSetting primeOfLife_12;
    public ButtonSetting primeOfLife_13;
    public ButtonSetting primeOfLife_14;
    public ButtonSetting primeOfLife_15;
}

public class PrimeOfLifePage : MonoBehaviour, IUICreate
{
    [NonSerialized] public PrimeOfLifeSetting primeOfLifeSetting;
    [HideInInspector] public MenuPage menuPageInstance;

    private async void Start()
    {
        primeOfLifeSetting = JsonLoader.Instance.LoadJsonData<PrimeOfLifeSetting>("JSON/PrimeOfLifeSetting.json");
        if (primeOfLifeSetting != null)
        {
            try
            {
                await CreateUI();
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[PrimeOfLifePage] Start canceled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[PrimeOfLifePage] Start failed: {e}");
                throw;
            }
        }
    }

    public async Task CreateUI()
    {
        await UIManager.Instance.CreateBackgroundImageAsync(primeOfLifeSetting.backgroundImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(primeOfLifeSetting.primeOfLifeImage, gameObject, default);
        await UIManager.Instance.CreateImageAsync(primeOfLifeSetting.primeOfLifeExplainImage, gameObject, default);

        var (backToIdleButton, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.backToIdleButton, gameObject, default);
        if (backToIdleButton != null && backToIdleButton.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(async () =>
            {
                await UIManager.Instance.ClearAllDynamic();
            });
        }

        var (backButton, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.backButton, gameObject, default);
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
        var (primeOfLife_1, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_1, gameObject, default);
        if (primeOfLife_1 != null && primeOfLife_1.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 1 button clicked.");
            });
        }
        var (primeOfLife_2, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_2, gameObject, default);
        if (primeOfLife_2 != null && primeOfLife_2.TryGetComponent<Button>(out var button2))
        {
            button2.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 2 button clicked.");
            });
        }
        var (primeOfLife_3, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_3, gameObject, default);
        if (primeOfLife_3 != null && primeOfLife_3.TryGetComponent<Button>(out var button3))
        {
            button3.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 3 button clicked.");
            });
        }
        var (primeOfLife_4, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_4, gameObject, default);
        if (primeOfLife_4 != null && primeOfLife_4.TryGetComponent<Button>(out var button4))
        {
            button4.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 4 button clicked.");
            });
        }
        var (primeOfLife_5, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_5, gameObject, default);
        if (primeOfLife_5 != null && primeOfLife_5.TryGetComponent<Button>(out var button5))
        {
            button5.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 5 button clicked.");
            });
        }
        var (primeOfLife_6, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_6, gameObject, default);
        if (primeOfLife_6 != null && primeOfLife_6.TryGetComponent<Button>(out var button6))
        {
            button6.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 6 button clicked.");
            });
        }
        var (primeOfLife_7, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_7, gameObject, default);
        if (primeOfLife_7 != null && primeOfLife_7.TryGetComponent<Button>(out var button7))
        {
            button7.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 7 button clicked.");
            });
        }
        var (primeOfLife_8, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_8, gameObject, default);
        if (primeOfLife_8 != null && primeOfLife_8.TryGetComponent<Button>(out var button8))
        {
            button8.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 8 button clicked.");
            });
        }
        var (primeOfLife_9, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_9, gameObject, default);
        if (primeOfLife_9 != null && primeOfLife_9.TryGetComponent<Button>(out var button9))
        {
            button9.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 9 button clicked.");
            });
        }
        var (primeOfLife_10, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_10, gameObject, default);
        if (primeOfLife_10 != null && primeOfLife_10.TryGetComponent<Button>(out var button10))
        {
            button10.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 10 button clicked.");
            });
        }
        var (primeOfLife_11, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_11, gameObject, default);
        if (primeOfLife_11 != null && primeOfLife_11.TryGetComponent<Button>(out var button11))
        {
            button11.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 11 button clicked.");
            });
        }
        var (primeOfLife_12, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_12, gameObject, default);
        if (primeOfLife_12 != null && primeOfLife_12.TryGetComponent<Button>(out var button12))
        {
            button12.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 12 button clicked.");
            });
        }
        var (primeOfLife_13, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_13, gameObject, default);
        if (primeOfLife_13 != null && primeOfLife_13.TryGetComponent<Button>(out var button13))
        {
            button13.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 13 button clicked.");
            });
        }
        var (primeOfLife_14, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_14, gameObject, default);
        if (primeOfLife_14 != null && primeOfLife_14.TryGetComponent<Button>(out var button14))
        {
            button14.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 14 button clicked.");
            });
        }
        var (primeOfLife_15, _) = await UIManager.Instance.CreateSingleButtonAsync(primeOfLifeSetting.primeOfLife_15, gameObject, default);
        if (primeOfLife_15 != null && primeOfLife_15.TryGetComponent<Button>(out var button15))
        {
            button15.onClick.AddListener(() =>
            {
                Debug.Log("[PrimeOfLifePage] Prime of Life 15 button clicked.");
            });
        }
    }
}

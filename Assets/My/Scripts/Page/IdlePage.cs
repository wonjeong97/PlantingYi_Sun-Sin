using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IdleSetting
{
    public ImageSetting backgroundImage;
    public TextSetting titleText;
    public ButtonSetting startButton;
    public PageSetting menuPage;
}

public class IdlePage : MonoBehaviour, IUICreate
{
    [NonSerialized] public IdleSetting idleSetting;

    private async void Start()
    {
        idleSetting = JsonLoader.Instance.LoadJsonData<IdleSetting>("JSON/IdleSetting.json");
        if (idleSetting != null)
        {
            try
            {
                await CreateUI();
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[IdlePage] Start canceled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[IdlePage] Start failed: {e}");
                throw;
            }
        }
    }

    public async Task CreateUI()
    {
        try
        {
            await UIManager.Instance.CreateBackgroundImageAsync(idleSetting.backgroundImage, gameObject, default);
            await UIManager.Instance.CreateSingleTextAsync(idleSetting.titleText, gameObject, default);
            var (startButton, _) = await UIManager.Instance.CreateSingleButtonAsync(idleSetting.startButton, gameObject, default);
            if (startButton != null && startButton.TryGetComponent<Button>(out var startBtn))
            {
                startBtn.onClick.AddListener(async () =>
                {
                    await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                    gameObject.SetActive(false);
                    GameObject parent = UIManager.Instance.mainBackground;
                    GameObject menuPage = await UIManager.Instance.CreatePageAsync(idleSetting.menuPage, parent);
                    if (menuPage)
                    {
                        menuPage.AddComponent<MenuPage>();
                    }
                });
            }
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("[IdlePage] CreateUI canceled.");
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"[IdlePage] CreateUI failed: {e}");
            throw;
        }
    }
}

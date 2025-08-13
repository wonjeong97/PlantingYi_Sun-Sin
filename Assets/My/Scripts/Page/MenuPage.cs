using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;

    public ButtonSetting whatIsButton;       // 플랜팅 이순신이란
    public ButtonSetting childhoodButton;    // 유년
    public ButtonSetting youthButton;        // 청년
    public ButtonSetting primeOfLifeButton;  // 장년
    public ButtonSetting lastButton;         // 말년
    public ButtonSetting deathButton;        // 죽음
    public ButtonSetting afterDeathButton;   // 사후

    public PageSetting whatIsPage;       // 플랜팅 이순신이란 페이지
    public PageSetting childhoodPage;    // 유년 페이지
    public PageSetting youthPage;        // 청년 페이지
    public PageSetting primeOfLifePage;  // 장년 페이지    
    public PageSetting lastPage;         // 말년 페이지
    public PageSetting deathPage;        // 죽음 페이지
    public PageSetting afterDeathPage;   // 사후 페이지
}

public class MenuPage : MonoBehaviour, IUICreate
{
    [NonSerialized] public MenuSetting menuSetting;

    private GameObject whatIsPage;
    private GameObject childhoodPage;
    private GameObject youthPage;
    private GameObject primeOfLifePage;
    private GameObject lastPage;
    private GameObject deathPage;
    private GameObject afterDeathPage;

    private async void Start()
    {
        menuSetting = JsonLoader.Instance.LoadJsonData<MenuSetting>("JSON/MenuSetting.json");
        if (menuSetting != null)
        {
            try
            {
                await CreateUI();
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
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
        await UIManager.Instance.CreateBackgroundImageAsync(menuSetting.backgroundImage, gameObject, default);
        var (backtoIdleButton, _) = await UIManager.Instance.CreateSingleButtonAsync(menuSetting.backToIdleButton, gameObject, default);
        if (backtoIdleButton != null && backtoIdleButton.TryGetComponent<Button>(out var idleBtn))
        {
            idleBtn.onClick.AddListener(async () =>
            {
                await UIManager.Instance.ClearAllDynamic();
            });
        }

        await CreatePageButton();
    }

    private async Task CreatePageButton()
    {
        GameObject parent = UIManager.Instance.mainBackground;

        var (whatIsButton, _) = await UIManager.Instance.CreateSingleButtonAsync(menuSetting.whatIsButton, gameObject, default);       
        if (whatIsButton != null && whatIsButton.TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(async () =>
            {
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (whatIsPage == null)
                {
                    whatIsPage = await UIManager.Instance.CreatePageAsync(menuSetting.whatIsPage, parent);
                    if (whatIsPage != null)
                    {
                        whatIsPage.AddComponent<WhatIsPage>();
                        whatIsPage.GetComponent<WhatIsPage>().menuPageInstance = this;
                    }
                }
                else
                {
                    whatIsPage.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }
            });
        }

        var (childhoodButton, _) = await UIManager.Instance.CreateSingleButtonAsync(menuSetting.childhoodButton, gameObject, default);
        if (childhoodButton != null && childhoodButton.TryGetComponent<Button>(out var childhoodBtn))
        {
            childhoodBtn.onClick.AddListener(async () =>
            {
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (childhoodPage == null)
                {
                    childhoodPage = await UIManager.Instance.CreatePageAsync(menuSetting.childhoodPage, parent);
                    if (childhoodPage != null)
                    {
                        childhoodPage.AddComponent<ChildhoodPage>();
                        childhoodPage.GetComponent<ChildhoodPage>().menuPageInstance = this;
                    }
                }
                else
                {
                    childhoodPage.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }
            });
        }

        var (youthButton, _) = await UIManager.Instance.CreateSingleButtonAsync(menuSetting.youthButton, gameObject, default);
        if (youthButton != null && youthButton.TryGetComponent<Button>(out var youthBtn))
        {
            youthBtn.onClick.AddListener(async () =>
            {
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (youthPage == null)
                {
                    youthPage = await UIManager.Instance.CreatePageAsync(menuSetting.youthPage, parent);
                    if (youthPage != null)
                    {
                        youthPage.AddComponent<YouthPage>();
                        youthPage.GetComponent<YouthPage>().menuPageInstance = this;
                    }
                }
                else
                {
                    youthPage.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }
            });
        }

        var (primeOfLifeButton, _) = await UIManager.Instance.CreateSingleButtonAsync(menuSetting.primeOfLifeButton, gameObject, default);
        if (primeOfLifeButton != null && primeOfLifeButton.TryGetComponent<Button>(out var primeBtn))
        {
            primeBtn.onClick.AddListener(async () =>
            {
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (primeOfLifePage == null)
                {
                    primeOfLifePage = await UIManager.Instance.CreatePageAsync(menuSetting.primeOfLifePage, parent);
                    if (primeOfLifePage != null)
                    {
                        primeOfLifePage.AddComponent<PrimeOfLifePage>();
                        primeOfLifePage.GetComponent<PrimeOfLifePage>().menuPageInstance = this;
                    }
                }
                else
                {
                    primeOfLifePage.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }

            });
        }

        var (lastButton, _) = await UIManager.Instance.CreateSingleButtonAsync(menuSetting.lastButton, gameObject, default);
        if (lastButton != null && lastButton.TryGetComponent<Button>(out var lastBtn))
        {
            lastBtn.onClick.AddListener(async () =>
            {
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (lastPage == null)
                {
                    lastPage = await UIManager.Instance.CreatePageAsync(menuSetting.lastPage, parent);
                    if (lastPage != null)
                    {
                        lastPage.AddComponent<LastPage>();
                        lastPage.GetComponent<LastPage>().menuPageInstance = this;
                    }
                }
                else
                {
                    lastPage.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }
            });
        }

        var (deathButton, _) = await UIManager.Instance.CreateSingleButtonAsync(menuSetting.deathButton, gameObject, default);
        if (deathButton != null && deathButton.TryGetComponent<Button>(out var deathBtn))
        {
            deathBtn.onClick.AddListener(async () =>
            {
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (deathPage == null)
                {
                    deathPage = await UIManager.Instance.CreatePageAsync(menuSetting.deathPage, parent);
                    if (deathPage != null)
                    {
                        deathPage.AddComponent<DeathPage>();
                        deathPage.GetComponent<DeathPage>().menuPageInstance = this;
                    }
                }
                else
                {
                    deathPage.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }
            });
        }

        var (afterDeathButton, _) = await UIManager.Instance.CreateSingleButtonAsync(menuSetting.afterDeathButton, gameObject, default);
        if (afterDeathButton != null && afterDeathButton.TryGetComponent<Button>(out var afterDeathBtn))
        {
            afterDeathBtn.onClick.AddListener(async () =>
            {
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (afterDeathPage == null)
                {
                    afterDeathPage = await UIManager.Instance.CreatePageAsync(menuSetting.afterDeathPage, parent);
                    if (afterDeathPage != null)
                    {
                        afterDeathPage.AddComponent<AfterDeathPage>();
                        afterDeathPage.GetComponent<AfterDeathPage>().menuPageInstance = this;
                    }
                }
                else
                {
                    afterDeathPage.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }
            });
        }
    }
}

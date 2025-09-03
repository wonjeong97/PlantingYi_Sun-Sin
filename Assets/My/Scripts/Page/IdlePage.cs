using System;
using System.Threading;
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

public class IdlePage : BasePage<IdleSetting>
{
    private GameObject menuPageObject;

    // JSON 경로
    protected override string JsonPath => "JSON/IdleSetting.json";

    // 페이지 전용 콘텐츠(타이틀 텍스트, 시작 버튼)만 구성
    protected override async Task BuildContentAsync()
    {
        // 타이틀 텍스트 생성
        await UICreator.Instance.CreateSingleTextAsync(
            setting.titleText, gameObject, CancellationToken.None);

        // 시작 버튼 생성
        var created = await UICreator.Instance.CreateSingleButtonAsync(
            setting.startButton, gameObject, CancellationToken.None);

        // 생성된 오브젝트 중 button 오브젝트만 가져옴
        var startGO = created.button;
        if (startGO != null && startGO.TryGetComponent<Button>(out var startBtn))
        {
            startBtn.onClick.AddListener(() => { _ = HandleStartButtonClickedAsync(); });
        }
    }

    private async Task HandleStartButtonClickedAsync()
    {
        try
        {
            // 페이드 아웃 후 IdlePage 비활성화
            await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.settings.fadeTime, true);
            gameObject.SetActive(false);

            if (menuPageObject == null)
            {
                // 메뉴 페이지 생성 및 표시
                GameObject parent = UIManager.Instance.MainBackground;
                menuPageObject = await UICreator.Instance.CreatePageAsync(setting.menuPage, parent, CancellationToken.None);
                if (menuPageObject != null)
                {
                    menuPageObject.AddComponent<MenuPage>();
                }
            }
            else
            {
                menuPageObject.SetActive(true);
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.settings.fadeTime, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[IdlePage] Start Button Click failed: {e}");
        }
    }
}
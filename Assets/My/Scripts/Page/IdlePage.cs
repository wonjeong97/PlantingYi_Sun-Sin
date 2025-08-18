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
        await UIManager.Instance.CreateSingleTextAsync(
            Setting.titleText, gameObject, default(CancellationToken));

        // 시작 버튼 생성
        var created = await UIManager.Instance.CreateSingleButtonAsync(
            Setting.startButton, gameObject, default(CancellationToken));

        // 생성된 오브젝트 중 button 오브젝트만 가져옴
        var startGO = created.button;
        if (startGO != null && startGO.TryGetComponent<Button>(out var startBtn))
        {
            startBtn.onClick.AddListener(async () =>
            {
                // 페이드 아웃 후 IdlePage 비활성화
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (menuPageObject == null)
                {
                    // 메뉴 페이지 생성 및 표시
                    GameObject parent = UIManager.Instance.mainBackground;
                    menuPageObject = await UIManager.Instance.CreatePageAsync(Setting.menuPage, parent);
                    if (menuPageObject != null)
                    {
                        menuPageObject.AddComponent<MenuPage>();
                    }
                }
                else
                {
                    menuPageObject.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }                
            });
        }
    }
}
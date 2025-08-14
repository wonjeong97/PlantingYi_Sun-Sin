using System;
using System.Threading;
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

public class AfterDeathPage : BasePage<AfterDeathSetting>
{
    protected override string JsonPath => "JSON/AfterDeathSetting.json";

    protected override async Task BuildContentAsync()
    {
        // 전용 이미지
        if (Setting.afterDeathImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.afterDeathImage, gameObject, default(CancellationToken));

        if (Setting.afterDeathExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.afterDeathExplainImage, gameObject, default(CancellationToken));

        // 전용 버튼들
        await WireButton(Setting.afterDeath_1, "[AfterDeathPage] 1 clicked");
        await WireButton(Setting.afterDeath_2, "[AfterDeathPage] 2 clicked");
        await WireButton(Setting.afterDeath_3, "[AfterDeathPage] 3 clicked");
        await WireButton(Setting.afterDeath_4, "[AfterDeathPage] 4 clicked");
        await WireButton(Setting.afterDeath_5, "[AfterDeathPage] 5 clicked");
        await WireButton(Setting.afterDeath_6, "[AfterDeathPage] 6 clicked");
        await WireButton(Setting.afterDeath_7, "[AfterDeathPage] 7 clicked");
        await WireButton(Setting.afterDeath_8, "[AfterDeathPage] 8 clicked");
        await WireButton(Setting.afterDeath_9, "[AfterDeathPage] 9 clicked");
        await WireButton(Setting.afterDeath_10, "[AfterDeathPage] 10 clicked");
        await WireButton(Setting.afterDeath_11, "[AfterDeathPage] 11 clicked");
        await WireButton(Setting.afterDeath_12, "[AfterDeathPage] 12 clicked");
    }

    // 버튼 생성 및 이벤트 연결
    // TODO: 향후 로그 메시지 대신 실제 페이지 전환 로직으로 변경 필요
    private async Task WireButton(ButtonSetting bs, string logMessage)
    {
        if (bs == null) return;

        var created = await UIManager.Instance.CreateSingleButtonAsync(
            bs, gameObject, default(CancellationToken));

        var go = created.button;
        if (go != null && go.TryGetComponent<Button>(out var btn))
        {
            btn.onClick.AddListener(() => Debug.Log(logMessage));
        }
    }
}
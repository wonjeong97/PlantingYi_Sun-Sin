using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LastPageSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting lastImage;
    public ImageSetting lastExplainImage;

    public ButtonSetting last_1;
    public ButtonSetting last_2;
    public ButtonSetting last_3;
    public ButtonSetting last_4;
    public ButtonSetting last_5;
    public ButtonSetting last_6;
    public ButtonSetting last_7;
    public ButtonSetting last_8;
    public ButtonSetting last_9;
}

public class LastPage : BasePage<LastPageSetting>
{
    protected override string JsonPath => "JSON/LastSetting.json";

    protected override async Task BuildContentAsync()
    {
        // 전용 이미지
        if (Setting.lastImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.lastImage, gameObject, default(CancellationToken));

        if (Setting.lastExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.lastExplainImage, gameObject, default(CancellationToken));

        // 전용 버튼들
        await WireButton(Setting.last_1, "[LastPage] last_1 clicked");
        await WireButton(Setting.last_2, "[LastPage] last_2 clicked");
        await WireButton(Setting.last_3, "[LastPage] last_3 clicked");
        await WireButton(Setting.last_4, "[LastPage] last_4 clicked");
        await WireButton(Setting.last_5, "[LastPage] last_5 clicked");
        await WireButton(Setting.last_6, "[LastPage] last_6 clicked");
        await WireButton(Setting.last_7, "[LastPage] last_7 clicked");
        await WireButton(Setting.last_8, "[LastPage] last_8 clicked");
        await WireButton(Setting.last_9, "[LastPage] last_9 clicked");
    }

    // 버튼 생성 및 이벤트 연결
    // TODO: 향후 로그 메시지 대신 실제 페이지 전환 로직으로 변경 필요
    private async Task WireButton(ButtonSetting bs, string logMessage)
    {
        if (bs == null) return;

        var created = await UIManager.Instance.CreateSingleButtonAsync(bs, gameObject, default(CancellationToken));
        var go = created.button;
        if (go != null && go.TryGetComponent<Button>(out var btn))
        {
            btn.onClick.AddListener(() => Debug.Log(logMessage));
        }
    }
}

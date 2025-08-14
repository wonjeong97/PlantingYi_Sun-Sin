using System;
using System.Threading;
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
public class YouthPage : BasePage<YouthSetting>
{
    protected override string JsonPath => "JSON/YouthSetting.json";

    protected override async Task BuildContentAsync()
    {
        // 페이지 전용 이미지
        if (Setting.youthImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.youthImage, gameObject, default(CancellationToken));

        if (Setting.youthExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.youthExplainImage, gameObject, default(CancellationToken));

        // 페이지 전용 버튼들
        await WireButton(Setting.youth_1, "[YouthPage] youth_1 clicked.");
        await WireButton(Setting.youth_2, "[YouthPage] youth_2 clicked.");
        await WireButton(Setting.youth_3, "[YouthPage] youth_3 clicked.");
    }

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

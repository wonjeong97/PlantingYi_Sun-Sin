using System;
using System.Threading;
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

public class DeathPage : BasePage<DeathSetting>
{
    protected override string JsonPath => "JSON/DeathSetting.json";

    protected override async Task BuildContentAsync()
    {
        // 전용 이미지
        if (Setting.deathImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.deathImage, gameObject, default(CancellationToken));

        if (Setting.deathExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.deathExplainImage, gameObject, default(CancellationToken));

        // 전용 버튼들
        await WireButton(Setting.death_1, "[DeathPage] death_1 clicked");
        await WireButton(Setting.death_2, "[DeathPage] death_2 clicked");
        await WireButton(Setting.death_3, "[DeathPage] death_3 clicked");
        await WireButton(Setting.death_4, "[DeathPage] death_4 clicked");
        await WireButton(Setting.death_5, "[DeathPage] death_5 clicked");
        await WireButton(Setting.death_6, "[DeathPage] death_6 clicked");
        await WireButton(Setting.death_7, "[DeathPage] death_7 clicked");
        await WireButton(Setting.death_8, "[DeathPage] death_8 clicked");
    }

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

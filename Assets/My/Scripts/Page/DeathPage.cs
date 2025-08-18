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

    public ButtonSetting[] deathBtns;
    public PopupSetting[] deathPopups;     
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
        for (int i = 0; i < Setting.deathBtns.Length; i++)
        {
            await WireButton(Setting.deathBtns[i], Setting.deathPopups[i], gameObject);
        }
    }
}

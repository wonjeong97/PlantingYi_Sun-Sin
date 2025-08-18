using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ChildhoodSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting childhoodImage;
    public ImageSetting childhoodExplainImage;

    public ButtonSetting[] childhoodBtns;
    public PopupSetting[] childhoodPopups;
}

public class ChildhoodPage : BasePage<ChildhoodSetting>
{
    // JSON ���
    protected override string JsonPath => "JSON/ChildhoodSetting.json";

    // ������ �� ������ ����
    protected override async Task BuildContentAsync()
    {
        // ������ ���� �̹���
        if (Setting.childhoodImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.childhoodImage, gameObject, default(CancellationToken));
        if (Setting.childhoodExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.childhoodExplainImage, gameObject, default(CancellationToken));

        for (int i = 0; i < Setting.childhoodBtns.Length; i++)
        {
            await WireButton(Setting.childhoodBtns[i], Setting.childhoodPopups[i], gameObject);
        }
    }
}

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

    public ButtonSetting[] youthBtns;
    public PopupSetting[] youthPopups;   
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
        for (int i = 0; i < Setting.youthBtns.Length; i++)
        {
            await WireButton(Setting.youthBtns[i], Setting.youthPopups[i], gameObject);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

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
        if (Setting.youthImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.youthImage, gameObject, CancellationToken.None);

        if (Setting.youthExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.youthExplainImage, gameObject,
                CancellationToken.None);
        
        for (int i = 0; i < Setting.youthBtns.Length; i++)
        {
            await WireButton(Setting.youthBtns[i], Setting.youthPopups[i], gameObject);
        }
    }
}
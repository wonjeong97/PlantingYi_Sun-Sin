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
    
    public PopupSetting[] subPopups;
}

public class YouthPage : BasePage<YouthSetting>
{
    protected override string JsonPath => "JSON/YouthSetting.json";

    protected override async Task BuildContentAsync()
    {
        if (setting.youthImage != null)
            await UIManager.Instance.CreateImageAsync(setting.youthImage, gameObject, CancellationToken.None);

        if (setting.youthExplainImage != null)
            await UIManager.Instance.CreateImageAsync(setting.youthExplainImage, gameObject,
                CancellationToken.None);

        for (int i = 0; i < setting.youthBtns.Length; i++)
        {
            await WireButton(setting.youthBtns[i], setting.youthPopups, i, gameObject);
        }
    }
}
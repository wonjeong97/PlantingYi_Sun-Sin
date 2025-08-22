using System;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public class AfterDeathSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting afterDeathImage;
    public ImageSetting afterDeathExplainImage;

    public ButtonSetting[] afterDeathBtns;
    public PopupSetting[] afterDeathPopups;
}

public class AfterDeathPage : BasePage<AfterDeathSetting>
{
    protected override string JsonPath => "JSON/AfterDeathSetting.json";

    protected override async Task BuildContentAsync()
    {
        if (setting.afterDeathImage != null)
            await UIManager.Instance.CreateImageAsync(setting.afterDeathImage, gameObject, CancellationToken.None);

        if (setting.afterDeathExplainImage != null)
            await UIManager.Instance.CreateImageAsync(setting.afterDeathExplainImage, gameObject,
                CancellationToken.None);

        for (int i = 0; i < setting.afterDeathBtns.Length; i++)
        {
            await WireButton(setting.afterDeathBtns[i], setting.afterDeathPopups, i, gameObject);
        }
    }
}
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
        if (Setting.afterDeathImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.afterDeathImage, gameObject, CancellationToken.None);

        if (Setting.afterDeathExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.afterDeathExplainImage, gameObject, CancellationToken.None);
        
        for (int i = 0; i < Setting.afterDeathBtns.Length; i++)
        {
            await WireButton(Setting.afterDeathBtns[i], Setting.afterDeathPopups[i], gameObject);
        }
    }
}
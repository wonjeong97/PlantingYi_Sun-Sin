using System;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public class LastPageSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting lastImage;
    public ImageSetting lastExplainImage;

    public ButtonSetting[] lastBtns;
    public PopupSetting[] lastPopups;
}

public class LastPage : BasePage<LastPageSetting>
{
    protected override string JsonPath => "JSON/LastSetting.json";

    protected override async Task BuildContentAsync()
    {
        if (setting.lastImage != null)
            await UIManager.Instance.CreateImageAsync(setting.lastImage, gameObject, CancellationToken.None);

        if (setting.lastExplainImage != null)
            await UIManager.Instance.CreateImageAsync(setting.lastExplainImage, gameObject, CancellationToken.None);

        for (int i = 0; i < setting.lastBtns.Length; i++)
        {
            await WireButton(setting.lastBtns[i], setting.lastPopups, i, gameObject);
        }
    }
}
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
        if (Setting.lastImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.lastImage, gameObject, CancellationToken.None);

        if (Setting.lastExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.lastExplainImage, gameObject, CancellationToken.None);
        
        for (int i = 0; i < Setting.lastBtns.Length; i++)
        {
            await WireButton(Setting.lastBtns[i], Setting.lastPopups[i], gameObject);
        }
    }
}

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
        // 전용 이미지
        if (Setting.lastImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.lastImage, gameObject, default(CancellationToken));

        if (Setting.lastExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.lastExplainImage, gameObject, default(CancellationToken));

        // 전용 버튼들
        for (int i = 0; i < Setting.lastBtns.Length; i++)
        {
            await WireButton(Setting.lastBtns[i], Setting.lastPopups[i], gameObject);
        }
    }
}

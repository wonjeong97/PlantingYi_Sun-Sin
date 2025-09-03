using System;
using System.Threading;
using System.Threading.Tasks;

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

    public PopupSetting[] subPopups;
}

public class ChildhoodPage : BasePage<ChildhoodSetting>
{
    // JSON 경로
    protected override string JsonPath => "JSON/ChildhoodSetting.json";

    // 페이지 별 컨텐츠 생성
    protected override async Task BuildContentAsync()
    {
        // 페이지 전용 이미지
        if (setting.childhoodImage != null)
            await UICreator.Instance.CreateSingleImageAsync(setting.childhoodImage, gameObject, CancellationToken.None);
        if (setting.childhoodExplainImage != null)
            await UICreator.Instance.CreateSingleImageAsync(setting.childhoodExplainImage, gameObject, CancellationToken.None);

        for (int i = 0; i < setting.childhoodBtns.Length; i++)
        {
            await WireButton(setting.childhoodBtns[i], setting.childhoodPopups, i, gameObject);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public class DeathSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting deathImage;
    public ImageSetting deathExplainImage;

    public ButtonSetting[] deathBtns;
    public PopupSetting[] deathPopups;
    
    public PopupSetting[] subPopups;
}

public class DeathPage : BasePage<DeathSetting>
{
    protected override string JsonPath => "JSON/DeathSetting.json";

    protected override async Task BuildContentAsync()
    {
        if (setting.deathImage != null)
            await UIManager.Instance.CreateImageAsync(setting.deathImage, gameObject, CancellationToken.None);

        if (setting.deathExplainImage != null)
            await UIManager.Instance.CreateImageAsync(setting.deathExplainImage, gameObject, CancellationToken.None);

        for (int i = 0; i < setting.deathBtns.Length; i++)
        {
            await WireButton(setting.deathBtns[i], setting.deathPopups, i, gameObject);
        }
    }
}
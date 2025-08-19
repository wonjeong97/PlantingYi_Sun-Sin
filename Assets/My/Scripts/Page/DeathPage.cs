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
}

public class DeathPage : BasePage<DeathSetting>
{
    protected override string JsonPath => "JSON/DeathSetting.json";

    protected override async Task BuildContentAsync()
    {
        // Àü¿ë ÀÌ¹ÌÁö
        if (Setting.deathImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.deathImage, gameObject, CancellationToken.None);

        if (Setting.deathExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.deathExplainImage, gameObject, CancellationToken.None);

        // Àü¿ë ¹öÆ°µé
        for (int i = 0; i < Setting.deathBtns.Length; i++)
        {
            await WireButton(Setting.deathBtns[i], Setting.deathPopups[i], gameObject);
        }
    }
}
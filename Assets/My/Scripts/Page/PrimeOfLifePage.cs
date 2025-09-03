using System;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public class PrimeOfLifeSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;
    public ButtonSetting backButton;

    public ImageSetting primeOfLifeImage;
    public ImageSetting primeOfLifeExplainImage;

    public ButtonSetting[] primeOfLifeBtns;
    public PopupSetting[] primeOfLifePopups;

    public PopupSetting[] subPopups;
}

public class PrimeOfLifePage : BasePage<PrimeOfLifeSetting>
{
    protected override string JsonPath => "JSON/PrimeOfLifeSetting.json";

    protected override async Task BuildContentAsync()
    {
        if (setting.primeOfLifeImage != null)
            await UICreator.Instance.CreateSingleImageAsync(setting.primeOfLifeImage, gameObject,
                CancellationToken.None);

        if (setting.primeOfLifeExplainImage != null)
            await UICreator.Instance.CreateSingleImageAsync(setting.primeOfLifeExplainImage, gameObject,
                CancellationToken.None);

        for (int i = 0; i < setting.primeOfLifeBtns.Length; i++)
        {
            await WireButton(setting.primeOfLifeBtns[i], setting.primeOfLifePopups, i, gameObject);
        }
    }
}
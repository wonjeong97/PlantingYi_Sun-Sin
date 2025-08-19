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
}

public class PrimeOfLifePage : BasePage<PrimeOfLifeSetting>
{
    protected override string JsonPath => "JSON/PrimeOfLifeSetting.json";

    protected override async Task BuildContentAsync()
    {
        if (Setting.primeOfLifeImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.primeOfLifeImage, gameObject, CancellationToken.None);

        if (Setting.primeOfLifeExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.primeOfLifeExplainImage, gameObject, CancellationToken.None);
        
        for (int i = 0; i < Setting.primeOfLifeBtns.Length; i++)
        {
            await WireButton(Setting.primeOfLifeBtns[i], Setting.primeOfLifePopups[i], gameObject);
        }
    }
}
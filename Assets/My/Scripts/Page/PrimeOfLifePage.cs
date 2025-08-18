using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
        // 전용 이미지
        if (Setting.primeOfLifeImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.primeOfLifeImage, gameObject, default(CancellationToken));

        if (Setting.primeOfLifeExplainImage != null)
            await UIManager.Instance.CreateImageAsync(Setting.primeOfLifeExplainImage, gameObject, default(CancellationToken));

        // 전용 버튼들
        for (int i = 0; i < Setting.primeOfLifeBtns.Length; i++)
        {
            await WireButton(Setting.primeOfLifeBtns[i], Setting.primeOfLifePopups[i], gameObject);
        }
    }
}
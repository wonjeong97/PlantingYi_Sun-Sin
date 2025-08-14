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

    public ButtonSetting primeOfLife_1;
    public ButtonSetting primeOfLife_2;
    public ButtonSetting primeOfLife_3;
    public ButtonSetting primeOfLife_4;
    public ButtonSetting primeOfLife_5;
    public ButtonSetting primeOfLife_6;
    public ButtonSetting primeOfLife_7;
    public ButtonSetting primeOfLife_8;
    public ButtonSetting primeOfLife_9;
    public ButtonSetting primeOfLife_10;
    public ButtonSetting primeOfLife_11;
    public ButtonSetting primeOfLife_12;
    public ButtonSetting primeOfLife_13;
    public ButtonSetting primeOfLife_14;
    public ButtonSetting primeOfLife_15;
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
        await WireButton(Setting.primeOfLife_1, "[PrimeOfLifePage] 1 clicked");
        await WireButton(Setting.primeOfLife_2, "[PrimeOfLifePage] 2 clicked");
        await WireButton(Setting.primeOfLife_3, "[PrimeOfLifePage] 3 clicked");
        await WireButton(Setting.primeOfLife_4, "[PrimeOfLifePage] 4 clicked");
        await WireButton(Setting.primeOfLife_5, "[PrimeOfLifePage] 5 clicked");
        await WireButton(Setting.primeOfLife_6, "[PrimeOfLifePage] 6 clicked");
        await WireButton(Setting.primeOfLife_7, "[PrimeOfLifePage] 7 clicked");
        await WireButton(Setting.primeOfLife_8, "[PrimeOfLifePage] 8 clicked");
        await WireButton(Setting.primeOfLife_9, "[PrimeOfLifePage] 9 clicked");
        await WireButton(Setting.primeOfLife_10, "[PrimeOfLifePage] 10 clicked");
        await WireButton(Setting.primeOfLife_11, "[PrimeOfLifePage] 11 clicked");
        await WireButton(Setting.primeOfLife_12, "[PrimeOfLifePage] 12 clicked");
        await WireButton(Setting.primeOfLife_13, "[PrimeOfLifePage] 13 clicked");
        await WireButton(Setting.primeOfLife_14, "[PrimeOfLifePage] 14 clicked");
        await WireButton(Setting.primeOfLife_15, "[PrimeOfLifePage] 15 clicked");
    }

    // 버튼 생성 및 이벤트 연결
    // TODO: 향후 로그 메시지 대신 실제 페이지 전환 로직으로 변경 필요
    private async Task WireButton(ButtonSetting bs, string logMessage)
    {
        if (bs == null) return;

        var created = await UIManager.Instance.CreateSingleButtonAsync(
            bs, gameObject, default(CancellationToken));

        var go = created.button;
        if (go != null && go.TryGetComponent<Button>(out var btn))
        {
            btn.onClick.AddListener(() => Debug.Log(logMessage));
        }
    }
}
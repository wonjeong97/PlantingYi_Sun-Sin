using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IdleSetting
{
    public ImageSetting backgroundImage;
    public TextSetting titleText;
    public ButtonSetting startButton;
    public PageSetting menuPage;
}

public class IdlePage : BasePage<IdleSetting>
{
    private GameObject menuPageObject;

    // JSON ���
    protected override string JsonPath => "JSON/IdleSetting.json";   

    // ������ ���� ������(Ÿ��Ʋ �ؽ�Ʈ, ���� ��ư)�� ����
    protected override async Task BuildContentAsync()
    {
        // Ÿ��Ʋ �ؽ�Ʈ ����
        await UIManager.Instance.CreateSingleTextAsync(
            Setting.titleText, gameObject, default(CancellationToken));

        // ���� ��ư ����
        var created = await UIManager.Instance.CreateSingleButtonAsync(
            Setting.startButton, gameObject, default(CancellationToken));

        // ������ ������Ʈ �� button ������Ʈ�� ������
        var startGO = created.button;
        if (startGO != null && startGO.TryGetComponent<Button>(out var startBtn))
        {
            startBtn.onClick.AddListener(async () =>
            {
                // ���̵� �ƿ� �� IdlePage ��Ȱ��ȭ
                await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                gameObject.SetActive(false);

                if (menuPageObject == null)
                {
                    // �޴� ������ ���� �� ǥ��
                    GameObject parent = UIManager.Instance.mainBackground;
                    menuPageObject = await UIManager.Instance.CreatePageAsync(Setting.menuPage, parent);
                    if (menuPageObject != null)
                    {
                        menuPageObject.AddComponent<MenuPage>();
                    }
                }
                else
                {
                    menuPageObject.SetActive(true);
                    await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                }                
            });
        }
    }
}
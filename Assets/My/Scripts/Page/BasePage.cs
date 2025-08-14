using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePage<TSetting> : MonoBehaviour, IUICreate where TSetting : class
{
    [NonSerialized] public TSetting Setting;
    [HideInInspector] public MenuPage menuPageInstance;

    // 각 파생 페이지에서 JSON 경로만 지정
    protected abstract string JsonPath { get; }

    // 각 파생 페이지의 전용 콘텐츠
    protected abstract Task BuildContentAsync();

    protected virtual async void Start()
    {
        Setting = JsonLoader.Instance.LoadJsonData<TSetting>(JsonPath);
        if (Setting == null)
        {
            Debug.LogError($"[{GetType().Name}] Settings not found at {JsonPath}");
            return;
        }

        try
        {
            await CreateUI();

            // 공통 페이드 인
            await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"[{GetType().Name}] Start canceled.");
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"[{GetType().Name}] Start failed: {e}");
            throw;
        }
    }

    public async Task CreateUI()
    {
        // 1) 배경
        var background = GetFieldOrProperty<ImageSetting>(Setting, "backgroundImage");
        if (background != null)
            await UIManager.Instance.CreateBackgroundImageAsync(background, gameObject, default(CancellationToken));

        // 2) 처음으로 버튼 (Idle 복귀)
        var backToIdle = GetFieldOrProperty<ButtonSetting>(Setting, "backToIdleButton");
        if (backToIdle != null)
        {
            var created = await UIManager.Instance.CreateSingleButtonAsync(backToIdle, gameObject, default(CancellationToken));
            var go = created.button; // 튜플 분해 대신 명시적 접근
            if (go != null && go.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(async () =>
                {
                    await UIManager.Instance.ClearAllDynamic();
                });
            }
        }

        // 3) 뒤로가기 버튼 (메뉴 페이지 복귀)
        var backButtonSetting = GetFieldOrProperty<ButtonSetting>(Setting, "backButton");
        if (backButtonSetting != null)
        {
            var created = await UIManager.Instance.CreateSingleButtonAsync(backButtonSetting, gameObject, default(CancellationToken));
            var go = created.button;
            if (go != null && go.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(async () =>
                {
                    await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.Settings.fadeTime, true);
                    gameObject.SetActive(false);
                    if (menuPageInstance != null)
                    {
                        menuPageInstance.gameObject.SetActive(true);
                        await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.Settings.fadeTime, true);
                    }
                });
            }
        }

        // 4) 각 페이지 전용 콘텐츠
        await BuildContentAsync();
    }

    // 공통 필드 추출 유틸: 필드/프로퍼티 둘 다 지원
    private static TField GetFieldOrProperty<TField>(object obj, string name) where TField : class
    {
        if (obj == null) return null;

        var type = obj.GetType();

        // Field 먼저
        var fi = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fi != null)
            return fi.GetValue(obj) as TField;

        // Property 다음
        var pi = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (pi != null)
            return pi.GetValue(obj) as TField;

        return null;
    }
}

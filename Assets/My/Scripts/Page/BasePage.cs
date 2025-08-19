using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 모든 UI 페이지의 공통 기반 클래스.
/// 제네릭으로 페이지별 Setting 타입을 지정하고,
/// 공통 UI 생성 로직(배경, 뒤로가기, Idle 복귀 버튼 등)을 제공.
/// </summary>
/// <typeparam name="TSetting">각 페이지별 설정 데이터 클래스 타입</typeparam>
public abstract class BasePage<TSetting> : MonoBehaviour, IUICreate where TSetting : class
{
    [NonSerialized] protected TSetting Setting; // 페이지별 설정 데이터
    [HideInInspector] public MenuPage menuPageInstance; // 메뉴 페이지 인스턴스 (뒤로가기 버튼에서 사용)

    protected abstract string JsonPath { get; } // 각 파생 페이지에서 JSON 경로만 지정
    protected abstract Task BuildContentAsync(); // 각 파생 페이지의 전용 콘텐츠    

    // 페이지 시작 시 JSON 로드 → UI 생성 → 페이드 인 순서로 실행
    protected virtual async Task StartAsync()
    {
        try
        {
            // JSON 데이터 로드
            Setting = JsonLoader.Instance.LoadJsonData<TSetting>(JsonPath);
            if (Setting == null)
            {
                Debug.LogError($"[{GetType().Name}] Settings not found at {JsonPath}");
                return;
            }

            // UI 생성
            await CreateUI();

            // 공통 페이드 인
            await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.settings.fadeTime, true);
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"[{GetType().Name}] Start canceled.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[{GetType().Name}] Start failed: {e}");
        }
    }

    protected virtual void Start()
    {
        _ = StartAsync(); // fire-and-forget, 예외는 StartAsync 내부에서 처리됨
    }

    // 공통 UI 생성 로직
    public async Task CreateUI()
    {
        // 1) 배경 생성
        var background = GetFieldOrProperty<ImageSetting>(Setting, "backgroundImage");
        if (background != null)
            await UIManager.Instance.CreateBackgroundImageAsync(background, gameObject, CancellationToken.None);

        // 2) 처음으로 버튼 (Idle 복귀)
        var backToIdle = GetFieldOrProperty<ButtonSetting>(Setting, "backToIdleButton");
        if (backToIdle != null)
        {
            var created =
                await UIManager.Instance.CreateSingleButtonAsync(backToIdle, gameObject, CancellationToken.None);
            var go = created.button;
            if (go != null && go.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() => { _ = HandleBackToIdleAsync(); });
            }
        }

        // 3) 뒤로가기 버튼 (메뉴 페이지 복귀)
        var backButtonSetting = GetFieldOrProperty<ButtonSetting>(Setting, "backButton");
        if (backButtonSetting != null)
        {
            var created =
                await UIManager.Instance.CreateSingleButtonAsync(backButtonSetting, gameObject, CancellationToken.None);

            var go = created.button;
            if (go != null && go.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() => _ = HandleBackToMenuAsync());
            }
        }

        // 4) 페이지별 전용 콘텐츠 생성
        await BuildContentAsync();
    }

    private async Task HandleBackToIdleAsync()
    {
        try
        {
            await UIManager.Instance.ShowIdlePageOnly();
        }
        catch (Exception e)
        {
            Debug.LogError($"[BasePage] Button click failed: {e}");
        }
    }

    private async Task HandleBackToMenuAsync()
    {
        try
        {
            await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.settings.fadeTime, true);
            gameObject.SetActive(false);
            if (menuPageInstance != null)
            {
                menuPageInstance.gameObject.SetActive(true);
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.settings.fadeTime, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[BasePage] Button click failed: {e}");
        }
    }

    // 버튼 생성 및 이벤트 연결
    protected async Task WireButton(ButtonSetting bs, PopupSetting ps, GameObject parent)
    {
        if (bs == null || ps == null) return;

        var created = await UIManager.Instance.CreateSingleButtonAsync(bs, parent, CancellationToken.None);
        var go = created.button;
        if (go != null && go.TryGetComponent<Button>(out var btn))
        {
            btn.onClick.AddListener(() => _ = HandleButtonClick(ps, parent));
        }
    }

    private async Task HandleButtonClick(PopupSetting ps, GameObject parent)
    {
        try
        {
            await UIManager.Instance.CreatePopupAsync(ps, parent);
        }
        catch (Exception e)
        {
            Debug.LogError($"[BasePage] Popup create failed: {e}");
        }
    }

    /// <summary>
    /// 지정한 이름의 필드나 프로퍼티 값을 가져오는 유틸 메서드
    /// (JSON 세팅에서 필드/프로퍼티 구분 없이 접근 가능)
    /// </summary>
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
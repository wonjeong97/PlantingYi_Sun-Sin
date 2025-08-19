using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuSetting
{
    public ImageSetting backgroundImage;
    public ButtonSetting backToIdleButton;

    public ButtonSetting whatIsButton;      // 플랜팅 이순신이란
    public ButtonSetting childhoodButton;   // 유년
    public ButtonSetting youthButton;       // 청년
    public ButtonSetting primeOfLifeButton; // 장년
    public ButtonSetting lastButton;        // 말년
    public ButtonSetting deathButton;       // 죽음
    public ButtonSetting afterDeathButton;  // 사후

    public PageSetting whatIsPage;          // 플랜팅 이순신이란 페이지
    public PageSetting childhoodPage;       // 유년 페이지
    public PageSetting youthPage;           // 청년 페이지
    public PageSetting primeOfLifePage;     // 장년 페이지    
    public PageSetting lastPage;            // 말년 페이지
    public PageSetting deathPage;           // 죽음 페이지
    public PageSetting afterDeathPage;      // 사후 페이지
}

public class MenuPage : BasePage<MenuSetting>
{
    // JSON 경로
    protected override string JsonPath => "JSON/MenuSetting.json";

    // 생성해 둔 하위 페이지 캐시
    private GameObject whatIsPage;
    private GameObject childhoodPage;
    private GameObject youthPage;
    private GameObject primeOfLifePage;
    private GameObject lastPage;
    private GameObject deathPage;
    private GameObject afterDeathPage;

    protected override async Task BuildContentAsync()
    {
        // 각 네비게이션 버튼 생성 및 와이어링
        await WireNavButton(
            Setting.whatIsButton, // 버튼 데이터
            () => whatIsPage, // 이미 생성된 페이지를 가져오는 함수
            go => whatIsPage = go, // 생성된 페이지를 저장하는 함수
            Setting.whatIsPage, // 생성할 하위 페이지의 설정
            pageGO => // 페이지 생성 후 추가 작업
            {
                var comp = pageGO.AddComponent<WhatIsPage>();
                comp.menuPageInstance = this;
            });

        await WireNavButton(
            Setting.childhoodButton,
            () => childhoodPage,
            go => childhoodPage = go,
            Setting.childhoodPage,
            pageGO =>
            {
                var comp = pageGO.AddComponent<ChildhoodPage>();
                comp.menuPageInstance = this;
            });

        await WireNavButton(
            Setting.youthButton,
            () => youthPage,
            go => youthPage = go,
            Setting.youthPage,
            pageGO =>
            {
                var comp = pageGO.AddComponent<YouthPage>();
                comp.menuPageInstance = this;
            });

        await WireNavButton(
            Setting.primeOfLifeButton,
            () => primeOfLifePage,
            go => primeOfLifePage = go,
            Setting.primeOfLifePage,
            pageGO =>
            {
                var comp = pageGO.AddComponent<PrimeOfLifePage>();
                comp.menuPageInstance = this;
            });

        await WireNavButton(
            Setting.lastButton,
            () => lastPage,
            go => lastPage = go,
            Setting.lastPage,
            pageGO =>
            {
                var comp = pageGO.AddComponent<LastPage>();
                comp.menuPageInstance = this;
            });

        await WireNavButton(
            Setting.deathButton,
            () => deathPage,
            go => deathPage = go,
            Setting.deathPage,
            pageGO =>
            {
                var comp = pageGO.AddComponent<DeathPage>();
                comp.menuPageInstance = this;
            });

        await WireNavButton(
            Setting.afterDeathButton,
            () => afterDeathPage,
            go => afterDeathPage = go,
            Setting.afterDeathPage,
            pageGO =>
            {
                var comp = pageGO.AddComponent<AfterDeathPage>();
                comp.menuPageInstance = this;
            });
    }

    /// <summary>
    /// 공통 네비게이션 버튼 생성 및 핸들러 연결
    /// 1) 버튼 생성
    /// 2) 클릭 시 페이드 아웃 → 본인 비활성화 → 하위 페이지 생성/활성화 → 페이드 인
    /// </summary>
    private async Task WireNavButton(
        ButtonSetting buttonSetting, // 버튼 세팅 데이터
        Func<GameObject> getCache, // 이미 생성된 페이지를 가져오는 함수
        Action<GameObject> setCache, // 생성된 페이지를 저장하는 함수
        PageSetting targetPageSetting, // 생성할 하위 페이지의 설정
        Action<GameObject> onCreatedAttach) // 페이지 생성 후 추가 작업
    {
        if (buttonSetting == null || targetPageSetting == null) return;

        // UIManager를 통해 버튼 생성
        var createdBtn = await UIManager.Instance.CreateSingleButtonAsync(
            buttonSetting, gameObject, CancellationToken.None);

        var btnGO = createdBtn.button;
        if (btnGO != null && btnGO.TryGetComponent<Button>(out var btn))
        {
            // 버튼 클릭 이벤트 등록 (fire-and-forget 패턴)
            btn.onClick.AddListener(() => _ = HandleNavButtonClickedAsync(
                getCache, setCache, targetPageSetting, onCreatedAttach));
        }
    }

    /// <summary>
    /// 네비게이션 버튼 클릭 시 실행되는 처리
    /// </summary>
    private async Task HandleNavButtonClickedAsync(
        Func<GameObject> getCache,
        Action<GameObject> setCache,
        PageSetting targetPageSetting,
        Action<GameObject> onCreatedAttach)
    {
        try
        {
            // 1) 페이드 아웃 및 메뉴 비활성화
            await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.settings.fadeTime, true);
            gameObject.SetActive(false);

            // 2) 페이지 생성 or 재활성화
            var cached = getCache();
            if (cached == null)
            {
                GameObject parent = UIManager.Instance.mainBackground;
                var pageGO = await UIManager.Instance.CreatePageAsync(targetPageSetting, parent);
                if (pageGO != null)
                {
                    onCreatedAttach?.Invoke(pageGO);
                    setCache(pageGO);
                }
            }
            else
            {
                cached.SetActive(true);
                await FadeManager.Instance.FadeInAsync(JsonLoader.Instance.settings.fadeTime, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[MenuPage] Navigation button click failed: {e}");
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Settings jsonSetting;
    private CancellationTokenSource cts;
    private float inactivityTimer;
    private float inactivityThreshold = 60f;
    private GameObject idlePage;

    public GameObject MainBackground { get; private set; }

    #region Unity life-cycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            cts = new CancellationTokenSource();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        try
        {
            if (UICreator.Instance == null)
            {
                Debug.LogError("[UIManager] UI_Creator is null. Place UI_Creator in the scene.");
                return;
            }


            if (JsonLoader.Instance.settings == null)
            {
                Debug.LogError("[UIManager] Settings are not loaded yet.");
                return;
            }


            jsonSetting = JsonLoader.Instance.settings;
            inactivityThreshold = jsonSetting.inactivityTime;


            await InitUI();
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("[UIManager] UI initialization canceled.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIManager] UI initialization failed: {e}");
        }
    }

    private void Update()
    {
        if (idlePage && !idlePage.activeInHierarchy)
        {
            inactivityTimer += Time.deltaTime;
            if (inactivityTimer >= inactivityThreshold)
            {
                inactivityTimer = 0f;
                _ = ShowIdlePageOnly();
            }


            if (Input.anyKeyDown || Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                inactivityTimer = 0f;
            }
        }
    }

    private void OnDestroy()
    {
        try
        {
            cts?.Cancel();
        }
        catch
        {
        }

        cts?.Dispose();
        cts = null;


        // Ensure creator cleans up any remaining instances and cached assets   
        if (UICreator.Instance != null)
        {
            UICreator.Instance.DestroyAllTrackedInstances();
        }
    }

    #endregion


    /// <summary>초기 UI(캔버스/배경/아이들 페이지) 생성 및 연결</summary>
    private async Task InitUI(CancellationToken token = default)
    {
        CancellationToken ct = UIUtility.MergeTokens(cts.Token, token); // 내부 CTS와 외부 토큰 병합
        try
        {
            GameObject canvas = await UICreator.Instance.CreateCanvasAsync(ct); // 캔버스 생성
            MainBackground = await UICreator.Instance.CreateBackgroundImageAsync(
                jsonSetting.mainBackground, canvas, ct); // 메인 배경 생성

            idlePage = await UICreator.Instance.CreatePageAsync(
                jsonSetting.idlePage, MainBackground, ct); // Idle 페이지 생성
            if (idlePage != null)
            {
                idlePage.AddComponent<IdlePage>(); // Idle 동작 스크립트 부착
            }
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("[UIManager] InitUI canceled."); // 취소 로그
            throw; // 취소 전파
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIManager] InitUI failed: {e}"); // 예외 로그
            throw; // 상위에 알림
        }
    }

    /// <summary>페이드로 전환하며 idlePage만 활성화</summary>
    public async Task ShowIdlePageOnly()
    {
        if (!MainBackground || !idlePage) return; // 초기화 전 보호

        try
        {
            float fadeTime = JsonLoader.Instance.settings.fadeTime; // 페이드 시간 조회
            await FadeManager.Instance.FadeOutAsync(fadeTime, true); // 화면 어둡게

            Transform parent = MainBackground.transform; // 자식 순회 대상
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject childGo = parent.GetChild(i).gameObject; // i번째 자식
                if (childGo == idlePage || !childGo.activeSelf) continue; // idlePage거나 이미 비활성화면 건너뜀
                childGo.SetActive(false); // 다른 페이지/팝업 비활성화
            }

            if (!idlePage.activeSelf) idlePage.SetActive(true); // IdlePage 활성화 보장
            await FadeManager.Instance.FadeInAsync(fadeTime, true); // 화면 밝게
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIManager] ShowIdlePageOnly failed: {e}"); // 예외 로깅
        }
    }

    /// <summary>동적으로 생성된 인스턴스를 모두 해제하고 초기 UI 재구성</summary>
    public async Task ClearAllDynamic()
    {
        UICreator.Instance.DestroyAllTrackedInstances();
        await InitUI();
    }
}
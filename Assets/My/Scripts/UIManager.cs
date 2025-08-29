using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private float inactivityTimer;
    private float inactivityThreshold = 60f; // 입력이 없는 경우 타이틀로 되돌아가는 시간
    private GameObject idlePage;
    private Settings jsonSetting;
    private ButtonSetting defaultButtonSetting;

    // Addressable.InstantiateAsync로 만든 동적 오브젝트 추적
    public readonly List<GameObject> addrInstances = new List<GameObject>();
    private CancellationTokenSource cts;
    [HideInInspector] public GameObject mainBackground;

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
        // IdlePage가 아니고, 일정시간 입력이 없을 시 초기화
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
        catch (Exception e)
        {
            Debug.LogWarning($"[UIManager] Failed to cancel token: {e}");
        }

        cts?.Dispose();
        cts = null;

        // 생성했던 Addressable 인스턴스 정리
        for (int i = addrInstances.Count - 1; i >= 0; --i)
        {
            var go = addrInstances[i];
            if (go)
            {
                Addressables.ReleaseInstance(go);
            }
        }

        addrInstances.Clear();
    }

    #endregion


    private async Task InitUI(CancellationToken token = default)
    {
        CancellationToken ct = MergeToken(token);

        try
        {
            GameObject canvas = await CreateCanvasAsync(ct);
            mainBackground = await CreateBackgroundImageAsync(jsonSetting.mainBackground, canvas, ct);
            idlePage = await CreatePageAsync(jsonSetting.idlePage, mainBackground, ct);
            if (idlePage != null)
            {
                idlePage.AddComponent<IdlePage>();
            }
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("[UIManager] InitUI canceled.");
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIManager] InitUI failed: {e}");
            throw;
        }
    }

    /// <summary>
    /// IdlePage를 제외한 모든 페이지를 비활성화함
    /// </summary>
    public async Task ShowIdlePageOnly()
    {
        if (mainBackground is null || idlePage is null) return;
        try
        {
            float fadeTime = JsonLoader.Instance.settings.fadeTime;
            await FadeManager.Instance.FadeOutAsync(fadeTime, true);

            // foreach 대신 for 루프 사용: GC 할당 없음
            var parent = mainBackground.transform;
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childGo = parent.GetChild(i).gameObject;
                if (childGo == idlePage || !childGo.activeSelf) continue;
                childGo.SetActive(false);
            }

            // IdlePage만 다시 활성화
            if (!idlePage.activeSelf) idlePage.SetActive(true);

            await FadeManager.Instance.FadeInAsync(fadeTime, true);
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIManager] ShowIdlePageOnly failed: {e}");
        }
    }

    #region Public API

    public Task<GameObject> CreatePopupChainAsync(PopupSetting[] allPopups, int index, GameObject parent,
        UnityAction<GameObject> onClose = null, CancellationToken token = default)
        => CreatePopupInternalAsync(allPopups, index, parent, onClose, MergeToken(token));

    public Task<GameObject> CreatePageAsync(PageSetting page, GameObject parent, CancellationToken token = default)
        => CreatePageInternalAsync(page, parent, MergeToken(token));

    /// <summary>
    /// Addressable.InstantiateAsync로 동적으로 생성된 모든 GameObject를 해제하고 목록을 비움
    /// </summary>
    public async Task ClearAllDynamic()
    {
        await FadeManager.Instance.FadeOutAsync(JsonLoader.Instance.settings.fadeTime, true);
        for (int i = addrInstances.Count - 1; i >= 0; --i)
        {
            var go = addrInstances[i];
            if (go != null)
            {
                Addressables.ReleaseInstance(go); // Addressable 인스턴스 해제
            }
        }

        addrInstances.Clear(); // 목록 초기화
        await InitUI();
    }

    /// <summary>
    /// Canvas 프리팹을 비동기로 생성하고 기본 UI 구성요소(Canvas, CanvasScaler, GraphicRaycaster)를 보장
    /// </summary>
    private async Task<GameObject> CreateCanvasAsync(CancellationToken token = default)
    {
        // Canvas 프리팹 인스턴스 생성
        var go = await InstantiateAsync("Prefabs/CanvasPrefab.prefab", null, MergeToken(token));
        if (!go) return null;

        return go;
    }

    /// <summary>
    /// 배경용 이미지를 부모 아래에 비동기로 생성하고, 캔버스 좌상단(0,1) 기준으로 고정 배치
    /// </summary>
    public async Task<GameObject> CreateBackgroundImageAsync(ImageSetting setting, GameObject parent,
        CancellationToken token)
    {
        // Addressable로 Image 프리팹 비동기 인스턴스
        var go = await InstantiateAsync("Prefabs/ImagePrefab.prefab", parent.transform, token);
        if (!go) return null;
        go.name = setting.name;

        // Image 컴포넌트가 존재하면 스프라이트/색/타입 적용
        if (go.TryGetComponent<Image>(out var image))
        {
            // StreamingAssets에서 Texture2D 로드 후 Sprite 생성
            var texture = LoadTexture(setting.sourceImage);
            if (texture)
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
            image.color = setting.color;
            image.type = (Image.Type)setting.type;
        }

        // RectTransform 설정
        if (go.TryGetComponent<RectTransform>(out var rt))
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f); // 좌상단 
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.Euler(setting.rotation);
            rt.sizeDelta = setting.size;
        }

        return go;
    }

    /// <summary>
    /// ImageSetting 데이터를 기반으로 부모 객체 아래에 이미지를 비동기로 생성
    /// </summary>
    public async Task<GameObject> CreateImageAsync(ImageSetting setting, GameObject parent, CancellationToken token)
    {
        // Addressable를 통해 ImagePrefab 인스턴스 생성
        var go = await InstantiateAsync("Prefabs/ImagePrefab.prefab", parent.transform, token);
        if (!go) return null;
        go.name = setting.name;

        // Image 컴포넌트가 있으면 텍스처 로드 후 속성 적용
        if (go.TryGetComponent<Image>(out var image))
        {
            var texture = LoadTexture(setting.sourceImage);
            if (texture) // Texture2D를 Sprite로 변환해 적용
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

            image.color = setting.color;
            image.type = (Image.Type)setting.type;
        }

        // RectTransform 속성 적용
        if (go.TryGetComponent<RectTransform>(out var rt))
        {
            rt.anchoredPosition = new Vector2(setting.position.x, -setting.position.y);
            rt.localRotation = Quaternion.Euler(setting.rotation);
            rt.sizeDelta = setting.size;
        }

        return go;
    }

    /// <summary>
    /// 여러 TextSetting을 받아 부모 아래에 비동기로 텍스트들을 생성
    /// </summary>
    private async Task CreateTextsAsync(TextSetting[] settings, GameObject parent, CancellationToken token)
    {
        if (settings == null || settings.Length == 0) return;

        // 병렬 실행할 작업 수에 맞게 리스트 초기 용량 예약
        var tasks = new List<Task>(settings.Length);

        // 각 텍스트 생성 작업 추가
        foreach (var setting in settings)
            tasks.Add(CreateSingleTextAsync(setting, parent, token));

        // 모든 텍스트 생성 완료 대기
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 단일 TextSetting을 기반으로 TextPrefab을 인스턴스화하고 속성을 적용
    /// </summary>
    public async Task CreateSingleTextAsync(TextSetting setting, GameObject parent, CancellationToken token)
    {
        // Text 프리팹 생성 (Addressable)
        var go = await InstantiateAsync("Prefabs/TextPrefab.prefab", parent.transform, token);
        if (!go) return;

        go.name = setting.name;

        // 텍스트 시각 속성 적용
        if (go.TryGetComponent<TextMeshProUGUI>(out var uiText))
        {
            // 폰트는 Addressable에서 로드되며, alignment와 텍스트 등도 함께 적용
            await LoadFontAndApplyAsync(
                uiText,
                setting.fontName,
                setting.text,
                setting.fontSize,
                setting.fontColor,
                setting.alignment,
                token
            );
        }

        // 위치/회전 적용
        if (go.TryGetComponent<RectTransform>(out var rt))
        {
            rt.anchoredPosition = new Vector2(setting.position.x, -setting.position.y);
            rt.localRotation = Quaternion.Euler(setting.rotation);
        }
    }

    /// <summary>
    /// 여러 개의 ButtonSetting 배열을 기반으로 버튼들을 비동기 생성하는 함수
    /// </summary>
    private async Task<List<(GameObject button, GameObject addImage)>> CreateButtonsAsync(
        ButtonSetting[] settings, GameObject parent, CancellationToken token)
    {
        // 반환할 결과 리스트 초기화
        var results = new List<(GameObject button, GameObject addImage)>();
        if (settings == null || settings.Length == 0)
            return results;

        var tasks = new List<Task<(GameObject button, GameObject addImage)>>(settings.Length);
        foreach (var setting in settings)
            tasks.Add(CreateSingleButtonAsync(setting, parent, token));

        var created = await Task.WhenAll(tasks);
        results.AddRange(created);
        return results;
    }

    /// <summary>
    /// ButtonSetting 정보를 기반으로 버튼 UI를 동적으로 생성하고 설정합니다.
    /// </summary>
    public async Task<(GameObject button, GameObject addImage)> CreateSingleButtonAsync(
        ButtonSetting setting, GameObject parent, CancellationToken token)
    {
        // 버튼 프리팹 인스턴스
        var go = await InstantiateAsync("Prefabs/ButtonPrefab.prefab", parent.transform, token);
        if (!go) return (null, null);
        go.name = setting.name;

        // 프리팹 컴포넌트 참조
        var rtBtn = go.GetComponent<RectTransform>();
        var raw = go.GetComponent<RawImage>(); // 배경은 RawImage
        var vp = go.GetComponent<VideoPlayer>(); // 배경 비디오
        var btn = go.GetComponent<Button>();
        var audioSource = go.GetComponent<AudioSource>() ?? go.AddComponent<AudioSource>();

        // 버튼 크기/위치/회전 먼저 적용
        if (rtBtn)
        {
            rtBtn.sizeDelta = setting.size;
            rtBtn.anchoredPosition = new Vector2(setting.position.x, -setting.position.y);
            rtBtn.localRotation = Quaternion.Euler(setting.rotation);
        }

        // 배경: 비디오가 있으면 우선, 실패/미지정이면 이미지
        bool videoApplied = false;
        if (vp != null && setting.buttonBackgroundVideo != null &&
            !string.IsNullOrEmpty(setting.buttonBackgroundVideo.fileName))
        {
            // 버튼 크기에 맞는 RenderTexture를 만들고 연결
            VideoManager.Instance.WireRawImageAndRenderTexture(
                vp, raw,
                new Vector2Int(Mathf.RoundToInt(setting.size.x), Mathf.RoundToInt(setting.size.y))
            );

            // webm 우선, Windows 등에서 안되면 mp4로 자동 폴백
            string url = VideoManager.Instance.ResolvePlayableUrl(setting.buttonBackgroundVideo.fileName);

            bool ok = await VideoManager.Instance.PrepareAndPlayAsync(
                vp, url, audioSource, setting.buttonBackgroundVideo.volume, token);

            if (!ok)
            {
                Debug.LogError($"[Button Video] prepare failed: {url}");
            }
            else
            {
                videoApplied = true;
            }
        }

        // 비디오가 없거나 실패했다면 이미지 사용
        if (!videoApplied && raw != null && setting.buttonBackgroundImage != null)
        {
            var tex = LoadTexture(setting.buttonBackgroundImage.sourceImage);
            if (tex) raw.texture = tex;
            raw.color = setting.buttonBackgroundImage.color;
            // RawImage는 Image.Type이 없으니 type은 별도 처리 불가
        }

        // 텍스트
        var textComp = go.GetComponentInChildren<TextMeshProUGUI>(true);
        if (textComp != null && setting.buttonText != null && !string.IsNullOrEmpty(setting.buttonText.text))
        {
            await LoadFontAndApplyAsync(
                textComp,
                setting.buttonText.fontName,
                setting.buttonText.text,
                setting.buttonText.fontSize,
                setting.buttonText.fontColor,
                setting.buttonText.alignment,
                token
            );

            if (textComp.TryGetComponent<RectTransform>(out var textRT))
            {
                textRT.anchoredPosition = new Vector2(setting.buttonText.position.x, setting.buttonText.position.y);
                textRT.localRotation = Quaternion.Euler(setting.buttonText.rotation);
            }
        }

        // 추가 이미지 (옵션)
        GameObject addImgGo = null;
        if (setting.buttonAdditionalImage != null && !string.IsNullOrEmpty(setting.buttonAdditionalImage.sourceImage))
        {
            addImgGo = await CreateImageAsync(setting.buttonAdditionalImage, go, token);
            if (addImgGo && addImgGo.TryGetComponent<RectTransform>(out var addRT))
            {
                addRT.anchoredPosition = new Vector2(setting.buttonAdditionalImage.position.x,
                    -setting.buttonAdditionalImage.position.y);
                addRT.sizeDelta = setting.buttonAdditionalImage.size;
            }
        }

        // 클릭 사운드
        if (btn)
        {
            var soundKey = setting.buttonSound;
            if (!string.IsNullOrEmpty(soundKey))
                btn.onClick.AddListener(() => { AudioManager.Instance?.Play(soundKey); });
        }

        return (go, addImgGo);
    }

    /// <summary>
    /// VideoSetting 데이터를 기반으로 부모 객체 아래에 VideoPlayer 프리팹을 생성하고 재생
    /// </summary>
    public async Task<GameObject> CreateVideoPlayerAsync(VideoSetting setting, GameObject parent,
        CancellationToken token)
    {
        if (setting == null || string.IsNullOrEmpty(setting.fileName)) return null;

        // 비디오 플레이어 프리팹 로드
        var go = await InstantiateAsync("Prefabs/VideoPlayerPrefab.prefab", parent.transform, token);
        if (!go) return null;
        go.name = setting.name;

        // 컴포넌트 참조
        var vp = go.GetComponent<VideoPlayer>();
        var raw = go.GetComponent<RawImage>();
        var audioSource = go.GetComponent<AudioSource>() ?? go.AddComponent<AudioSource>();

        if (!vp)
        {
            Debug.LogError("[UIManager] Video prefab does not contain VideoPlayer component");
            return go;
        }

        // RectTransform 위치, 크기, 회전 적용
        if (go.TryGetComponent<RectTransform>(out var rt))
        {
            rt.anchoredPosition = new Vector2(setting.position.x, -setting.position.y);
            rt.sizeDelta = setting.size;
            rt.localRotation = Quaternion.Euler(0, 0, 0);
        }

        //RenderTexture 연결
        VideoManager.Instance.WireRawImageAndRenderTexture(
            vp, raw,
            new Vector2Int(Mathf.RoundToInt(setting.size.x), Mathf.RoundToInt(setting.size.y))
        );

        // URL 결정
        var url = VideoManager.Instance.ResolvePlayableUrl(setting.fileName);

        // 준비 및 재생
        var ok = await VideoManager.Instance.PrepareAndPlayAsync(
            vp, url, audioSource, setting.volume, token
        );

        if (!ok)
        {
            Debug.LogError($"[UIManager] Failed to prepare video: {url}");
        }

        return go;
    }

    /// <summary>
    /// PopupSetting 배열과 인덱스를 기반으로 팝업을 생성합니다.
    /// - popupBackgroundImage, popupTexts, popupImages를 생성합니다.
    /// - popupNextButton / popupPreviousButton / popupCloseButton을 연결합니다.
    /// </summary>
    private async Task<GameObject> CreatePopupInternalAsync(
        PopupSetting[] allPopups, int index,
        GameObject parent, UnityAction<GameObject> onClose, CancellationToken token)
    {
        if (allPopups == null || index < 0 || index >= allPopups.Length)
            return null;

        var setting = allPopups[index];

        // 팝업 루트 생성
        var popupRoot = new GameObject(string.IsNullOrEmpty(setting.name) ? "GeneratedPopup" : setting.name);
        if (!popupRoot) return null;

        popupRoot.transform.SetParent(parent.transform, false);
        popupRoot.AddComponent<PopupObject>();

        // 팝업 배경 이미지 설정
        var popupBg = await CreateBackgroundImageAsync(setting.popupBackgroundImage, popupRoot, token);
        if (!popupBg) return popupRoot;

        popupBg.transform.SetAsLastSibling();

        // 텍스트/이미지 병렬 생성
        var allTasks = new List<Task>(2)
        {
            CreateTextsAsync(setting.popupTexts, popupBg, token),
            CreatePopupImagesAsync(setting.popupImages, popupBg, token)
        };
        await Task.WhenAll(allTasks);

        // Next 버튼
        /*if (setting.popupNextButton != null && index < allPopups.Length - 1)
        {
            var (btnGo, _) = await CreateSingleButtonAsync(setting.popupNextButton, popupBg, token);
            if (btnGo && btnGo.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    Destroy(popupRoot);
                    _ = CreatePopupInternalAsync(allPopups, index + 1, parent, onClose, token);
                });
            }
        }*/

        // Prev 버튼
        /*if (setting.popupPreviousButton != null && index > 0)
        {
            var (btnGo, _) = await CreateSingleButtonAsync(setting.popupPreviousButton, popupBg, token);
            if (btnGo && btnGo.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    Destroy(popupRoot);
                    _ = CreatePopupInternalAsync(allPopups, index - 1, parent, onClose, token);
                });
            }
        }*/

        // 내부 팝업 버튼들 생성
        var popup2Btns = await CreateButtonsAsync(setting.popup2Buttons, popupBg, token);
        foreach (var (btnGo2, _) in popup2Btns)
        {
            if (btnGo2 != null && btnGo2.TryGetComponent<Button>(out var btn2))
            {
                // 팝업 내 버튼2 세팅을 가져옴
                var buttonSetting = setting.popup2Buttons.FirstOrDefault(b => b.name == btnGo2.name);
                if (buttonSetting == null) continue;

                btn2.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(buttonSetting.targetPopupName))
                    {
                        // 부모 페이지에서 SubPopups 가져오기
                        var page = parent.GetComponentInParent<IPageWithSubPopups>();
                        var subPopups = page?.GetSubPopups();

                        if (subPopups != null)
                        {
                            var target = subPopups.FirstOrDefault(p => p.name == buttonSetting.targetPopupName);
                            if (target != null)
                            {
                                int idx = Array.IndexOf(subPopups, target);
                                _ = CreatePopupInternalAsync(subPopups, idx, parent, null, token);
                            }
                            else
                            {
                                Debug.LogWarning($"[UIManager] SubPopup not found: {buttonSetting.targetPopupName}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("[UIManager] No subPopups found on parent page");
                        }
                    }
                });
            }
        }

        // Close 버튼
        if (setting.popupCloseButton != null)
        {
            var (btnGo, _) = await CreateSingleButtonAsync(setting.popupCloseButton, popupBg, token);
            if (btnGo && btnGo.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    onClose?.Invoke(popupRoot);
                    popupRoot.SetActive(false); // OnDisable에서 해제/파괴 처리
                });
            }
        }

        return popupRoot;
    }

    /// <summary>
    /// 팝업에 사용할 이미지들을 비동기로 생성
    /// </summary>
    private async Task CreatePopupImagesAsync(ImageSetting[] images, GameObject parent, CancellationToken token)
    {
        if (images == null || images.Length == 0) return;

        // 병렬 실행 작업 리스트 준비
        var tasks = new List<Task>(images.Length);

        // 각 이미지 생성 작업 추가
        foreach (var img in images)
            tasks.Add(CreateImageAsync(img, parent, token));

        // 모든 이미지 생성이 완료될 때까지 대기
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// PageSetting 정보를 기반으로 하나의 페이지를 동적으로 생성
    /// </summary>
    private async Task<GameObject> CreatePageInternalAsync(PageSetting page, GameObject parent, CancellationToken token)
    {
        // 페이지 루트 오브젝트 생성
        var pageRoot = new GameObject(string.IsNullOrEmpty(page.name) ? "GeneratedPage" : page.name);

        // 부모 지정 (worldPositionStays=false로 로컬 위치 유지)        
        pageRoot.transform.SetParent(parent.transform, false);
        var rt = pageRoot.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(page.position.x, -page.position.y);
        rt.sizeDelta = page.size;

        // 병렬 실행 작업 준비
        var jobs = new List<Task>(4);
        {
            jobs.Add(CreateTextsAsync(page.texts, pageRoot, token)); // 텍스트 생성
            jobs.Add(CreatePopupImagesAsync(page.images, pageRoot, token)); // 이미지 생성
            jobs.Add(CreateButtonsAsync(page.buttons, pageRoot, token)); // 버튼 생성
            // TODO: CreateKeyboardsAsync(page.keyboards, pageRoot, token)  // 키보드 UI 생성
            // TODO: CreateVideosAsync(page.videos, pageRoot, token)        // 비디오 UI 생성
        }
        // 모든 생성 작업 완료 대기
        await Task.WhenAll(jobs);

        return pageRoot;
    }

    #endregion

    #region Utilities (Addressables, Fonts, Materials, Files)

    /// <summary>
    ///  Addressable를 사용해 비동기로 프리팹을 인스턴스화(Instantiate)
    /// </summary>
    private async Task<GameObject> InstantiateAsync(string key, Transform parent, CancellationToken token)
    {
        // 호출 즉시 취소 요청이 있었는지 확인함
        token.ThrowIfCancellationRequested();

        var handle = Addressables.InstantiateAsync(key, parent); // 비동기 에셋 생성
        try
        {
            // 취소 토큰을 고려한 Await
            var go = await AwaitWithCancellation(handle, token);

            // 정상 생성시 추적 리스트에 추가
            if (go != null) addrInstances.Add(go);
            return go;
        }
        catch (OperationCanceledException)
        {
            // 생성이 완료된 상태라면 즉시 해제하여 메모리 / 참조 누수 방지
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded && handle.Result)
                Addressables.ReleaseInstance(handle.Result);
            throw;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[UIManager] Instantiate failed: {key}\n{e}");

            // 생성된 경우 즉시 해제
            if (handle.IsValid() && handle.Result)
                Addressables.ReleaseInstance(handle.Result);
            return null;
        }
    }

    /// <summary>
    /// Addressable를 사용하여 비동기로 에셋을 로드
    /// </summary>
    private async Task<T> LoadAssetAsync<T>(string key, CancellationToken token) where T : UnityEngine.Object
    {
        // 호출 시 즉시 취소 요청 확인
        token.ThrowIfCancellationRequested();

        // Addressable로 비동기 에셋 로드
        var handle = Addressables.LoadAssetAsync<T>(key);
        try
        {
            return await AwaitWithCancellation(handle, token); // 취소 가능 대기
        }
        finally
        {
            // 로드 완료 혹은 취소 시 handle 해제
            if (handle.IsValid())
                Addressables.Release(handle);
        }
    }

    /// <summary>
    /// Addressable의 AsyncOperationHandle을 await하면서 CancellationToken 지원
    /// </summary>
    private static async Task<T> AwaitWithCancellation<T>(AsyncOperationHandle<T> handle, CancellationToken token)
    {
        // 취소 신호를 감지할 Task 생성
        var tcs = new TaskCompletionSource<bool>();

        // 토큰이 취소되면 tcs를 완료시켜 Task.WhenAny에서 취소를 먼저 감지할 수 있도록 함
        await using (token.Register(() => tcs.TrySetResult(true)))
        {
            // 로드 완료(handle.Task)와 취소(tcs.Task) 중 하나가 먼저 끝날 때까지 대기
            var completed = await Task.WhenAny(handle.Task, tcs.Task);

            // 취소 Task가 먼저 끝난 경우
            if (completed == tcs.Task) throw new OperationCanceledException(token);

            // 정상적으로 handle.Task가 끝난 경우 결과 반환
            return await handle.Task;
        }
    }

    /// <summary>
    /// StreamingAssets 경로에서 Texture2D를 로드
    /// </summary>
    private Texture2D LoadTexture(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return null;

        var fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        if (!File.Exists(fullPath)) return null;

        byte[] fileData = File.ReadAllBytes(fullPath);
        var texture = new Texture2D(2, 2); // 임시 크기, LoadImage 시 자동 변경
        texture.LoadImage(fileData);

        return texture;
    }

    /// <summary>
    /// 폰트 키를 실제 Addressable 리소스 키로 변환
    /// </summary>
    private string ResolveFont(string key)
    {
        var fontMap = JsonLoader.Instance.settings.fontMap;
        if (fontMap == null) return key; // fontMap이 없으면 변환 없이 반환

        // fontMap에서 해당 키에 해당하는 필드 찾음
        var field = typeof(FontMaps).GetField(key);
        if (field != null)
            return field.GetValue(fontMap) as string ?? key;
        return key;
    }

    /// <summary>
    /// Addressable에서 폰트를 비동기로 로드하여 지정한 TextMeshProUGUI에 적용
    /// </summary>
    private async Task LoadFontAndApplyAsync(TextMeshProUGUI uiText, string fontKey, string textValue,
        float fontSize,
        Color fontColor, TextAlignmentOptions alignment, CancellationToken token)
    {
        if (!uiText || string.IsNullOrEmpty(fontKey)) return;

        // fontKey를 fontMap에서 실제 리소스 키로 변환
        string mappedFontName = ResolveFont(fontKey);
        try
        {
            var font = await LoadAssetAsync<TMP_FontAsset>(mappedFontName, token);
            token.ThrowIfCancellationRequested(); // 로드 도중 취소되었는지 최종 확인

            uiText.font = font;
            uiText.fontSize = fontSize;
            uiText.color = fontColor;
            uiText.alignment = alignment;
            uiText.text = textValue; // 폰트 속성 적용
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"[UIManager] Font load canceled: {mappedFontName}\n{textValue}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[UIManager] Font load failed: {mappedFontName}\n{e}");
        }
    }

    /// <summary>
    /// Addressable에서 Material을 비동기로 로드하여 지정한 Image 컴포넌트에 적용
    /// </summary>
    private async Task LoadMaterialAndApplyAsync(Image targetImage, string materialKey, CancellationToken token)
    {
        // 대상 이미지 또는 키가 없으면 바로 실패 처리
        if (!targetImage || string.IsNullOrEmpty(materialKey)) return;
        try
        {
            // Addressable에서 Material 로드
            var mat = await LoadAssetAsync<Material>(materialKey, token);
            token.ThrowIfCancellationRequested();
            targetImage.material = mat;
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"[UIManager] Material load canceled: {materialKey}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[UIManager] Material load failed: {materialKey}\n{e}");
        }
    }

    /// <summary>
    ///  Addressable로 생성된 인스턴스를 안전하게 해제
    /// </summary>
    private void SafeReleaseInstance(GameObject go)
    {
        if (!go) return; // 유효하지 않으면 종료
        Addressables.ReleaseInstance(go); // Addressable 인스턴스 해제
        addrInstances.Remove(go); // 추적 리스트에서 제거
    }

    /// <summary>
    /// 내부 CancellationToken(cts.Token)과 외부 토큰을 병합하여 단일 토큰 반환.
    /// </summary>
    private CancellationToken MergeToken(CancellationToken external)
    {
        if (cts == null) return external; // 내부 토큰이 없으면 외부 토큰 그대로 사용
        if (!external.CanBeCanceled)
            return cts.Token; // 외부 토큰이 취소 불가능하면 내부 토큰만 사용

        var linked = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, external);
        return linked.Token;
    }

    #endregion
}
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIUtility : MonoBehaviour
{
 /// <summary>
    ///  Addressable를 사용해 비동기로 프리팹을 인스턴스화(Instantiate)
    /// </summary>
    /// <param name="key">Addressable에서 로드할 프리팹 키</param>
    /// <param name="parent">생성할 오브젝트의 부모 Transform</param>
    /// <param name="token">생성 작업을 취소할 수 있는 CancellationToken</param>
    /// <returns>생성된 GameObject, 실패 시 null</returns>
    private async Task<GameObject> InstantiateAsync(string key, Transform parent, CancellationToken token)
    {
        // 호출 즉시 취소 요청이 있었는지 확인함
        token.ThrowIfCancellationRequested();

        // Addressable로 비동기 인스턴스 생성 시작
        var handle = Addressables.InstantiateAsync(key, parent);
        try
        {
            // 취소 토큰을 고려한 Await
            var go = await AwaitWithCancellation(handle, token);

            // 정상 생성시 추적 리스트에 추가
            if (go != null && UIManager.Instance) UIManager.Instance.addrInstances.Add(go);
            return go;
        }
        catch (OperationCanceledException) // 취소된 경우
        {
            // 생성이 완료된 상태라면 즉시 해제하여 메모리 / 참조 누수 방지
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded && handle.Result)
                Addressables.ReleaseInstance(handle.Result);
            throw;
        }
        catch (Exception e) // 생성 중 예외 발생 시
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
    /// <typeparam name="T"></typeparam>
    /// <param name="key"> Addressable에서 로드할 에셋 키</param>
    /// <param name="token">로드 작업을 취소할 수 있는 CancellationToken</param>
    /// <returns>로드된 에셋(T 타입), 취소 시 OperationCanceledException 발생</returns>
    private async Task<T> LoadAssetAsync<T>(string key, CancellationToken token) where T : UnityEngine.Object
    {
        // 호출 시 즉시 취소 요청 확인
        token.ThrowIfCancellationRequested();

        // Addressable로 비동기 에셋 로
        var handle = Addressables.LoadAssetAsync<T>(key);

        try
        {
            // 취소 가능 대기
            return await AwaitWithCancellation(handle, token);
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
    /// <typeparam name="T"></typeparam>
    /// <param name="handle">Addressable 비동기 로드/생성 핸들</param>
    /// <param name="token">취소 시 OperationCanceledException 발생</param>
    /// <returns>정상 완료 시 handle.Task의 결과 반환</returns>
    /// <exception cref="OperationCanceledException"></exception>
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
            if (completed == tcs.Task)
                throw new OperationCanceledException(token);

            // 정상적으로 handle.Task가 끝난 경우 결과 반환
            return await handle.Task;
        }
    }

    /// <summary>
    /// StreamingAssets 경로에서 Texture2D를 로드
    /// </summary>
    /// <param name="relativePath">StreamingAssets 하위의 상대 경로</param>
    /// <returns>로드된 Texture2D 객체, 실패 시 null</returns>
    private Texture2D LoadTexture(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return null;

        // StreamingAssets 폴더 기준 전체 경로 생성
        var fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);

        // 파일 존재 여부 확인
        if (!File.Exists(fullPath)) return null;

        // 바이너리로 읽어서 Texture2D 생성
        byte[] fileData = File.ReadAllBytes(fullPath);
        var texture = new Texture2D(2, 2); // 임시 크기, LoadImage 시 자동 변경
        texture.LoadImage(fileData);

        return texture;
    }

    /// <summary>
    /// 폰트 키를 실제 Addressable 리소스 키로 변환
    /// </summary>
    /// <param name="key">JSON 설정에서 지정한 폰트 맵핑 키</param>
    /// <returns> 매핑된 폰트 이름(또는 원본 키)</returns>
    private string ResolveFont(string key)
    {
        var fontMap = JsonLoader.Instance.settings.fontMap;

        // fontMap이 없으면 변환 없이 반환
        if (fontMap == null) return key;

        // fontMap에서 해당 키에 해당하는 필드 찾기
        var field = typeof(FontMaps).GetField(key);
        if (field != null)
            return field.GetValue(fontMap) as string ?? key;
        return key;
    }

    /// <summary>
    /// Addressable에서 폰트를 비동기로 로드하여 지정한 TextMeshProUGUI에 적용
    /// </summary>
    /// <param name="uiText">폰트를 적용할 TextMeshProUGUI 컴포넌트</param>
    /// <param name="fontKey">JSON 설정에서 정의된 폰트 키</param>
    /// <param name="textValue">적용할 문자열</param>
    /// <param name="fontSize">글자 크기</param>
    /// <param name="fontColor">글자 색상</param>
    /// <param name="alignment">텍스트 정렬 방식</param>
    /// <param name="token">취소 토큰 (작업 중단 가능)</param>
    /// <returns>성공 여부 (true: 적용 성공, false: 실패 또는 취소)</returns>
    private async Task<bool> LoadFontAndApplyAsync(TextMeshProUGUI uiText, string fontKey, string textValue,
        float fontSize,
        Color fontColor, TextAlignmentOptions alignment, CancellationToken token)
    {
        // UI 텍스트 객체나 폰트 키가 없으면 실패 처리
        if (!uiText || string.IsNullOrEmpty(fontKey)) return false;

        // 로드 중 깜박임 방지를 위해 비활성화
        uiText.enabled = false;

        // fontKey를 fontMap에서 실제 리소스 키로 변환
        string mappedFontName = ResolveFont(fontKey);

        try
        {
            // Addressable에서 TMP_FontAsset 로드
            var font = await LoadAssetAsync<TMP_FontAsset>(mappedFontName, token);

            // 로드 도중 취소되었는지 최종 확인
            token.ThrowIfCancellationRequested();

            // 폰트 속성 적용
            uiText.font = font;
            uiText.fontSize = fontSize;
            uiText.color = fontColor;
            uiText.alignment = alignment;
            uiText.text = textValue;

            // 적용 후 활성화
            uiText.enabled = true;

            return true;
        }
        catch (OperationCanceledException)
        {
            // 취소된 경우 로그 출력 후 false 반환
            Debug.Log("load failed");
            return false;
        }
        catch (Exception e)
        {
            // 로드 실패 예외 로그
            Debug.LogWarning($"[UIManager] Font load failed: {mappedFontName}\n{e}");
            return false;
        }
    }

    /// <summary>
    /// Addressable에서 Material을 비동기로 로드하여 지정한 Image 컴포넌트에 적용
    /// </summary>
    /// <param name="targetImage">머티리얼을 적용할 UI Image 컴포넌트</param>
    /// <param name="materialKey">Addressable에 등록된 머티리얼 키</param>
    /// <param name="token">취소 토큰 (작업 도중 취소 가능)</param>
    /// <returns>성공 여부 (true: 적용 성공, false: 실패 또는 취소)</returns>
    private async Task<bool> LoadMaterialAndApplyAsync(Image targetImage, string materialKey, CancellationToken token)
    {
        // 대상 이미지 또는 키가 없으면 바로 실패 처리
        if (!targetImage || string.IsNullOrEmpty(materialKey)) return false;

        try
        {
            // Addressable에서 Material 로드
            var mat = await LoadAssetAsync<Material>(materialKey, token);

            // 로드 도중 취소 요청이 있었는지 최종 확인
            token.ThrowIfCancellationRequested();

            // 머티리얼을 이미지에 적용
            targetImage.material = mat;
            return true;
        }
        catch (OperationCanceledException)
        {
            // 취소된 경우 false 반환
            return false;
        }
        catch (Exception e)
        {
            // 로드 실패 시 경고 출력
            Debug.LogWarning($"[UIManager] Material load failed: {materialKey}\n{e}");
            return false;
        }
    }

    /// <summary>
    ///  Addressable로 생성된 인스턴스를 안전하게 해제
    /// </summary>
    /// <param name="go">해제할 GameObject 인스턴스</param>
    private void SafeReleaseInstance(GameObject go)
    {
        if (!go && !UIManager.Instance) return; // 유효하지 않으면 종료
        Addressables.ReleaseInstance(go); // Addressable 인스턴스 해제
        UIManager.Instance.addrInstances.Remove(go); // 추적 리스트에서 제거
    }

    /// <summary>
    /// 내부 CancellationToken(cts.Token)과 외부 토큰을 병합하여 단일 토큰 반환.
    /// </summary>
    /// <param name="external">외부에서 전달된 CancellationToken</param>
    /// <returns>병합된 CancellationToken</returns>
    private CancellationToken MergeToken(CancellationToken external)
    {
        if (!UIManager.Instance || UIManager.Instance.cts == null) return external; // 내부 토큰이 없으면 외부 토큰 그대로 사용

        if (!external.CanBeCanceled)
            return UIManager.Instance.cts.Token; // 외부 토큰이 취소 불가능하면 내부 토큰만 사용

        // 두 토큰을 병합하여 하나의 LinkedToken 생성
        var linked = CancellationTokenSource.CreateLinkedTokenSource(UIManager.Instance.cts.Token, external);

        return linked.Token;
    }
}

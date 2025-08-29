using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class PopupObject : MonoBehaviour
{
    private bool isDestroying;

    private void OnDisable()
    {
        if (isDestroying) return;
        isDestroying = true;

        var children = new List<Transform>();
        foreach (Transform t in transform) children.Add(t);

        var ui = UIManager.Instance;
        
        foreach (var t in children)
        {
            var go = t.gameObject;

            // VideoPlayer가 만든 RenderTexture 정리
            var vps = go.GetComponentsInChildren<VideoPlayer>(true);
            foreach (var vp in vps)
            {
                var rt = vp.targetTexture;
                if (rt != null)
                {
                    rt.Release();
                    Destroy(rt);
                    vp.targetTexture = null;
                }
            }

            // Addressables로 생성된 것만 해제, 아니면 일반 Destroy
            if (ui != null && ui.addrInstances.Remove(go))
            {
                Addressables.ReleaseInstance(go);
            }
            else
            {
                Destroy(go);
            }
        }

        Destroy(gameObject);
    }
    
}

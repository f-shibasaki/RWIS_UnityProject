using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class EffectsManager : MonoBehaviour
{
    [SerializeField] private GameObject _lineClearEffectPrefab;

    private static float duration = 0.5f;

    public async UniTask PlayLineClearEffect(int height)
    {
        GameObject vfxObject = Instantiate(_lineClearEffectPrefab, transform);
        VisualEffect vfx = vfxObject.GetComponent<VisualEffect>();
        vfx.SetInt("height", height);
        vfx.SetFloat("duration", duration);
        vfx.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        Destroy(vfxObject);
    }
    
    public async UniTask PlayLineClearEffect(List<int> lines)
    {
        List<GameObject> vfxObjects = new List<GameObject>();

        foreach (var line in lines)
        {
            GameObject vfxObject = Instantiate(_lineClearEffectPrefab, transform);
            vfxObjects.Add(vfxObject);
            VisualEffect vfx = vfxObject.GetComponent<VisualEffect>();
            vfx.SetInt("height", line);
            vfx.SetFloat("duration", duration);
            vfx.Play();
        }
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        foreach (var vfxObject in vfxObjects)
        {
            Destroy(vfxObject);
        }
    }
}
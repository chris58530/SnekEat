using System;
using UnityEngine;
using UnityEngine.UI;

public class SnekkiesAssetSetting : ScriptableObject
{
    public SnekkiesAsset[] snekkiesAssets;

    public Sprite GetSnekkiesAssetImage(int unit)
    {
        foreach (var asset in snekkiesAssets)
        {
            if (asset.ID == unit)
            {
                return asset.sprite;
            }
        }
        return null;
    }
}
[Serializable]
public class SnekkiesAsset
{
    public int ID;
    public Sprite sprite;  // 對應的圖片資源
}

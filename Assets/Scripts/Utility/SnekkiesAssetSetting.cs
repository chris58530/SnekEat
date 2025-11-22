using System;
using UnityEngine;
using UnityEngine.UI;

public class SnekkiesAssetSetting : ScriptableObject
{
    public SnekkiesAsset[] snekkiesAssets;

    public Image GetSnekkiesAssetImage(string unit)
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
    public string ID;
    public Image sprite;  // 對應的圖片資源
}

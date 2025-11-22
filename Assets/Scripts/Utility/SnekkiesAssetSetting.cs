using System;
using UnityEngine;
using UnityEngine.UI;

public class SnekkiesAssetSetting : ScriptableObject
{
    public SnekkiesAsset[] snekkiesAssets;

    public SnekkiesAsset GetSnekkiesAsset(int id)
    {
        foreach (var asset in snekkiesAssets)
        {
            if (asset.ID == id)
            {
                return asset;
            }
        }
        return null;
    }
}
[Serializable]
public class SnekkiesAsset
{
    public int ID;
    public Sprite UISprite;
    public Sprite head;
    public Sprite body;
}

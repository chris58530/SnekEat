using System;
using UnityEngine;
using UnityEngine.UI;

public class SnekUIObjectView : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private Image uiImage;

    private Action<int> onClickCallback;

    public void Setup(int id, Sprite sprite, Action<int> onClickCallback)
    {
        this.id = id;
        uiImage.sprite = sprite;
    }

    public void OnClick()
    {
        Debug.Log($"SnekUIObjectView clicked with ID: {id}");
        onClickCallback?.Invoke(id);
    }
}

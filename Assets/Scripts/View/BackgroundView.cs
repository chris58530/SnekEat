using UnityEngine;
using Core.MVC;
using System;

public class BackgroundView : BaseView<BackgroundViewMediator>
{
    [SerializeField] private BackgroundSetting[] backgroundSettings;

    public void SetBackground(BackgroundType type)
    {
        foreach (var setting in backgroundSettings)
        {
            if (setting.backgroundType == type)
            {
                setting.backgroundObject.SetActive(true);
            }
            else
            {
                setting.backgroundObject.SetActive(false);
            }
        }
    }

}
[Serializable]
public class BackgroundSetting
{
    public BackgroundType backgroundType;
    public GameObject backgroundObject;
}
public enum BackgroundType
{
    Maze,
    Pepe
}

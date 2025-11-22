using UnityEngine;
using Zenject;
using Core.MVC;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // 綁定 MVC 事件中心 Listener
        Container.Bind<Listener>().AsSingle();

        // 這裡可以綁定其他全域或場景內的依賴
        // 例如:
        // Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
    }
}

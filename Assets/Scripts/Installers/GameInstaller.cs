using UnityEngine;
using Zenject;
using Core.MVC;
using System.Linq;
using System.Reflection;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // 綁定 MVC 事件中心 Listener
        Container.Bind<Listener>().AsSingle();

        // 自動綁定所有繼承自 IMediator 的類別
        BindAllMediators();

        // 自動綁定所有繼承自 IProxy 的類別
        BindAllProxies();
    }

    private void BindAllMediators()
    {
        // 取得當前 Assembly 中所有繼承自 IMediator 且不是抽象類別的型別
        var mediatorTypes = Assembly.GetAssembly(typeof(IMediator))
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(IMediator)));

        foreach (var type in mediatorTypes)
        {
            // 綁定為 Single (單例)
            Container.Bind(type).AsSingle();
            // Debug.Log($"[AutoBind] Mediator: {type.Name}");
        }
    }

    private void BindAllProxies()
    {
        // 取得當前 Assembly 中所有繼承自 IProxy 且不是抽象類別的型別
        var proxyTypes = Assembly.GetAssembly(typeof(IProxy))
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(IProxy)));

        foreach (var type in proxyTypes)
        {
            // 綁定為 Single (單例)
            Container.Bind(type).AsSingle();
            // Debug.Log($"[AutoBind] Proxy: {type.Name}");
        }
    }
}

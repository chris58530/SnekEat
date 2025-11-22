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

        // 初始化 InjectService，讓 View 可以使用補救注入
        InjectService.Instance.SetContainer(Container);
        Debug.Log("[GameInstaller] Bindings Installed & InjectService Ready.");
    }

    private void BindAllMediators()
    {
        // 使用 Installer 所在的 Assembly 進行搜尋，確保能找到遊戲邏輯中的類別
        // 避免因為 MVC Core 在不同 Assembly 而漏掉
        var targetAssembly = this.GetType().Assembly;

        var mediatorTypes = targetAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IMediator).IsAssignableFrom(t));

        foreach (var type in mediatorTypes)
        {
            // 避免重複綁定
            if (!Container.HasBinding(type))
            {
                Container.Bind(type).AsSingle();
                Debug.Log($"[AutoBind] Mediator: {type.Name}");
            }
        }
    }

    private void BindAllProxies()
    {
        var targetAssembly = this.GetType().Assembly;

        var proxyTypes = targetAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IProxy).IsAssignableFrom(t));

        foreach (var type in proxyTypes)
        {
            if (!Container.HasBinding(type))
            {
                Container.Bind(type).AsSingle();
                Debug.Log($"[AutoBind] Proxy: {type.Name}");
            }
        }
    }
}

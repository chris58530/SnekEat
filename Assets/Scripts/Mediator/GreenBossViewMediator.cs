using Core.MVC;
using UnityEngine;
using Zenject;

public class GreenBossViewMediator : BaseMediator<GreenBossView>
{
    [Inject] private BossProxy bossProxy;


}

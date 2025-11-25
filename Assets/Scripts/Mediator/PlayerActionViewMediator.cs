using Core.MVC;
using UnityEngine;
using Zenject;

public class PlayerActionViewMediator : BaseMediator<PlayerActionView>
{
    public void OnShoot()
    {
        listener.BroadCast(PlayerActionEvent.ON_PLAYER_SHOOT);
    }

    public void OnDash()
    {
        listener.BroadCast(PlayerActionEvent.ON_PLAYER_DASH);
    }
}

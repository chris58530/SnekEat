using Core.MVC;
using UnityEngine;

public class MazeGenerateViewMediator : BaseMediator<MazeGenerateView>
{
    [Listener(GameEvent.ON_GAME_INIT)]
    public void OnGameInit()
    {
        view.GenerateMaze(160, 90, 1);
    }
}

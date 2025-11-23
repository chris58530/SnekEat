using Core.MVC;

public class ScoreGenerateViewMediator : BaseMediator<ScoreGenerateView>
{
    [Listener(GameEvent.ON_GAME_INIT)]
    public void OnGameInit()
    {
        view.GenerateScores();
    }
}

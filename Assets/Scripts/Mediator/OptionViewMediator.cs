using Core.MVC;
using Zenject;

public class OptionViewMediator : BaseMediator<OptionView>
{
    [Inject] private GameProxy gameProxy;
    public void OnClickStart()
    {
        gameProxy.SetCurrentStage(GameStage.Playing);
    }
}

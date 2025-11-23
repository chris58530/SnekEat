using Core.MVC;
using Zenject;

public class BackgroundViewMediator : BaseMediator<BackgroundView>
{
    [Inject] private BackgroundProxy backgroundProxy;

    [Listener(BackgroundEvent.ON_SETUP_BACKGROUND)]
    public void SetBackground()
    {
        BackgroundType type = backgroundProxy.backgroundType;
        view.SetBackground(type);
    }
}

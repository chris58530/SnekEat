using Core.MVC;

public class BackgroundProxy : IProxy
{
    public BackgroundType backgroundType;

    public void SetBackground(BackgroundType type)
    {
        backgroundType = type;
        listener.BroadCast(BackgroundEvent.ON_SETUP_BACKGROUND);
    }
}

using UnityEngine;
using Core.MVC;

public class OptionView : BaseView<OptionViewMediator>
{
    public void OnClickStart()
    {
        mediator.OnClickStart();
    }
}

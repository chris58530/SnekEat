using Core.MVC;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DebugView : MonoBehaviour
{
    [Inject] private GameProxy gameProxy;
    private void Awake()
    {
        InjectService.Instance.Inject(this);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void OnClickMenu()
    {
        Debug.Log("DebugView: OnClickMenu");
        gameProxy.SetCurrentStage(GameStage.Menu);
    }
}

using Core.MVC;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionView : BaseView<PlayerActionViewMediator>
{
    [SerializeField] private Button shootButton;
    [SerializeField] private Button dashButton;

    protected override void Awake()
    {
        base.Awake();
        shootButton?.onClick.AddListener(OnShootClick);
        dashButton?.onClick.AddListener(OnDashClick);
    }

    private void Update()
    {
        // Keyboard shortcuts for testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnShootClick();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnDashClick();
        }
    }

    private void OnShootClick()
    {
        mediator.OnShoot();
    }

    private void OnDashClick()
    {
        mediator.OnDash();
    }
}

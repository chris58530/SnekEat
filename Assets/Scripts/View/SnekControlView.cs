using System;
using Core.MVC;
using UnityEngine;

public class SnekControlView : BaseView<SnekControlViewMediator>
{
    [SerializeField] private SnekObjectView snekObjectView;
    [SerializeField] private Joystick joystickPrefab;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private InputType inputType = InputType.PC;

    private SnekInputController inputController;
    private Joystick currentJoystick;

    [SerializeField] private int initialBodyLength = 30;
    [SerializeField] private bool isDebug;
    private int singleBodyLength = 10;
    private int currentLength = 0;
    private int speed = 5;

    protected override void Awake()
    {
        base.Awake();

        inputController = new SnekInputController();

        if (inputType == InputType.Mobile && joystickPrefab != null)
        {
            if (uiCanvas != null)
            {
                currentJoystick = Instantiate(joystickPrefab, uiCanvas.transform);
            }
            else
            {
                Debug.LogWarning("UI Canvas not assigned in SnekControlView. Instantiating Joystick under this object, which might not work if it's not a UI element.");
                currentJoystick = Instantiate(joystickPrefab, transform);
            }
        }

        if (inputType == InputType.PC)
        {
            inputController.SetStrategy(new PCInputStrategy());
        }
        else
        {
            inputController.SetStrategy(new MobileInputStrategy(currentJoystick));
        }
        snekObjectView.onGetScore = OnGetScore;
        snekObjectView.onEnterPortal = OnEnterPortal;
        snekObjectView.onStartEnterPortal = OnStartEnterPortal;
        snekObjectView.onHit = OnHit;
    }

    public void OnStartEnterPortal(Transform portalTransform)
    {
        mediator.OnStartEnterPortal(snekObjectView.transform, portalTransform);
    }

    public void EnableMove(bool canMove)
    {
        snekObjectView.EnabledMove(canMove);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            currentLength += singleBodyLength;
            UpdateSnekLength();
        }

        Vector2 direction = inputController.GetMoveDirection();

        if (snekObjectView != null)
        {
            snekObjectView.ManualUpdate(direction);
        }
    }

    private void Start()
    {
        currentLength = initialBodyLength;
        UpdateSnekLength();
    }

    public void SetupRunnerSkin(SnekkiesAsset skinAsset)
    {
        snekObjectView.Setup(skinAsset, () =>
        {
            UpdateSnekLength();
            mediator.OnRunnerSkinSetupComplete(snekObjectView.transform);
            snekObjectView.canMove = true;
        });
    }

    public void UpdateSnekLength()
    {
        snekObjectView.SetBodyLength(currentLength);
    }

    public void UpdateSnekSpeed(int newSpeed)
    {
        speed = newSpeed;
        snekObjectView.SetSpeed(speed);
    }

    public void OnGetScore(ScoreObjectView scoreObjectView)
    {
        currentLength += singleBodyLength;
        UpdateSnekLength();
        Destroy(scoreObjectView.gameObject);
    }

    public void OnEnterPortal(Transform portalTransform, Quaternion entryRotation)
    {
        snekObjectView.EnabledMove(false);
        mediator.OnEnterPortal(() =>
        {
            snekObjectView.ExitPortal(() =>
            {
                snekObjectView.EnabledMove(true);
                mediator.OnExitPortalComplete();
            });
        });
    }



    public void OnShoot()
    {
        if (currentLength > singleBodyLength || isDebug)
        {
            if (currentLength > singleBodyLength)
            {
                currentLength -= singleBodyLength;
                UpdateSnekLength();
            }
            snekObjectView.Shoot();
        }
    }

    public void OnDash()
    {
        if (currentLength > singleBodyLength || isDebug)
        {
            if (currentLength > singleBodyLength)
            {
                currentLength -= singleBodyLength;
                UpdateSnekLength();
            }
            snekObjectView.Dash();
        }
    }

    private void OnHit()
    {
        if (currentLength > singleBodyLength || isDebug)
        {
            if (currentLength > singleBodyLength)
            {
                currentLength -= singleBodyLength;
                UpdateSnekLength();
            }
        }
        mediator.RequestCameraShake();
    }

    public void SetDebugMode(bool debug)
    {
        this.isDebug = debug;
    }

    public void AddLengthDebug()
    {
        currentLength += singleBodyLength;
        UpdateSnekLength();
    }
}

public enum InputType
{
    PC,
    Mobile
}

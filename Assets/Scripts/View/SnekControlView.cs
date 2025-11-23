using Core.MVC;
using UnityEngine;

public class SnekControlView : BaseView<SnekControlViewMediator>
{
    [SerializeField] private SnekRunner snekRunner;

    [SerializeField] private int initialBodyLength = 30;
    private int singleBodyLength = 10;
    private int currentLength = 0;
    private int speed = 5;

    private void Update()
    {
        // For testing purposes, increase length with L key
        if (Input.GetKeyDown(KeyCode.L))
        {
            currentLength += singleBodyLength;
            UpdateSnekLength();
        }
    }

    private void Start()
    {
        currentLength = initialBodyLength;
        UpdateSnekLength();
    }

    public void SetupRunnerSkin(SnekkiesAsset skinAsset)
    {
        snekRunner.onAteFood = OnGetScore;
        snekRunner.Setup(skinAsset, () =>
        {
            UpdateSnekLength();
            mediator.OnRunnerSkinSetupComplete();
        });
    }

    public void UpdateSnekLength()
    {
        snekRunner.SetBodyLength(currentLength);
    }

    public void UpdateSnekSpeed(int newSpeed)
    {
        speed = newSpeed;
        snekRunner.SetSpeed(speed);
    }

    public void OnGetScore(ScoreObjectView scoreObjectView)
    {
        currentLength += singleBodyLength;
        UpdateSnekLength();
        Destroy(scoreObjectView.gameObject);
    }

}

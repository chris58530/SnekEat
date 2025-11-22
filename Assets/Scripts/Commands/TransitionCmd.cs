using UnityEngine;
using Zenject;
using Core.MVC;
using DG.Tweening;
using System.Collections;

[CreateAssetMenu(fileName = "TransitionCmd", menuName = "SnekRun/Commands/TransitionCmd")]
public class TransitionCmd : ICommand
{
    public enum TransitionType
    {
        FadeOut, // 變黑
        FadeIn   // 變亮
    }

    public TransitionType type;
    public float duration = 0.5f;

    // 這裡假設 TransitionOverlay 已經被綁定到 Zenject Container 中
    // 或者我們可以透過 FindObjectOfType 尋找 (如果沒有綁定的話)
    private TransitionOverlay transitionOverlay;

    public override void Initialize(MonoBehaviour context, Listener listener, DiContainer container)
    {
        base.Initialize(context, listener, container);

        // 嘗試從 Container 獲取，如果失敗則尋找場景中的物件
        if (container != null && container.HasBinding<TransitionOverlay>())
        {
            transitionOverlay = container.Resolve<TransitionOverlay>();
        }
        else
        {
            transitionOverlay = Object.FindAnyObjectByType<TransitionOverlay>();
        }
    }

    public override void Execute(MonoBehaviour mono)
    {
        if (transitionOverlay == null)
        {
            Debug.LogWarning("TransitionOverlay not found!");
            SetComplete();
            return;
        }

        mono.StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        if (type == TransitionType.FadeOut)
        {
            transitionOverlay.FadeOut(duration);
        }
        else
        {
            transitionOverlay.FadeIn(duration);
        }

        // 等待動畫時間
        yield return new WaitForSeconds(duration);

        // 如果是 FadeOut，我們可能還想額外等待一點時間確保完全變黑
        if (type == TransitionType.FadeOut)
        {
            // 這裡可以加入額外的等待邏輯，如果需要的話
        }

        SetComplete();
    }
}

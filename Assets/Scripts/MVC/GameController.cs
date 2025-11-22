using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Core.MVC;
using UnityEngine.SceneManagement;

public enum GameState
{
    // Deprecated, kept for compatibility if needed, but logic moved to GameDefinitions.cs
    Init,
    Playing,
    Transition,
}

public class GameController : MonoBehaviour
{
    [System.Serializable]
    public class StageData
    {
        public GameStage stageType;
        public StageCollection collection;
        [Tooltip("此階段需要載入的場景名稱")]
        public List<string> scenes = new List<string>();
    }

    [Header("Stage Configuration")]
    [Tooltip("Entry 階段的設定 (不需載入場景)")]
    public StageCollection entryCollection;

    [Tooltip("所有的階段設定")]
    public List<StageData> stages = new List<StageData>();

    [Header("Debug Info")]
    [SerializeField] private GameStage currentStage;
    [SerializeField] private StageLifecycle currentLifecycle;

    public GameStage CurrentStage => currentStage;
    public StageLifecycle CurrentLifecycle => currentLifecycle;

    // 當前正在執行的 StageData
    private StageData currentStageData;

    // 記錄當前已載入的場景，以便之後卸載
    private List<string> loadedScenes = new List<string>();

    [Inject] private Listener listener;
    [Inject] private DiContainer container;

    private IEnumerator Start()
    {
        // 遊戲開始，預設已經是 Entry 狀態
        // 不走 ChangeStage 的轉場流程 (避免開場黑屏或不必要的轉場)，直接執行 Entry 的邏輯
        Debug.Log("[GameController] Game Start - Initializing Entry Stage");

        currentStage = GameStage.Entry;

        if (entryCollection != null)
        {
            // 建立 Entry 的 StageData
            currentStageData = new StageData
            {
                stageType = GameStage.Entry,
                collection = entryCollection,
                scenes = new List<string>()
            };

            // 1. 執行 Init 指令
            currentLifecycle = StageLifecycle.Init;
            yield return StartCoroutine(ExecuteCommands(currentStageData.collection.initCommands));
            ChangeStage(GameStage.Menu);

            //     // 2. 執行 Processing 指令 (這裡面應該包含切換到 Menu 的邏輯)
            //     currentLifecycle = StageLifecycle.Processing;
            //     yield return StartCoroutine(ExecuteCommands(currentStageData.collection.processingCommands));
        }
        else
        {
            Debug.LogWarning("[GameController] Entry Collection is not assigned! Directly changing to Menu.");
            // 如果沒有 Entry 設定，直接嘗試進入 Menu
            ChangeStage(GameStage.Menu);
        }
    }

    /// <summary>
    /// 外部呼叫此方法來切換階段 (Stage)
    /// </summary>
    public void ChangeStage(GameStage newStage)
    {
        StartCoroutine(ProcessStageChange(newStage));
    }

    private IEnumerator ProcessStageChange(GameStage newStage)
    {
        Debug.Log($"[GameController] Start changing stage to: {newStage}");

        // 0. 找到目標 StageData
        StageData targetStageData = null;

        if (newStage == GameStage.Entry)
        {
            if (entryCollection != null)
            {
                // 動態建立一個 StageData 給 Entry 使用，不包含任何場景
                targetStageData = new StageData
                {
                    stageType = GameStage.Entry,
                    collection = entryCollection,
                    scenes = new List<string>()
                };
            }
        }
        else
        {
            targetStageData = stages.Find(s => s.stageType == newStage);
        }

        if (targetStageData == null)
        {
            Debug.LogError($"找不到階段 {newStage} 對應的 StageData 設定！(如果是 Entry 請檢查 entryCollection)");
            yield break;
        }

        // 1. 發送轉場請求事件 (通知 TransitionMediator 變黑)
        listener.BroadCast(TransitionEvent.REQUEST_TRANSITION);

        // 2. 如果當前有 Stage，先執行其 Transition 指令
        // 注意：如果 TransitionMediator 已經負責變黑，這裡的指令可能只需要負責 "等待" (例如 WaitCmd)
        // 或是保留原本的 TransitionCmd 但將其邏輯改為單純等待時間
        if (currentStageData != null && currentStageData.collection != null)
        {
            currentLifecycle = StageLifecycle.Transition;
            yield return StartCoroutine(ExecuteCommands(currentStageData.collection.transitionCommands));
        }

        // 2. 場景管理：卸載舊場景 -> 載入新場景
        // 注意：TransitionOverlay 所在的場景 (MenuScene) 不應該被卸載，所以這裡只卸載由 GameController 載入的場景
        if (loadedScenes.Count > 0)
        {
            foreach (var sceneName in loadedScenes)
            {
                // 檢查場景是否已載入，避免重複卸載或錯誤
                Scene scene = SceneManager.GetSceneByName(sceneName);
                if (scene.isLoaded)
                {
                    yield return SceneManager.UnloadSceneAsync(sceneName);
                }
            }
            loadedScenes.Clear();
        }

        // 載入新場景 (Additive 模式，不破壞 MenuScene)
        if (targetStageData.scenes != null && targetStageData.scenes.Count > 0)
        {
            foreach (var sceneName in targetStageData.scenes)
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                loadedScenes.Add(sceneName);
            }

            // 設定最後一個載入的場景為 Active，確保新生成的物件在正確場景
            Scene lastLoadedScene = SceneManager.GetSceneByName(targetStageData.scenes[targetStageData.scenes.Count - 1]);
            if (lastLoadedScene.IsValid())
            {
                SceneManager.SetActiveScene(lastLoadedScene);
            }
        }

        // 3. 更新狀態與當前 Data
        currentStage = newStage;
        currentStageData = targetStageData;

        // 4. 執行新 Stage 的 Init 指令 (需等待完成)
        currentLifecycle = StageLifecycle.Init;
        yield return StartCoroutine(ExecuteCommands(currentStageData.collection.initCommands));

        // 5. Init 完成，發送轉場完成事件 (通知 TransitionMediator 變亮)
        listener.BroadCast(TransitionEvent.TRANSITION_COMPLETE);

        // 6. Init 完成後，自動執行 Processing 指令
        currentLifecycle = StageLifecycle.Processing;
        yield return StartCoroutine(ExecuteCommands(currentStageData.collection.processingCommands));
    }
    private IEnumerator ExecuteCommands(List<ICommand> commands)
    {
        if (commands == null || commands.Count == 0) yield break;

        // 1. 初始化並執行所有 Command
        foreach (var cmd in commands)
        {
            if (cmd == null) continue;

            // 關鍵修改：手動注入依賴
            // 這行程式碼會讓 Command 裡面的 [Inject] 生效
            // 讓 Command 可以取得 Listener 或 Proxy
            container.Inject(cmd);

            // 注入依賴 (保留原本的 Initialize 以防萬一，但主要依賴 Zenject Inject)
            cmd.Initialize(this, listener, container);

            // 執行
            cmd.Execute(this);
        }

        // 2. 等待非 Lazy 的 Command 完成
        foreach (var cmd in commands)
        {
            if (cmd == null) continue;

            if (!cmd.isLazy)
            {
                // 等待 IsJobComplete 變為 true
                yield return new WaitUntil(() => cmd.IsJobComplete);
            }
        }
    }
}

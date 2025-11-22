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

    private void Start()
    {
        // 遊戲開始，進入 Entry 狀態
        ChangeStage(GameStage.Entry);
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
        // 0. 找到目標 StageData
        StageData targetStageData = stages.Find(s => s.stageType == newStage);
        if (targetStageData == null)
        {
            Debug.LogError($"找不到階段 {newStage} 對應的 StageData 設定！");
            yield break;
        }

        // 1. 如果當前有 Stage，先執行其 Transition 指令 (例如 FadeOut)
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

        // 5. Init 完成後，自動執行 Processing 指令
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

            // 注入依賴
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

using System.Collections.Generic;
using UnityEngine;
using Core.MVC;

[CreateAssetMenu(fileName = "NewStageCollection", menuName = "SnekRun/Stage Collection")]
public class StageCollection : ScriptableObject
{
    [Tooltip("此階段的狀態標識")]
    public GameStage stage;

    [Header("Commands")]
    [Tooltip("進入此階段時執行的指令 (需等待完成)")]
    public List<ICommand> initCommands = new List<ICommand>();

    [Tooltip("此階段進行中的指令 (通常 isLazy=true)")]
    public List<ICommand> processingCommands = new List<ICommand>();

    [Tooltip("離開此階段時執行的指令 (需等待完成)")]
    public List<ICommand> transitionCommands = new List<ICommand>();

    public string enterStageEvent;
}

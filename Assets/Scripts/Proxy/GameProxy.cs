using UnityEngine;
using Core.MVC;

public class GameProxy : IProxy
{
    public GameStage currentStage;
    public int selectNFTid;
    public void SetCurrentStage(GameStage stage)
    {
        currentStage = stage;
        listener.BroadCast(GameEvent.ON_STAGE_CHANGED, stage);
    }
    public void SetSelectNFT(int id)
    {
        selectNFTid = id;
    }

}
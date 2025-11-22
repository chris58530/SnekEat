using Core.MVC;
using UnityEngine;

public class MazeGenerateView : BaseView<MazeGenerateViewMediator>
{
    [SerializeField] private FixedSizeMapGenerator mazeGenerator;

    public void GenerateMaze(int width, int height, int difficulty = 1)
    {
        Debug.Log("GenerateMaze called");
        if (mazeGenerator != null)
        {
            mazeGenerator.GenerateMap(width, height, difficulty);
        }
        else
        {
            Debug.LogError("MazeGenerator 尚未指派！請在 Inspector 中拖曳引用。");
        }
    }
}

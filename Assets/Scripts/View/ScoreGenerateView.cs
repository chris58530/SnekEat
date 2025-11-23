using UnityEngine;
using Core.MVC;

public class ScoreGenerateView : BaseView<ScoreGenerateViewMediator>
{
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private int scoreGenerateCount = 10;


    public void GenerateScores()
    {
        for (int i = 0; i < scoreGenerateCount; i++)
        {
            float randomX = Random.Range(GameMathService.generateAreaMin.x, GameMathService.generateAreaMax.x);
            float randomY = Random.Range(GameMathService.generateAreaMin.y, GameMathService.generateAreaMax.y);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0);
            GameObject scoreObject = Instantiate(scorePrefab, spawnPosition, Quaternion.identity);
            scoreObject.transform.SetParent(root.transform);
        }
    }
}

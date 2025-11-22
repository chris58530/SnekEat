using UnityEngine;
using UnityEngine.Tilemaps;

public class FixedSizeMapGenerator : MonoBehaviour
{
    [Header("元件引用")]
    public Tilemap obstacleTilemap;
    public TileBase wallTile;

    [Header("當前設定 (由 View 傳入)")]
    public int width;
    public int height;

    // 內部參數
    private float seedX, seedY;

    /// <summary>
    /// 生成迷宮的主入口
    /// </summary>
    /// <param name="w">寬度</param>
    /// <param name="h">高度</param>
    /// <param name="difficultyLevel">難度 (1~10)</param>
    public void GenerateMap(int w, int h, int difficultyLevel)
    {
        this.width = w;
        this.height = h;

        // 1. 每次生成都重新隨機化種子 (這樣重玩同一關地圖也會變)
        // 如果希望同一關卡地圖固定，可以把這行移出去
        seedX = Random.Range(0f, 10000f);
        seedY = Random.Range(0f, 10000f);

        // 2. 清空舊地圖
        obstacleTilemap.ClearAllTiles();

        // 3. 計算難度參數
        // 難度越高(10) -> Threshold 低 (0.45, 牆多)
        // 難度越高(10) -> Scale 大 (0.35, 牆碎)
        float currentThreshold = Mathf.Lerp(0.7f, 0.45f, (difficultyLevel - 1) / 9f);
        float currentScale = Mathf.Lerp(0.15f, 0.35f, (difficultyLevel - 1) / 9f);

        // 4. 執行生成迴圈
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // A. 邊界牆 (最外圈)
                if (x == -halfWidth || x == halfWidth || y == -halfHeight || y == halfHeight)
                {
                    obstacleTilemap.SetTile(pos, wallTile);
                    continue;
                }

                // B. 安全區 (出生點周圍不生牆)
                if (Mathf.Abs(x) <= 2 && Mathf.Abs(y) <= 2)
                {
                    continue;
                }

                // C. 噪點生成
                float xCoord = (x + seedX) * currentScale;
                float yCoord = (y + seedY) * currentScale;
                float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);

                if (noiseValue > currentThreshold)
                {
                    obstacleTilemap.SetTile(pos, wallTile);
                }
            }
        }

        // (選用) 為了物理碰撞，有些 Tilemap 需要強制刷新 Collider
        // obstacleTilemap.GetComponent<TilemapCollider2D>()?.ProcessTilemapChanges();
    }
}
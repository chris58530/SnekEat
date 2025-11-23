using System.Collections.Generic;
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
        RemoveDisconnectedAreas();
        // (選用) 為了物理碰撞，有些 Tilemap 需要強制刷新 Collider
        // obstacleTilemap.GetComponent<TilemapCollider2D>()?.ProcessTilemapChanges();
    }
    private void RemoveDisconnectedAreas()
    {
        // 1. 準備一個 HashSet 來記錄哪些格子是「連通」的
        HashSet<Vector3Int> reachableTiles = new HashSet<Vector3Int>();

        // 2. 準備 BFS (廣度優先搜索) 用的 Queue
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        // 從中心點 (0,0) 開始搜尋 (假設這是出生點)
        Vector3Int startPos = new Vector3Int(0, 0, 0);

        // 如果出生點本身就有牆 (理論上不會，因為你有設安全區)，要先清掉
        if (obstacleTilemap.HasTile(startPos))
        {
            obstacleTilemap.SetTile(startPos, null);
        }

        queue.Enqueue(startPos);
        reachableTiles.Add(startPos);

        int halfWidth = width / 2;
        int halfHeight = height / 2;

        // 定義上下左右四個方向
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        // --- 3. 開始擴散 (Flood Fill) ---
        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector3Int neighbor = current + dir;

                // 邊界檢查：不要超出地圖範圍
                if (neighbor.x < -halfWidth || neighbor.x > halfWidth ||
                    neighbor.y < -halfHeight || neighbor.y > halfHeight)
                    continue;

                // 如果這個鄰居：
                // 1. 不是牆壁 (是路)
                // 2. 還沒被訪問過
                if (!obstacleTilemap.HasTile(neighbor) && !reachableTiles.Contains(neighbor))
                {
                    reachableTiles.Add(neighbor); // 標記為可到達
                    queue.Enqueue(neighbor);      // 加入佇列繼續搜尋
                }
            }
        }

        // --- 4. 填補封閉區域 ---
        // 遍歷整張地圖，檢查每一個格子
        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // 跳過邊界牆 (它們本來就是牆)
                if (x == -halfWidth || x == halfWidth || y == -halfHeight || y == halfHeight)
                    continue;

                // 如果這個位置：
                // 1. 目前沒有牆 (看起來是空地)
                // 2. 但是 Flood Fill 沒有走到這裡 (代表是不連通的封閉區域)
                if (!obstacleTilemap.HasTile(pos) && !reachableTiles.Contains(pos))
                {
                    // 把它變成牆壁！
                    obstacleTilemap.SetTile(pos, wallTile);
                }
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Transform[, ] grid; // 盤面を記憶

    [SerializeField] // ボード基盤用の枠
    private Transform emptyBox;

    [SerializeField] // ボードの高さ，幅，高さ調整
    private int height = 30, width = 10, header = 8;

    private List<int> filledRows = new List<int>();

    private Score score;

    private void Awake()
    {
        grid = new Transform[width, height];
        score = GameObject.FindObjectOfType<Score>(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
    }

    // ボードの生成
    void CreateBoard()
    {
        if (emptyBox)
        {
            for (int y = 0; y < height - header; y++) 
            {
                for (int x = 0; x < width; x++)
                {
                    Transform clone = Instantiate(emptyBox, new Vector3(x, y, 0), Quaternion.identity);

                    clone.transform.parent = transform;
                }
            }
        }
    }

    // ブロックが範囲内に存在するか判定
    public bool CheckPosition(Block block)
    {
        foreach (Transform item in block.transform)
        {
            Vector2 pos = Rounding.Round(item.position);

            // 左右と下が範囲外のとき
            if (!IsOnBoard((int)pos.x, (int)pos.y))
            {
                return false;
            }

            // ブロックが既に存在しているとき
            if (IsOccupied((int)pos.x, (int)pos.y, block))
            {
                return false;
            }
        }

        return true;
    }

    bool IsOnBoard (int x, int y)
    {
        // 左右と下が範囲内
        return (x >= 0 && x < width && y >= 0);
    }

    bool IsOccupied (int x, int y, Block block)
    {
        // grid[x, y]に親が違うブロックが存在
        return (grid[x, y] != null && grid[x, y].parent != block.transform);
    }

    // 停止したブロックの位置を記録
    public void SaveBlockPosition(Block block)
    {
        foreach (Transform item in block.transform)
        {
            Vector2 pos = Rounding.Round(item.position);

            grid[(int)pos.x, (int)pos.y] = item;
        }
        SoundManager.instance.PlaySE("atBottom");
    }
    
    public List<int> CheckFilledRows()
    {
        // 消去待ち配列
        filledRows = new List<int>();

        int lineNum = 0;

        for (int y = 0; y < height; y++)
        {
            if (IsFilledRow(y))
            {
                ClearRow(y);
                filledRows.Add(y);
                // 複数(lineNum)ライン消すと高得点
                lineNum += 1;
                score.AddScoreForClearLine(lineNum);
            }
        }
        return filledRows;
    }

    // 埋まった行を全て削除
    public void ClearFilledRows()
    {
        // 消去待ち配列を降順にソート
        filledRows.Sort((a, b) => b - a);
        // 消去待ち配列の行を削除
        foreach (int y in filledRows)
        {
            DropRows(y);
        }
        filledRows.Clear();
    }

    // 行が埋まっているか確認
    bool IsFilledRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        SoundManager.instance.PlaySE("filledRows");

        return true;
    }

    // 行を削除
    void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                if (grid[x, y].parent.transform.childCount > 1)
                {
                    Destroy(grid[x, y].gameObject);
                } 
                else
                {
                    Destroy(grid[x, y].parent.gameObject);
                }
            }
            grid[x, y] = null;
        }
    }

    // 空いた行分下げる
    void DropRows(int emptyRow)
    {
        for (int y = emptyRow + 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    // ブロックが上限を超えたか判定
    public bool IsOverflowed(Block block)
    {
        foreach (Transform item in block.transform)
        {
            if (item.transform.position.y >= height - header)
            {
                return true;
            }
        }

        return false;
    }
}

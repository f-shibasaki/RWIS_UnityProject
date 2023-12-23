using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Spawner spawner;
    Block activeBlock; // 落ちてくるブロック
    Queue<Block> upcomingBlocks = new Queue<Block>(); // 今後のブロック
    Block tmpBlock; // ホールドしたブロック

    [SerializeField]
    private float dropInterval = 0.25f; // ブロックが停止している時間
    float nextDropTimer; // 次にブロックが1マス落ちる時間

    Board board;

    float nextKeyDropTimer, nextKeyShiftTimer, nextKeyRotateTimer; // 入力受付タイマー
    [SerializeField] // 入力インターバル
    private float nextKeyDropInterval, nextKeyShiftInterval, nextKeyRotateInterval;

    [SerializeField]
    private GameObject gameOverPanel;
    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindObjectOfType<Spawner>();
        board = GameObject.FindObjectOfType<Board>();

        spawner.transform.position = Rounding.Round(spawner.transform.position);

        nextKeyDropTimer = Time.time + nextKeyDropInterval;
        nextKeyShiftTimer = Time.time + nextKeyShiftInterval;
        nextKeyRotateTimer = Time.time + nextKeyRotateInterval;

        for (int i = 0; i < 5; i++)
        {
            Block b = spawner.SpawnBlock();
            if (i > 0)
            {
                b.transform.position += new Vector3(7, -5 * i, 0);
            }
            upcomingBlocks.Enqueue(b);
        }
        if (!activeBlock)
        {
            activeBlock = upcomingBlocks.Dequeue();
        }

        if (gameOverPanel.activeInHierarchy)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            return;
        }

        PlayerInput();
    }

    // 入力を検知してブロックを移動
    void PlayerInput ()
    {
        if (Input.GetKey(KeyCode.D) && (Time.time > nextKeyShiftTimer) || Input.GetKeyDown(KeyCode.D))
        {
            activeBlock.MoveRight();
            nextKeyShiftTimer = Time.time + nextKeyShiftInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.MoveLeft();
            }
        }
        else if (Input.GetKey(KeyCode.A) && (Time.time > nextKeyShiftTimer) || Input.GetKeyDown(KeyCode.A))
        {
            activeBlock.MoveLeft();
            nextKeyShiftTimer = Time.time + nextKeyShiftInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.MoveRight();
            }
        }
        else if (Input.GetKey(KeyCode.E) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.E))
        {
            activeBlock.RotateRight();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateLeft();
            }
        }
        else if (Input.GetKey(KeyCode.Q) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.Q))
        {
            activeBlock.RotateLeft();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateRight();
            }
        }
        else if (Input.GetKey(KeyCode.W) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.W))
        {
            activeBlock.RotateUp();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateUp();
            }
        }
        else if (Input.GetKey(KeyCode.S) && (Time.time > nextKeyDropTimer) || (Time.time > nextDropTimer))
        {
            activeBlock.MoveDown();
            nextKeyDropTimer = Time.time + nextKeyDropInterval;
            nextDropTimer = Time.time + dropInterval;

            if (!board.CheckPosition(activeBlock))
            {
                if (board.IsOverflowed(activeBlock))
                {
                    GameOver();
                }
                else
                {
                    BlockAtBottom();
                }
            }
        }
    }

    void BlockAtBottom()
    {
        activeBlock.MoveUp();
        board.SaveBlockPosition(activeBlock);

        activeBlock = upcomingBlocks.Dequeue();
        activeBlock.transform.position -= new Vector3(7, -5, 0);

        foreach (var block in upcomingBlocks)
        {
            block.transform.position -= new Vector3(0, -5, 0);
        }

        Block b = spawner.SpawnBlock();
        b.transform.position += new Vector3(7, -5 * 4, 0);
        upcomingBlocks.Enqueue(b);

        nextKeyDropTimer = Time.time;
        nextKeyShiftTimer = Time.time;
        nextKeyRotateTimer = Time.time;

        board.ClearFilledRows();
    }

    // ゲームオーバーメニューの表示
    void GameOver()
    {
        activeBlock.MoveUp();

        if (!gameOverPanel.activeInHierarchy)
        {
            gameOverPanel.SetActive(true);
        }

        gameOver = true;
    }

    // ゲームシーンの再読み込み
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}

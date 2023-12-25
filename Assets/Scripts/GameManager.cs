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

    float nextKeyDropTimer, nextKeyShiftTimer, nextKeyRotateTimer, nextShakeTimer; // 入力受付タイマー
    // 入力インターバル
    [SerializeField]
    private float nextKeyDropInterval = 0.02f;
    [SerializeField]
    private float nextKeyShiftInterval = 0.25f;
    [SerializeField]
    private float nextKeyRotateInterval = 0.50f;
    [SerializeField]
    private float nextShakeInterval = 0.20f;

    private Quaternion inputGyro; // 入力姿勢
    private Quaternion initPose; // 初期姿勢
    private Quaternion newPose; // 現在の姿勢
    Vector3 angles; // 相対角度

    [SerializeField]
    float shakeThreshold = 2f;
    Vector3 acceleration, prevAcceleration;

    [SerializeField]
    private GameObject gameOverPanel;
    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
        inputGyro = Input.gyro.attitude;
        initPose = new Quaternion(-inputGyro.x, -inputGyro.y, inputGyro.z, inputGyro.w);

        acceleration.Set(-Input.acceleration.x, 0, 0);

        spawner = GameObject.FindObjectOfType<Spawner>();
        board = GameObject.FindObjectOfType<Board>();

        spawner.transform.position = Rounding.Round(spawner.transform.position);

        nextKeyDropTimer = Time.time + nextKeyDropInterval;
        nextKeyShiftTimer = Time.time + nextKeyShiftInterval;
        nextKeyRotateTimer = Time.time + nextKeyRotateInterval;
        nextShakeTimer = Time.time + nextShakeInterval;

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

        inputGyro = Input.gyro.attitude;
        newPose = new Quaternion(-inputGyro.x, -inputGyro.y, inputGyro.z, inputGyro.w);
        angles = (Quaternion.Inverse(initPose) * newPose).eulerAngles;

        prevAcceleration = acceleration;
        acceleration.Set(-Input.acceleration.x, 0, 0);

        PlayerInput();
    }

    // 入力を検知してブロックを移動
    void PlayerInput ()
    {
        // 右移動
        if (Input.GetKey(KeyCode.D) && (Time.time > nextKeyShiftTimer) || Input.GetKeyDown(KeyCode.D)
            || (angles.y > 270) && (angles.y < 330) && (Time.time > nextKeyShiftTimer))
        {
            activeBlock.MoveRight();
            nextKeyShiftTimer = Time.time + nextKeyShiftInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.MoveLeft();
            }
        }
        // 左移動
        else if (Input.GetKey(KeyCode.A) && (Time.time > nextKeyShiftTimer) || Input.GetKeyDown(KeyCode.A)
            || (angles.y > 30) && (angles.y < 90) && (Time.time > nextKeyShiftTimer))
        {
            activeBlock.MoveLeft();
            nextKeyShiftTimer = Time.time + nextKeyShiftInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.MoveRight();
            }
        }
        // 時計回り
        else if (Input.GetKey(KeyCode.E) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.E)
            // || (angles.z > 270) && (angles.z < 330) && (Time.time > nextKeyRotateTimer)
            || (Vector3.Dot(acceleration, prevAcceleration) < 0) && (prevAcceleration.x > 0) && (Input.acceleration.magnitude > shakeThreshold) && (Time.unscaledTime > nextShakeTimer))
        {
            activeBlock.RotateRight();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;
            nextShakeTimer = Time.time + nextShakeInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateLeft();
            }
        }
        // 反時計回り
        else if (Input.GetKey(KeyCode.Q) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.Q)
            // || (angles.z > 30) && (angles.z < 90) && (Time.time > nextKeyRotateTimer)
            || (Vector3.Dot(acceleration, prevAcceleration) < 0) && (prevAcceleration.x < 0) && (Input.acceleration.magnitude > shakeThreshold) && (Time.unscaledTime > nextShakeTimer))
        {
            activeBlock.RotateLeft();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;
            nextShakeTimer = Time.time + nextShakeInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateRight();
            }
        }
        // 裏返す
        else if (Input.GetKey(KeyCode.W) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.W))
        {
            activeBlock.RotateUp();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateUp();
            }
        }
        // 下移動
        else if (Input.GetKey(KeyCode.S) && (Time.time > nextKeyDropTimer) || (Time.time > nextDropTimer)
            // || (angles.x > 270) && (angles.x < 300) && (Time.time > nextKeyDropTimer)
            )
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

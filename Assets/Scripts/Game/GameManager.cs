using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EffectsManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    Spawner spawner;
    
    [SerializeField]
    Board board;
    
    EffectsManager effectsManager;
    
    Block activeBlock; // 落ちてくるブロック
    
    Queue<Block> upcomingBlocks = new Queue<Block>(); // 今後のブロック
    Block holdBlock; // ホールドしたブロック

    [SerializeField]
    private float dropInterval = 0.5f; // ブロックが停止している時間
    float nextDropTimer; // 次にブロックが1マス落ちる時間

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

    bool gameOver;
    bool pause = true;
    bool isHorizontalMoveEnabled = true;
    bool isRotationEnabled = true;
    bool isDropEnabled = true;

    public Action OnGameOverDelegate { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        Input.gyro.enabled = true;
        inputGyro = Input.gyro.attitude;
        initPose = new Quaternion(-inputGyro.x, -inputGyro.y, inputGyro.z, inputGyro.w);

        // acceleration.Set(-Input.acceleration.x, 0, 0); // 横方向のみ（回転を左右で分ける用）
        acceleration.Set(-Input.acceleration.x, -Input.acceleration.y, Input.acceleration.z);

        spawner = GameObject.FindObjectOfType<Spawner>();
        board = GameObject.FindObjectOfType<Board>();
        effectsManager = GetComponent<EffectsManager>();

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
        
        // 画面をスリープしない設定
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver || pause)
        {
            return;
        }

        inputGyro = Input.gyro.attitude;
        newPose = new Quaternion(-inputGyro.x, -inputGyro.y, inputGyro.z, inputGyro.w);
        angles = (Quaternion.Inverse(initPose) * newPose).eulerAngles;

        prevAcceleration = acceleration;
        acceleration.Set(-Input.acceleration.x, -Input.acceleration.y, Input.acceleration.z);

        PlayerInput();
    }

    public bool MoveInput()
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
                return true;
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
                return true;
            }
        }
        return false;
    }

    public bool RotationInput()
    {
        // 時計回り
        if (Input.GetKey(KeyCode.E) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.E)
            // || (angles.z > 270) && (angles.z < 330) && (Time.time > nextKeyRotateTimer)
            // || (Vector3.Dot(acceleration, prevAcceleration) < 0) && (prevAcceleration.x > 0) && (Input.acceleration.magnitude > shakeThreshold) && (Time.unscaledTime > nextShakeTimer)
           )
        {
            activeBlock.RotateRight();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;
            nextShakeTimer = Time.time + nextShakeInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateLeft();
                return true;
            }
        }
        // 反時計回り
        else if (Input.GetKey(KeyCode.Q) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.Q)
            // || (angles.z > 30) && (angles.z < 90) && (Time.time > nextKeyRotateTimer)
            // || (Vector3.Dot(acceleration, prevAcceleration) < 0) && (prevAcceleration.x < 0) && (Input.acceleration.magnitude > shakeThreshold) && (Time.unscaledTime > nextShakeTimer)
            || (Vector3.Dot(acceleration, prevAcceleration) < 0) && (Input.acceleration.magnitude > shakeThreshold) && (Time.unscaledTime > nextShakeTimer))
        {
            activeBlock.RotateLeft();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;
            nextShakeTimer = Time.time + nextShakeInterval;
            
            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateRight();
                return true;
            }
        }

        return false;
    }

    public bool Drop()
    {
        if (Input.GetKey(KeyCode.S) && (Time.time > nextKeyDropTimer) || (Time.time > nextDropTimer)
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

            return true;
        }

        return false;
    }

    // 入力を検知してブロックを移動
    void PlayerInput ()
    {
        if (isHorizontalMoveEnabled && MoveInput())
        {
            return;
        }
        if (isRotationEnabled && RotationInput())
        {
            return;
        }
        // 裏返す
        if (Input.GetKey(KeyCode.W) && (Time.time > nextKeyRotateTimer) || Input.GetKeyDown(KeyCode.W)) 
        {
            activeBlock.RotateUp();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;

            if (!board.CheckPosition(activeBlock))
            {
                activeBlock.RotateUp();
            }
        }
        // 下移動
        else if (isDropEnabled && Drop())
        {
            return;
        }
        // ホールド
        if (Input.GetKey(KeyCode.H) && (Time.time > nextKeyShiftTimer) || Input.GetKeyDown(KeyCode.H))
        {
            HoldBlock();

            nextKeyShiftTimer = Time.time + nextKeyShiftInterval;
        }
    }

    async void BlockAtBottom()
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

        List<int> filledRows = board.CheckFilledRows();
        await effectsManager.PlayLineClearEffect(filledRows);
        board.ClearFilledRows();
    }

    void HoldBlock()
    {
        if (holdBlock)
        {
            Block tmpBlock = holdBlock;
            Vector3 currentPosition = activeBlock.transform.position;

            holdBlock = activeBlock;
            holdBlock.transform.position = new Vector3(-2, 20, -1);
            holdBlock.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            activeBlock = tmpBlock;
            activeBlock.transform.position = currentPosition;
        }
        else
        {
            Vector3 currentPosition = activeBlock.transform.position;
            holdBlock = activeBlock;
            holdBlock.transform.position = new Vector3(-2, 20, -1);
            holdBlock.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            activeBlock = upcomingBlocks.Dequeue();
            activeBlock.transform.position = currentPosition;
        }
    }

    // ゲームオーバー処理の実行
    void GameOver()
    {
        // 画面スリープを有効化
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        activeBlock.MoveUp();

        gameOver = true;
        
        OnGameOverDelegate.Invoke();
    }

    public void Pause()
    {
        pause = true;
    }
    
    public void Resume()
    {
        pause = false;
        
        Invoke("ResetGyro", 1.0f);
    }

    public void ResetGyro()
    {
        initPose = new Quaternion(-inputGyro.x, -inputGyro.y, inputGyro.z, inputGyro.w);
    }
    

    public void EnableHorizontalMove()
    {
        isHorizontalMoveEnabled = true;
    }
    
    public void EnableRotation()
    {
        isRotationEnabled = true;
    }
    public void PrepareForTutorial()
    {
        activeBlock.transform.position -= new Vector3(0, 10, 0);

        isHorizontalMoveEnabled = false;
        isRotationEnabled = false;
        isDropEnabled = false;
        
        pause = false;
    }
}

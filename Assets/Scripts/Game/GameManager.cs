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
    Block shadowBlock;
    [SerializeField]
    Material shadowMaterial;
    
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
    bool isHoldEnabled = true;
    bool isSwipeEnabled = true;
    bool isDropEnabled = true;


    // swipe
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private float SWIPE_THRESHOLD = 50.0f; // しきい値以上スワイプするとスワイプとして検知
    private bool InputSwipe = false;       // swipe入力検知
    private bool InputTouch = false;       // touch入力検知

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
            shadowBlock = Instantiate(activeBlock, this.transform);
            foreach (Transform item in shadowBlock.transform)
            {
                Renderer render = item.gameObject.GetComponent<Renderer>();
                render.material = shadowMaterial;
            }
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
        SimulateShadowBlock();
    }

    public bool MoveInput()
    {
        // 右移動
        if (Input.GetKey(KeyCode.D) && (Time.time > nextKeyShiftTimer) || Input.GetKeyDown(KeyCode.D)
                                                                       || (angles.y > 270) && (angles.y < 330) && (Time.time > nextKeyShiftTimer))
        {
            activeBlock.MoveRight();
            nextKeyShiftTimer = Time.time + nextKeyShiftInterval;

            if (board.CheckPosition(activeBlock) == BlockValidation.RightOver
                || board.CheckPosition(activeBlock) == BlockValidation.Occupied)
            {
                activeBlock.MoveLeft();
                return true;
            }
            VibrationMng.Vibrate(30);
        }
        // 左移動
        else if (Input.GetKey(KeyCode.A) && (Time.time > nextKeyShiftTimer) || Input.GetKeyDown(KeyCode.A)
                                                                            || (angles.y > 30) && (angles.y < 90) && (Time.time > nextKeyShiftTimer))
        {
            activeBlock.MoveLeft();
            nextKeyShiftTimer = Time.time + nextKeyShiftInterval;

            if (board.CheckPosition(activeBlock) == BlockValidation.LeftOver
                || board.CheckPosition(activeBlock) == BlockValidation.Occupied)
            {
                activeBlock.MoveRight();
                return true;
            }
            VibrationMng.Vibrate(30);
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

            if (board.CheckPosition(activeBlock) != BlockValidation.Success)
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
            SoundManager.instance.PlaySE("rotationSE");
            VibrationMng.Vibrate(30);

            Transform tempActiveBlockTransform = activeBlock.transform;

            activeBlock.RotateLeft();
            nextKeyRotateTimer = Time.time + nextKeyRotateInterval;
            nextShakeTimer = Time.time + nextShakeInterval;

            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
            {
                // SRS
                if (activeBlock.name == "Block_T(Clone)")
                {
                    if (activeBlock.transform.rotation.eulerAngles.z == 90.00f)
                    {
                        // #1
                        activeBlock.MoveRight();
                        if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                        {
                            // #2
                            activeBlock.MoveUp();
                            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                            {
                                // #3
                                // #4
                                activeBlock.MoveDown();
                                activeBlock.MoveDown();
                                activeBlock.MoveDown();
                                if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                {
                                    activeBlock.MoveUp();
                                    activeBlock.MoveUp();
                                    activeBlock.MoveLeft();
                                    activeBlock.RotateRight();
                                }
                            }
                        }
                    } else if (activeBlock.transform.rotation.eulerAngles.z == 180.00f)
                    {
                        // #1
                        activeBlock.MoveLeft();
                        if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                        {
                            // #2
                            activeBlock.MoveDown();
                            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                            {
                                // #3
                                activeBlock.MoveUp();
                                activeBlock.MoveUp();
                                activeBlock.MoveUp();
                                activeBlock.MoveRight();
                                if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                {
                                    // #4
                                    activeBlock.MoveLeft();
                                    if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                    {
                                        activeBlock.MoveDown();
                                        activeBlock.MoveDown();
                                        activeBlock.MoveRight();
                                        activeBlock.RotateRight();
                                    }
                                }
                            }
                        }
                    } else if (activeBlock.transform.rotation.eulerAngles.z == 270.00f)
                    {
                        // #1
                        activeBlock.MoveLeft();
                        if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                        {
                            // #2
                            // #3
                            activeBlock.MoveRight();
                            activeBlock.MoveDown();
                            activeBlock.MoveDown();
                            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                            {
                                // #4
                                activeBlock.MoveLeft();
                                if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                {
                                    activeBlock.MoveUp();
                                    activeBlock.MoveUp();
                                    activeBlock.MoveRight();
                                    activeBlock.RotateRight();
                                }
                            }
                        }
                    } else if (activeBlock.transform.rotation.eulerAngles.z == 0.00f)
                    {
                        // #1
                        activeBlock.MoveRight();
                        if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                        {
                            // #2
                            activeBlock.MoveDown();
                            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                            {
                                // #3
                                activeBlock.MoveUp();
                                activeBlock.MoveUp();
                                activeBlock.MoveUp();
                                activeBlock.MoveLeft();
                                if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                {
                                    // #4
                                    activeBlock.MoveRight();
                                    if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                    {
                                        activeBlock.MoveDown();
                                        activeBlock.MoveDown();
                                        activeBlock.MoveLeft();
                                        activeBlock.RotateRight();
                                    }
                                }
                            }
                        }
                    }
                } else if (activeBlock.name == "Block_I(Clone)")
                {
                    if (activeBlock.transform.rotation.eulerAngles.z == 90.00f)
                    {
                        // #1
                        activeBlock.MoveLeft();
                        if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                        {
                            // #2
                            activeBlock.MoveRight();
                            activeBlock.MoveRight();
                            activeBlock.MoveRight();
                            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                            {
                                // #3
                                activeBlock.MoveLeft();
                                activeBlock.MoveLeft();
                                activeBlock.MoveLeft();
                                activeBlock.MoveUp();
                                activeBlock.MoveUp();
                                if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                {
                                    // #4
                                    activeBlock.MoveRight();
                                    activeBlock.MoveRight();
                                    activeBlock.MoveRight();
                                    activeBlock.MoveDown();
                                    activeBlock.MoveDown();
                                    activeBlock.MoveDown();
                                    if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                    {
                                        activeBlock.MoveUp();
                                        activeBlock.MoveLeft();
                                        activeBlock.MoveLeft();
                                        activeBlock.RotateRight();
                                    }
                                }
                            }
                        }
                    } else if (activeBlock.transform.rotation.eulerAngles.z == 180.00f)
                    {
                        // #1
                        activeBlock.MoveRight();
                        if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                        {
                            // #2
                            activeBlock.MoveLeft();
                            activeBlock.MoveLeft();
                            activeBlock.MoveLeft();
                            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                            {
                                // #3
                                activeBlock.MoveDown();
                                if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                {
                                    // #4
                                    activeBlock.MoveUp();
                                    activeBlock.MoveUp();
                                    activeBlock.MoveUp();
                                    activeBlock.MoveRight();
                                    activeBlock.MoveRight();
                                    activeBlock.MoveRight();
                                }
                            }
                        }
                    } else if (activeBlock.transform.rotation.eulerAngles.z == 270.00f)
                    {
                        // #1
                        activeBlock.MoveRight();
                        if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                        {
                            // #2
                            activeBlock.MoveLeft();
                            activeBlock.MoveLeft();
                            activeBlock.MoveLeft();
                            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                            {
                                // #3
                                activeBlock.MoveRight();
                                activeBlock.MoveRight();
                                activeBlock.MoveRight();
                                activeBlock.MoveDown();
                                activeBlock.MoveDown();
                                if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                {
                                    // #4
                                    activeBlock.MoveUp();
                                    activeBlock.MoveUp();
                                    activeBlock.MoveUp();
                                    activeBlock.MoveLeft();
                                    activeBlock.MoveLeft();
                                    activeBlock.MoveLeft();
                                    if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                    {
                                        activeBlock.MoveDown();
                                        activeBlock.MoveRight();
                                        activeBlock.MoveRight();
                                        activeBlock.RotateRight();
                                    }
                                }
                            }
                        }
                    } else if (activeBlock.transform.rotation.eulerAngles.z == 0.00f)
                    {
                        // #1
                        activeBlock.MoveRight();
                        activeBlock.MoveRight();
                        if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                        {
                            // #2
                            activeBlock.MoveLeft();
                            activeBlock.MoveLeft();
                            activeBlock.MoveLeft();
                            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                            {
                                // #3
                                activeBlock.MoveUp();
                                activeBlock.MoveRight();
                                activeBlock.MoveRight();
                                activeBlock.MoveRight();
                                if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                {
                                    // #4
                                    activeBlock.MoveDown();
                                    activeBlock.MoveDown();
                                    activeBlock.MoveDown();
                                    activeBlock.MoveLeft();
                                    activeBlock.MoveLeft();
                                    activeBlock.MoveLeft();
                                    if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
                                    {
                                        activeBlock.MoveUp();
                                        activeBlock.MoveUp();
                                        activeBlock.MoveRight();
                                        activeBlock.RotateRight();
                                    }
                                }
                            }
                        }
                    }
                } else
                {
                    activeBlock.RotateRight();
                }
            }
            MoveOnBoard(board.CheckPosition(activeBlock));

            // 画面外から元の場所に戻した結果盤面上のブロックと被った場合は、あきらめて元のpos,rotationに戻す
            if (board.CheckPosition(activeBlock) == BlockValidation.Occupied)
            {
                activeBlock.transform.position = tempActiveBlockTransform.position;
                activeBlock.transform.rotation = tempActiveBlockTransform.rotation;
            }
            return true;
        }

        return false;
    }

    public bool Drop()
    {
        // swipe 操作があったら
        if (Input.GetKey(KeyCode.S) && (Time.time > nextKeyDropTimer) || (Time.time > nextDropTimer)
            || (isSwipeEnabled && InputSwipe && (Time.time > nextKeyShiftTimer))
            )
        {
            activeBlock.MoveDown();
            nextKeyDropTimer = Time.time + nextKeyDropInterval;
            nextDropTimer = Time.time + dropInterval;

            if (board.CheckPosition(activeBlock) == BlockValidation.BottomOver
                || board.CheckPosition(activeBlock) == BlockValidation.Occupied)
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
        InputFingerCheck();

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

            if (board.CheckPosition(activeBlock) != BlockValidation.Success)
            {
                activeBlock.RotateUp();
            }
        }
        // 下移動
        else if (isDropEnabled && Drop())
        {
            return;
        }

        // ホールド ：画面タッチ(指が動いていない)
        if (
#if UNITY_EDITOR
            isHoldEnabled && Input.GetKey(KeyCode.H) && (Time.time > nextKeyShiftTimer) || isHoldEnabled && Input.GetKeyDown(KeyCode.H)
             || isHoldEnabled && Input.GetMouseButton(0) && (Time.time > nextKeyShiftTimer) ||
#endif
             (isHoldEnabled && InputTouch && (Time.time > nextKeyShiftTimer))
            )
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
        if (holdBlock)
        {
            b.transform.position -= new Vector3(0, -5, 0);
        }
        upcomingBlocks.Enqueue(b);

        nextKeyDropTimer = Time.time;
        nextKeyShiftTimer = Time.time;
        nextKeyRotateTimer = Time.time;

        List<int> filledRows = board.CheckFilledRows();
        await effectsManager.PlayLineClearEffect(filledRows);
        board.ClearFilledRows();
    }

    public void MoveOnBoard(BlockValidation blockValidation)
    {
        if (blockValidation == BlockValidation.RightOver)
        {
            activeBlock.transform.position += new Vector3(-1, 0, 0) * board.overBlockNum;
        }
        else if (blockValidation == BlockValidation.LeftOver)
        {
            activeBlock.transform.position += new Vector3(1, 0, 0)* board.overBlockNum;
        }
        else if (blockValidation == BlockValidation.BottomOver)
        {
            activeBlock.MoveUp();
        }
    }

    void HoldBlock()
    {
        if (holdBlock)
        {
            Block tmpBlock = holdBlock;
            Vector3 currentPosition = activeBlock.transform.position;

            holdBlock = activeBlock;
            holdBlock.transform.position = new Vector3(11, 0, -1);
            holdBlock.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            activeBlock = tmpBlock;
            activeBlock.transform.position = currentPosition;
        }
        else
        {
            Vector3 currentPosition = activeBlock.transform.position;
            holdBlock = activeBlock;
            holdBlock.transform.position = new Vector3(11, 0, -1); 
            holdBlock.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            activeBlock = upcomingBlocks.Dequeue();
            foreach (var block in upcomingBlocks)
            {
                block.transform.position -= new Vector3(0, -5, 0);
            }
            activeBlock.transform.position = currentPosition;
        }

        MoveOnBoard(board.CheckPosition(activeBlock));
    }

    // スワイプ検知
    void InputFingerCheck() 
    {
        if (Input.touches.Length <= 0)
        {
            InputTouch = false;
            InputSwipe = false;
            return;
        }
    
        if (Input.touches[0].phase == TouchPhase.Began)
        {
            InputTouch = false;
            InputSwipe = false;
            fingerUp = Input.touches[0].position;
            fingerDown = Input.touches[0].position;
        }

        if (Input.touches[0].phase == TouchPhase.Moved)
        {
            fingerUp = Input.touches[0].position;
            if (Mathf.Abs(fingerDown.y - fingerUp.y) > SWIPE_THRESHOLD)
            {
                InputSwipe = true;
            }
        }

        if (Input.touches[0].phase == TouchPhase.Ended)
        {
            fingerUp = Input.touches[0].position;
            if (Mathf.Abs(fingerDown.y - fingerUp.y) < SWIPE_THRESHOLD)
            {
                InputTouch = true;
                InputSwipe = false;
            }
            else
            {
                InputSwipe = true; 
                InputTouch = false;
            }
        }
    }

    void SimulateShadowBlock()
    {
        Destroy(shadowBlock.gameObject);
        shadowBlock = Instantiate(activeBlock, this.transform);
        foreach (Transform item in shadowBlock.transform)
        {
            Renderer render = item.gameObject.GetComponent<Renderer>();
            render.material = shadowMaterial;
        }
        // 最初の数秒間非表示
        shadowBlock.gameObject.SetActive(false);
        if (activeBlock.transform.position.y < 24)
        {
            shadowBlock.gameObject.SetActive(true);
            shadowBlock.transform.position = activeBlock.transform.position;
            while (board.CheckPosition(shadowBlock) == BlockValidation.Success)
            {
                shadowBlock.MoveDown();
            }
            shadowBlock.MoveUp();
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
        Camera.main.GetComponent<MoveWithGyro>().ResetGyro();
    }
    

    public void EnableHorizontalMove()
    {
        isHorizontalMoveEnabled = true;
    }
    
    public void EnableRotation()
    {
        isRotationEnabled = true;
    }
    
    public void EnableHold()
    {
        isHoldEnabled = true;
    }

    public void EnableDrop()
    {
        isDropEnabled = true;
    }
    
    public void EnableSwipe()
    {
        isSwipeEnabled = true;
    }
    
    public void PrepareForTutorial()
    {
        activeBlock.transform.position = new Vector3(4, 18, 0);

        isHorizontalMoveEnabled = false;
        isRotationEnabled = false;
        isHoldEnabled = false;
        isSwipeEnabled = false;
        isDropEnabled = false;
        
        pause = false;
    }
}

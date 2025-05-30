using optionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TurnController;

/// <summary>
/// CollisionControllerクラスは、プレイヤーとオブジェクト（箱など）の衝突処理を管理する。
/// 箱のアニメーション、エフェクト、音、カメラ操作、ゲームの状態（フェーズ）管理などを担当する。
/// </summary>
public class CollisionController : MonoBehaviour
{
    //アニメーターのパラメーターのハッシュを定義
    private static readonly int Open = Animator.StringToHash("open");
    private static readonly int Property = Animator.StringToHash("Bool Walk");

    // TurnControllerへの参照(フェーズ進行などを制御)
    private TurnController turnController;

    // ImageControllerへの参照（UI表示制御などに使用）
    private ImageController imageController;

    // 箱を開けるボタンのゲームオブジェクト（UI上の操作対象）
    [SerializeField] private GameObject openBottom;

    // 箱を戻す（キャンセル）ボタンのゲームオブジェクト
    [SerializeField] private GameObject buckBottom;

    // 箱が開いたときのパーティクルオブジェクト
    [SerializeField] private GameObject particle;

    // 爆弾オブジェクト（開けた箱が爆弾だった場合に使用）
    [SerializeField] private GameObject bomb;

    // 箱を開けたときのサウンド
    [SerializeField] private AudioClip openSound;

    // 爆発時のサウンド
    [SerializeField] private AudioClip explosionSound;

    // キャンセル（戻る）時のサウンド
    [SerializeField] private AudioClip canselSound;

    // ClickControllerへの参照（クリック操作や箱選択を制御）
    private ClickController clickController;

    // カメラ操作を制御するスクリプトへの参照
    private CamController camController;

    // 敵キャラクターの移動を制御するスクリプト
    private EnemyMoveController enemyMoveController;

    // プレイヤーのワープ先位置（Transform参照）
    [SerializeField] private Transform warpPoint;

    // 箱の開閉アニメーション用アニメーター
    [SerializeField] private Animator animator;

    // パーティクルシステム（演出用）
    [SerializeField] private new ParticleSystem particleSystem;

    // 爆発したかどうかのフラグ（複数回爆発処理が実行されないよう制御）
    private bool isExplosion;

    // 敵が箱を開ける状態かを判定するフラグ
    private bool enemyOpen;

    // カメラを元の位置に戻すかどうかのフラグ（trueなら戻す）
    public bool cameraBuck = true;

    // コルーチン一時停止制御用のフラグ（未使用）
    private const bool StopCoroutineFlag = false;

    // 箱を開ける処理のコルーチン参照（停止・再開に使用）
    private Coroutine boxOpenCoroutine;

    // コルーチンの一時停止フラグ
    private bool isPaused;

    // Start is called before the first frame update
    private void Start()
    {
        // 自身に付属するAnimatorコンポーネントを取得
        animator = GetComponent<Animator>();

        // TurnControllerをシーン内から検索して参照を取得（フェーズ進行管理用）
        turnController = FindObjectOfType<TurnController>();

        // カメラの制御を行うCamControllerを取得
        camController = FindObjectOfType<CamController>();

        // particleにアタッチされているParticleSystemコンポーネントを取得
        particleSystem = particle.GetComponent<ParticleSystem>();

        // UIの制御などに使用するImageControllerを取得
        imageController = FindObjectOfType<ImageController>();
    }

    // Update is called once per frame
    private void Update()
    {
        // プレイヤーが「次へ」クリックを待っている時
        if (OptionController.Instance.clickNext)
        {
            if (Input.GetMouseButtonDown(0)) // クリックを検出
            {
                if (isPaused)
                {
                    OptionController.Instance.clickNext = false;
                    isPaused = false; // アニメーション中断を解除
                    camController.MotionAids(); // カメラ補助復帰
                }
            }
        }

        // カメラをターゲットに向けて移動
        if (camController.isCameraMoving && camController.targetObject)
        {
            camController.mainCamera.transform.position = Vector3.Lerp(
                camController.mainCamera.transform.position,
                camController.targetObject.position + new Vector3(0, 2, 0), // カメラの目標位置
                Time.deltaTime * camController.cameraMoveSpeed
            );
        }

        //openBottomが表示中の場合
        if (openBottom.activeSelf)
        {
            //時間経過でアニメーションが自動で実行
            OptionController.Instance.openTime -= Time.deltaTime;

            if (OptionController.Instance.openTime <= 0f)
            {
                animator.SetBool(Open, true); // ボックスを開く
                openBottom.SetActive(false);
                buckBottom.SetActive(false);
                OptionController.Instance.openTime = 60f;
                turnController.countText.enabled = false;
            }

            // 時間が少なくなったらカウント表示
            if (OptionController.Instance.openTime <= 30f)
            {
                turnController.countText.enabled = true;

                turnController.countText.text = "" + OptionController.Instance.openTime;
            }
        }

        // 爆発演出が終了していればボックスを非表示にする
        if (isExplosion != true) return;
        if (!particleSystem || particleSystem.IsAlive()) return;
        bomb.SetActive(false);

        BottomInvisible();
    }

    /// <summary>
    /// 衝突時の処理。フェーズ状態と衝突相手によって動作を分岐。
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 現在のstateを取得
        var currentState = turnController.GetCurrentState();

        // ゲームオブジェクトが再生成された場合のため、毎回Find
        enemyMoveController = FindObjectOfType<EnemyMoveController>();

        //playerが消えた時再度読み込まなくてはならないのでここに置く
        clickController = FindObjectOfType<ClickController>();

        switch (other.gameObject.tag)
        {
            // プレイヤーが選択箱に移動してきたとき
            case "Player" when currentState == PhaseState.PlayerMoveToChoiceBox:
                clickController.isMoving = false; // フラグをリセット
                clickController.animator.SetBool(Property, false);
                StartCoroutine(MovePlayerToWarpPoint()); // プレイヤーを箱の中心に移動
                camController.targetObject = other.transform; // ターゲットを当たったオブジェクトに設定
                camController.isCameraMoving = true; // カメラ移動を開始
                StartCoroutine(turnController.NextState()); // 次フェーズへ移行
                BottomEmerge();
                break;
            // プレイヤーがセット箱に移動したとき
            case "Player":
            {
                if (currentState == PhaseState.PlayerMoveToSetBox)
                {
                    clickController.isMoving = false; // フラグをリセット
                    clickController.animator.SetBool(Property, false);
                    StartCoroutine(MovePlayerToWarpPoint()); // セット地点に移動
                    clickController.ActivateOtherColliders(); // 他のコライダーを有効化
                    StartCoroutine(turnController.NextState()); // 次フェーズへ
                }

                break;
            }
            // 敵が選択箱に移動してきたとき
            case "Enemy" when currentState == PhaseState.EnemyMoveToChoiceBox:
            {
                enemyMoveController.enemyMoving = false;
                enemyMoveController.enemyAnimator.SetBool(Property, false);
                StartCoroutine(MoveEnemyToWarpPoint());
                camController.targetObject = other.transform;
                camController.isCameraMoving = true;

                // boxOpenはアニメーションイベントで実行される
                if (camController.targetObject == other.transform)
                {
                    //Animation Eventを使ってboxOpenを行う
                    animator.SetBool(Open, true);
                    enemyOpen = true;
                    StartCoroutine(turnController.NextState());
                }

                break;
            }
            // 敵がセット箱に移動したとき
            case "Enemy":
            {
                if (currentState == PhaseState.EnemyMoveToSetBox)
                {
                    enemyMoveController.enemyMoving = false;
                    enemyMoveController.enemyAnimator.SetBool(Property, false);
                    StartCoroutine(MoveEnemyToWarpPoint());
                    StartCoroutine(turnController.NextState());
                    turnController.randomObject.tag = "Explosion"; // 敵が爆弾を設置
                    turnController.PlayerTurn(); // プレイヤーのターンに移行
                }

                break;
            }
        }
    }

    /// <summary>
    /// プレイヤーをワープポイント（箱中央）へ滑らかに移動
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePlayerToWarpPoint()
    {
        const float duration = 1.0f; // 移動時間（秒）
        var elapsedTime = 0f;
        var startPosition = turnController.playerObject.transform.position;
        var targetPosition = warpPoint.transform.position;

        while (elapsedTime < duration)
        {
            turnController.playerObject.transform.position =
                Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        turnController.playerObject.transform.position = targetPosition; // 最終位置を確定
    }

    /// <summary>
    /// 敵をワープポイントへ滑らかに移動
    /// </summary>
    private IEnumerator MoveEnemyToWarpPoint()
    {
        const float duration = 1.0f; // 移動時間（秒）
        var elapsedTime = 0f;
        var startPosition = turnController.enemyObject.transform.position;
        var targetPosition = warpPoint.transform.position;

        while (elapsedTime < duration)
        {
            turnController.enemyObject.transform.position =
                Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        turnController.enemyObject.transform.position = targetPosition; // 最終位置を確定
    }

    /// <summary>
    /// 箱を開くアニメーションを再生（Animation Eventから呼び出される）
    /// </summary>
    public void OpenAnimation()
    {
        //Animation Eventを使ってboxOpenを行う
        animator.SetBool(Open, true);
        openBottom.SetActive(false);
        buckBottom.SetActive(false);
        OptionController.Instance.choiceTime = 60f;
        OptionController.Instance.openTime = 60f;
    }

    /// <summary>
    /// ボタンやエフェクトを非表示にし、状態をリセット
    /// </summary>
    public void BottomInvisible()
    {
        particle.SetActive(false);
        OptionController.Instance.openTime = 60f;
        isExplosion = false;
        clickController.ActivateOtherColliders();
        camController.MotionAids();
        camController.isCameraMoving = false;

        if (!openBottom.activeSelf || !buckBottom.activeSelf) return;
        openBottom.SetActive(false);
        buckBottom.SetActive(false);
    }

    /// <summary>
    /// 「戻る」ボタンが押されたときの処理
    /// </summary>
    public void Buck()
    {
        GetComponent<AudioSource>().PlayOneShot(canselSound);
        turnController.canselTrigger = false;
        OptionController.Instance.canselTime = true;
        turnController.BuckState();
        cameraBuck = false;
    }

    /// <summary>
    /// ボタンを出現させる
    /// </summary>
    private void BottomEmerge()
    {
        openBottom.SetActive(true);
        buckBottom.SetActive(true);
    }

    /// <summary>
    /// プレイヤーが箱を開ける処理を開始
    /// </summary>
    public void DeliveryBoxOpen()
    {
        if (boxOpenCoroutine != null) // 既存のコルーチンがある場合は停止
        {
            StopCoroutine(boxOpenCoroutine);
            boxOpenCoroutine = null;
        }

        isPaused = true;
        boxOpenCoroutine = StartCoroutine(BoxOpen());
    }

    /// <summary>
    /// 箱を開けるコルーチン。中身に応じて処理（スコア加算 or 爆発）を分岐
    /// </summary>
    private IEnumerator BoxOpen()
    {
        while (!StopCoroutineFlag)
        {
            if (isPaused)
            {
                OptionController.Instance.clickNext = true;
                yield return null; // 一時停止（次のフレームへ）
                continue; // ループの最初に戻る
            }

            // プレイヤーのターン処理
            if (!enemyOpen)
            {
                turnController.countText.enabled = false;

                if (this.gameObject.CompareTag("Cube"))
                {
                    turnController.randomObject.tag = "Cube";

                    // 番号の処理
                    var assignedNumber = turnController.ObjectNumberMapping[this.gameObject];
                    playerPoint += assignedNumber + 1;

                    BottomInvisible();
                    imageController.Safe();
                    yield return new WaitForSeconds(1f);

                    var tempList = new List<GameObject>(turnController.objectArray);
                    if (tempList.Contains(this.gameObject))
                    {
                        tempList.Remove(this.gameObject);
                        turnController.objectArray = tempList.ToArray();
                        this.gameObject.SetActive(false);
                    }

                    boxOpenCoroutine = null; // コルーチンの参照をクリア

                    yield break;
                }
                else if (this.gameObject.CompareTag("Explosion"))
                {
                    bomb.SetActive(true);
                    particle.SetActive(true);
                    playerLife -= 1;
                    playerPoint = 0;
                    isExplosion = true;

                    animator.SetBool(Open, false);
                    camController.isCameraMoving = false;

                    StartCoroutine(imageController.ExplosionSwitch());
                    GetComponent<AudioSource>().PlayOneShot(explosionSound);
                    this.gameObject.tag = "Cube";

                    yield return new WaitForSeconds(1f);
                    boxOpenCoroutine = null; // コルーチンの参照をクリア
                    yield break;
                }
            }
            // 敵のターン処理
            else
            {
                enemyOpen = false;
                camController.isCameraMoving = false;

                if (this.gameObject.CompareTag("Cube"))
                {
                    enemyPoint += turnController.ObjectNumberMapping[this.gameObject] + 1;
                    imageController.Safe();
                    yield return new WaitForSeconds(1f);
                    this.gameObject.SetActive(false);

                    var tempList = new List<GameObject>(turnController.objectArray);
                    tempList.Remove(this.gameObject);
                    turnController.objectArray = tempList.ToArray();

                    var objectsWithTag = GameObject.FindGameObjectsWithTag("Explosion");
                    foreach (var obj in objectsWithTag)
                    {
                        obj.tag = "Cube";
                    }

                    boxOpenCoroutine = null; // コルーチンの参照をクリア
                    yield break;
                }
                else if (this.gameObject.CompareTag("Explosion"))
                {
                    bomb.SetActive(true);
                    particle.SetActive(true);
                    enemyLife -= 1;
                    enemyPoint = 0;

                    //Animation Eventを使ってboxOpenを行う
                    animator.SetBool(Open, false);
                    StartCoroutine(imageController.ExplosionSwitch());
                    GetComponent<AudioSource>().PlayOneShot(explosionSound);
                    this.gameObject.tag = "Cube";

                    yield return new WaitForSeconds(1f);
                    boxOpenCoroutine = null; // コルーチンの参照をクリア
                    yield break;
                }
            }

            yield return null; // 次のフレームへ
        }
    }

    /// <summary>
    /// 箱を開ける際のサウンドを再生（Animation Eventで呼び出される）
    /// </summary>
    private void OpenSe()
    {
        GetComponent<AudioSource>().PlayOneShot(openSound);
    }
}
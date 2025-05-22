using optionSpace;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static TurnController;

/// <summary>
/// CamControllerはゲーム中のカメラ制御とフェード演出を担当するスクリプトです。
/// プレイヤーや敵の行動フェーズに応じてカメラの移動やフェードイン・アウトを行い、
/// 状態遷移と演出をつなぐ役割を持ちます。
/// </summary>
public class CamController : MonoBehaviour
{
    // 参照する他スクリプトのインスタンス
    [SerializeField] private TurnController turnController;
    [SerializeField] private CollisionController collisionController;

    // メインカメラの参照
    public Camera mainCamera;

    // カメラが移動中かどうかのフラグ
    public bool isCameraMoving;

    // カメラの初期位置
    [SerializeField] private Vector3 cameraStartPosition;

    // カメラが注視する対象
    public Transform targetObject;

    // カメラの移動速度
    public float cameraMoveSpeed = 2f;

    // フェード用のUIパネル
    [SerializeField] public GameObject panelFade;

    // フェード用画像のアルファ操作
    private Image fadeAlpha;
    private float alpha;
    private bool fadeInTrigger;

    // Start is called before the first frame update
    private void Start()
    {
        // カメラ初期位置を記録
        cameraStartPosition = mainCamera.transform.position;

        // フェードパネルのImage取得
        fadeAlpha = panelFade.GetComponent<Image>();

        // アルファ初期値取得
        alpha = fadeAlpha.color.a;

        // フェードインフラグ初期化
        fadeInTrigger = false;
    }

    // Update is called once per frame
    private void Update()
    {
        var currentState = turnController.GetCurrentState();

        // ボム設置フェーズ時にフェードアウト
        if (currentState is PhaseState.PlayerSetBomb or PhaseState.EnemyChoiceToSetBomb)
        {
            FadeOut();
        }

        // フェードインが必要な場合または敵がボムを設置するフェーズに入ったらフェードイン
        if(fadeInTrigger || currentState == PhaseState.EnemySetBomb)
        {
            FadeIn();
        }
    }

    /// <summary>
    /// カメラを元の位置に戻す演出を開始（ボックスオープン後など）
    /// </summary>
    public void MotionAids()
    {
        //現在のstateを取得
        var currentState = turnController.GetCurrentState();

        // カメラを戻すコルーチンを開始
        StartCoroutine(CameraBack());

        // ボックスオープンフェーズなら次の状態に遷移（戻るタイミングが来たとき）
        if (currentState is not (PhaseState.EnemyOpenBox or PhaseState.PlayerOpenBox)) return;
        
        if(collisionController.cameraBuck)
        {
            StartCoroutine(turnController.NextState());
        }
    }

    /// <summary>
    /// カメラを元の位置に戻す処理
    /// </summary>
    private IEnumerator CameraBack()
    {
        // 時間キャンセル状態に応じて待機時間を変更
        switch (OptionController.Instance.canselTime)
        {
            case false:
                yield return new WaitForSeconds(5f);
                break;
            case true:
                OptionController.Instance.canselTime = false;
                yield return new WaitForSeconds(2f);
                break;
        }
        // カメラ位置を初期位置に戻す
        mainCamera.transform.position = cameraStartPosition;

        // カメラ戻し完了フラグ
        collisionController.cameraBuck = true;
    }

    /// <summary>
    /// 画面を明るくしていく処理（フェードイン）
    /// </summary>
    private void FadeIn()
    {
        var currentState = turnController.GetCurrentState();

        // アルファ値を下げて透明にしていく
        alpha -= 0.01f;
        fadeAlpha.color = new Color(0,0,0,alpha);

        // 完全に透明になったら処理を切り替える
        if (!(alpha <= 0)) return;
        fadeInTrigger = false;

        // 状態に応じて処理を実行
        switch (currentState)
        {
            case PhaseState.EnemyChoiceToOpenBox:
                turnController.EnemyBoxChoice();
                break;
            case PhaseState.EnemySetBomb:
                StartCoroutine(turnController.NextState());
                break;
        }
    }

    /// <summary>
    /// 画面を暗くしていく処理（フェードアウト）
    /// </summary>
    private void FadeOut()
    {
        //現在のstateを取得
        var currentState = turnController.GetCurrentState();

        // アルファ値を上げて暗くしていく
        alpha += 1f;
        fadeAlpha.color = new Color(0,0,0,alpha);

        // 完全に暗くなったらフェーズに応じて処理を実行
        if (!(alpha >= 1)) return;
        
        switch (currentState)
        {
            case PhaseState.PlayerSetBomb:
                // プレイヤー設置後に次のフェーズへ
                StartCoroutine(turnController.NextState());
                fadeInTrigger = true;
                break;
            case PhaseState.EnemyChoiceToSetBomb:
                // 敵が設置場所選択 → 設置処理と次フェーズへ
                StartCoroutine(turnController.NextState());
                turnController.EnemyBombSet();
                break;
        }
    }
}

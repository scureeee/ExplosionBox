using System.Collections;
using UnityEngine;
using static TurnController;
using optionSpace;

/// <summary>
/// ClickControllerは、プレイヤーがマウスでボックスをクリックして移動・爆弾設置・開封を行う
/// 操作の中心となるクラスです。現在のフェーズに応じて挙動を切り替えます。
/// </summary>
public class ClickController : MonoBehaviour
{
    // アニメーターのパラメータ（移動中アニメーションを切り替えるためのハッシュ値）
    private static readonly int Property = Animator.StringToHash("Bool Walk");
    
    // プレイヤー回転補間速度
    [SerializeField] private float smooth = 10f;

    // 移動させる対象のプレイヤーオブジェクト
    [SerializeField] public GameObject player;

    // プレイヤー移動速度
    private const float MoveSpeed = 5f;

    // プレイヤーの目的地（クリックした場所）
    public Vector3 targetPosition;

    // 現在プレイヤーが移動中かどうか
    public bool isMoving;

    // プレイヤーのアニメーター
    public Animator animator;

    // フェーズを管理するTurnController
    [SerializeField] private TurnController turnController;

    //Start is called before the first frame update
    private void Start()
    {
        isMoving = false;

        // アニメーター取得
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        //現在のstateを取得
        var currentState = turnController.GetCurrentState();

        //クリック時の処理
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var clickedObject = hit.collider.gameObject;

                switch (currentState)
                {
                    // プレイヤーが爆弾を設置するために選択するフェーズ
                    case PhaseState.PlayerChoiceToSetBomb:
                    {
                        if (hit.collider.CompareTag("Cube"))
                        {
                            // Cubeを爆発対象に変更（爆弾が設置される）
                            hit.collider.gameObject.tag = "Explosion";

                            // 他のオブジェクトのColliderを無効化（再選択防止）
                            DeactivateOtherColliders(clickedObject);

                            // 移動先位置の保存
                            targetPosition = hit.point;

                            // タイマーリセット
                            OptionController.Instance.choiceTime = 60f;

                            isMoving = true;

                            // カウントテキスト非表示
                            turnController.countText.enabled = false;

                            // 次の状態へ進む
                            StartCoroutine(turnController.NextState());
                        }

                        break;
                    }
                    // プレイヤーがボックスを開けるフェーズ
                    case PhaseState.PlayerChoiceToOpenBox:
                    {
                        if (hit.collider.CompareTag("Cube") || hit.collider.CompareTag("Explosion"))
                        {
                            DeactivateOtherColliders(clickedObject);

                            targetPosition = hit.point;

                            isMoving = true;

                            turnController.countText.enabled = false;

                            StartCoroutine(turnController.NextState());
                        }

                        break;
                    }
                }
            }
        }

        // プレイヤー移動処理を実行
        if (isMoving)
        {
            MovePlayer();
        }
    }

    /// <summary>
    /// クリックされたオブジェクト以外のColliderを無効化して再選択を防止する
    /// </summary>
    /// <param name="clickedObject">クリックされたオブジェクト</param>
    public void DeactivateOtherColliders(GameObject clickedObject)
    {
        foreach (var obj in turnController.objectArray)
        {
            if (obj == clickedObject) continue;
            var collider = obj.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }
        }
    }

    /// <summary>
    /// 全てのオブジェクトのColliderを再有効化（ゲーム進行時に使用）
    /// </summary>
    public void ActivateOtherColliders()
    {
        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            var collider = obj.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = true;
            }

            obj.SetActive(true);
        }
    }

    /// <summary>
    /// プレイヤーを目的地へ移動させ、回転・アニメーションを調整
    /// </summary>
    private void MovePlayer()
    {
        // プレイヤーをターゲットに向かって移動
        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            targetPosition,
            MoveSpeed * Time.deltaTime
        );

        // 向きをターゲットに補間して回転
        var rotation = Quaternion.LookRotation(targetPosition);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);

        // 歩行アニメーションをON
        animator.SetBool(Property, true);
    }
}
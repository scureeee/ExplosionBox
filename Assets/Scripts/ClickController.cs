using System.Collections;
using UnityEngine;
using static TurnController;
using optionSpace;

public class ClickController : MonoBehaviour
{
    [SerializeField] float smooth = 10f;

    public GameObject player;

    //プレイヤーの移動速度
    public float moveSpeed = 5f;

    //移動先のターゲット位置
    public Vector3 targetPosition;

    //プレイヤーが移動中かどうか
    public bool isMoving = false;

    //アニメーターコンポーネント
    public Animator animator;

    private TurnController turnController;

    private OptionController optionController;

    // 再生成する NavMesh の範囲半径
    public float navMeshUpdateRadius = 5f;

    //Start is called before the first frame update
    void Start()
    {
        isMoving = false;

        animator = GetComponent<Animator>();

        optionController = FindObjectOfType<OptionController>();

        turnController = FindObjectOfType<TurnController>();

        StartCoroutine(WaitForTurnController());

        if (turnController != null)
        {
            turnController.InitializeObjectArray();
        }

        StartCoroutine(BuildNavMeshAsync());
    }

    // Update is called once per frame
    void Update()
    {

        // 現在のstateを取得
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        if (Input.GetMouseButtonDown(0))
        {
            //Rayを生成
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            //Rayを投射
            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject; // クリックしたオブジェクト

                //turnCountが整数か判別
                if (currentState == PhaseState.PlayerChoiceToSetBomb)
                {
                    if (hit.collider.CompareTag("Cube"))
                    {
                        hit.collider.gameObject.tag = "Explosion";

                        Debug.Log($"オブジェクト{hit.collider.gameObject.name}のタグを'Explosion'に変更しました。");

                        // NavMeshの非同期構築を開始
                        StartCoroutine(BuildNavMeshAsync());

                        // クリックしたオブジェクト以外のコライダーを無効化
                        DeactivateOtherColliders(clickedObject);

                        targetPosition = hit.point;

                        optionController.choiceTime = 60f;

                        // フラグを有効化
                        isMoving = true;

                        turnController.countText.enabled = false;

                        StartCoroutine(turnController.NextState());
                    }
                }
                //後で変える
                else if(currentState == PhaseState.PlayerChoiceToOpenBox)
                {
                    //タグを比較
                    //explosionが付いていない
                    if (hit.collider.CompareTag("Cube") || hit.collider.CompareTag("Explosion"))
                    {
                        // NavMeshの非同期構築を開始
                        StartCoroutine(BuildNavMeshAsync());

                        // クリックしたオブジェクト以外のコライダーを無効化
                        DeactivateOtherColliders(clickedObject);

                        targetPosition = hit.point;

                        // フラグを有効化
                        isMoving = true;

                        turnController.countText.enabled = false;

                        StartCoroutine(turnController.NextState());
                    }
                }
            }
        }

        // プレイヤーを移動させる処理
        if (isMoving)
        {
            MovePlayer();
        }
    }

    IEnumerator WaitForTurnController()
    {
        while (turnController == null || turnController.objectArray == null)
        {
            Debug.Log("WaitForTurnController: TurnController または objectArray が null です。待機中...");
            yield return new WaitForSeconds(0.5f);
            turnController = FindObjectOfType<TurnController>();
        }
        Debug.Log("WaitForTurnController: TurnController の準備完了！");
    }

    // クリックしたオブジェクト以外のコライダーを無効化するメソッド
    public void DeactivateOtherColliders(GameObject clickedObject)
    {
        if (turnController == null)
        {
            Debug.LogError("DeactivateOtherColliders: turnController が null です。");
            return;
        }

        if (turnController.objectArray == null)
        {
            Debug.LogError("DeactivateOtherColliders: objectArray が null です。");
            return;
        }

        foreach (GameObject obj in turnController.objectArray)
        {
            if (obj == null)
            {
                Debug.LogWarning("DeactivateOtherColliders: objectArray に null の要素があります。");
                continue;
            }

            if (obj != clickedObject)
            {
                Collider collider = obj.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false; // コライダーを無効化
                }
            }
        }

        Debug.Log("クリックしたオブジェクト以外のコライダーを無効化しました。");
    }

    public void ActivateOtherColliders()
    {
        // "対象オブジェクト" を判定する条件に基づき取得する
        // 必要に応じてタグや名前で絞り込む
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true; // コライダーを有効化
            }

            obj.SetActive(true); // オブジェクト自体をアクティブ化
        }

        Debug.Log("シーン内のすべての対象オブジェクトをアクティブにしました。");
    }

    public IEnumerator BuildNavMeshAsync()
    {
        yield return new WaitForSeconds(0.1f); // NavMesh構築前に少し待機
        Debug.Log("NavMeshの構築が完了しました。");
    }

    private void MovePlayer()
    {
        // プレイヤーをターゲット位置に向けて移動
        //MoveTowards関数によってスムーズに移動する
        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        //体の向きを滑らかに変更する
        Quaternion rotation = Quaternion.LookRotation(targetPosition);
        //最初に向いている方向からTime.deltaTime * smoothでtargetPositionに向きを変える
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);

        animator.SetBool("Bool Walk", true);
        // ターゲット位置に到達したら移動を終了
    }
}
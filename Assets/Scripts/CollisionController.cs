using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static TurnController;
using ImageSpace;

public class CollisionController : MonoBehaviour
{

    private TurnController turnController;

    private ImageController imageController;

    public GameObject openBotton;

    public GameObject buckBotton;

    public GameObject particle;

    public GameObject bomb;

    private ClickController clickController;

    private CamController camController;

    private EnemyMoveController enemyMoveController;

    public Transform warpPoint;

    private float openTime;

    //アニメーターコンポーネント

    public Animator animator;

    // パーティクルシステムの参照
    public new ParticleSystem particleSystem;

    private bool isExplosion = false;

    private bool enemyOpen = false;

    // Start is called before the first frame update

    void Start()
    {

        openTime = 60f;

        animator = GetComponent<Animator>();

        turnController = FindObjectOfType<TurnController>();

        camController = FindObjectOfType<CamController>();

        particleSystem = particle.GetComponent<ParticleSystem>();

        imageController = FindObjectOfType<ImageController>();
    }

    // Update is called once per frame
    void Update()
    {

        // カメラをターゲットに向けて移動
        if (camController.isCameraMoving && camController.targetObject != null)
        {
            camController.mainCamera.transform.position = Vector3.Lerp(
                camController.mainCamera.transform.position,
                camController.targetObject.position + new Vector3(0, 2, 0), // カメラの目標位置
                Time.deltaTime * camController.cameraMoveSpeed
            );
        }


        //Debug.Log($"open"+openTime);

        //openBottonが有ったら
        if (openBotton.activeSelf)
        {
            //時間経過でアニメーションが自動で実行
            openTime -= Time.deltaTime;

            if (openTime <= 0f)
            {
                //Animation Eventを使ってboxOpenを行う

                //改善中
                animator.SetBool("open", true);

                openBotton.SetActive(false);

                buckBotton.SetActive(false);

                openTime = 60f;

                turnController.countText.enabled = false;
            }


            if(openTime <= 30f)
            {
                turnController.countText.enabled = true;

                turnController.countText.text = "" + openTime;
            }
        }

        if(isExplosion == true)
        {
            if (particleSystem != null && !particleSystem.IsAlive())
            {
                bomb.SetActive(false);

                Debug.Log("届いてる");
                BottonInbisible();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 現在のstateを取得
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        //start時に読み込めないのでここに置く
        enemyMoveController = FindObjectOfType<EnemyMoveController>();

        //playerが消えた時再度読み込まなくてはならないのでここに置く
        clickController = FindObjectOfType<ClickController>();

        if (other.gameObject.tag == "Player")
        {
            if(currentState == PhaseState.PlayerMoveToChoiceBox)
            {
                clickController.isMoving = false; // フラグをリセット

                clickController.animator.SetBool("Bool Walk", false);

                // NavMeshAgentの移動を完全に停止
                clickController.agent.isStopped = true; // NavMeshAgentを停止
                clickController.agent.velocity = Vector3.zero; // 移動速度をリセット

                //目的地に移動し終えたplayerを定位置に移動させる
                turnController.playerObject.transform.position = warpPoint.transform.position;

                // カメラを当たったオブジェクトに近づける処理を開始
                camController.targetObject = other.transform; // ターゲットを当たったオブジェクトに設定
                camController.isCameraMoving = true; // カメラ移動を開始

                StartCoroutine(turnController.NextState());

                BottonEmerge();
            }
            else if(currentState == PhaseState.PlayerMoveToSetBox)
            {
                clickController.isMoving = false; // フラグをリセット

                clickController.animator.SetBool("Bool Walk", false);

                // NavMeshAgentの移動を完全に停止
                clickController.agent.isStopped = true; // NavMeshAgentを停止
                clickController.agent.velocity = Vector3.zero; // 移動速度をリセット

                //目的地に移動し終えたplayerを定位置に移動させる
                turnController.playerObject.transform.position = warpPoint.transform.position;

                clickController.ActivateOtherColliders();

                StartCoroutine(turnController.NextState());
            }
        }
        else if (other.gameObject.tag == "Enemy")
        {
            if(currentState == PhaseState.EnemyMoveToChoiceBox)
            {
                enemyMoveController.enemyMoving = false;

                enemyMoveController.enemyAnimator.SetBool("Bool Walk", false);

                turnController.enemyObject.transform.position = warpPoint.transform.position;

                camController.targetObject = other.transform;
                camController.isCameraMoving = true;

                if (camController.targetObject = other.transform)
                {
                    Debug.Log("疲れた");

                    //Animation Eventを使ってboxOpenを行う
                    animator.SetBool("open", true);

                    enemyOpen = true;

                    StartCoroutine(turnController.NextState());
                }
            }
            else if(currentState == PhaseState.EnemyMoveToSetBox)
            {
                enemyMoveController.enemyMoving = false;

                enemyMoveController.enemyAnimator.SetBool("Bool Walk", false);

                turnController.enemyObject.transform.position = warpPoint.transform.position;

                StartCoroutine(turnController.NextState());

                turnController.randomObject.tag = "Explosion";
                Debug.Log($"Enemyがオブジェクト {turnController.randomObject.name} のタグを 'Explosion' に変更しました。");

                turnController.PlayerTurn();
            }
        }

    }

    public void OpenAnimation()
    {
        //Animation Eventを使ってboxOpenを行う
        animator.SetBool("open", true);

        openBotton.SetActive(false);

        buckBotton.SetActive(false);

        openTime = 60f;
    }

    public void BottonInbisible()

    {

        particle.SetActive(false);

        openTime = 60f;

        isExplosion = false;

        clickController.ActivateOtherColliders();

        camController.MotionAids();

        camController.isCameraMoving = false;

        if (openBotton.activeSelf && buckBotton.activeSelf)
        {
            openBotton.SetActive(false);

            buckBotton.SetActive(false);
        }
    }

    public void Buck()
    {
        turnController.BuckState();
    }

    public void BottonEmerge()
    {

        openBotton.SetActive(true);

        buckBotton.SetActive(true);

        turnController.choiceTime = 60f;
    }

    public void boxOpen()
    {
        if(enemyOpen == false)
        {
            turnController.countText.enabled = false;

            camController.MotionAids();

            if (this.gameObject.tag == "Cube")
            {

                Debug.Log("cubeだよ");

                turnController.randomObject.tag = "Cube";

                // オブジェクトの番号を取得

                // オブジェクトの番号取得

                int assignedNumber = turnController.objectNumberMapping[this.gameObject];

                // 番号+1をポイントに加算

                playerPoint += assignedNumber + 1;

                Debug.Log(playerPoint);

                // 選択したオブジェクトをリストから削除

                List<GameObject> tempList = new List<GameObject>(turnController.objectArray);

                if (tempList.Contains(this.gameObject))
                {

                    tempList.Remove(this.gameObject);  // リストから削除

                    turnController.objectArray = tempList.ToArray();  // 配列に戻す

                    // オブジェクトを非アクティブ化する

                    this.gameObject.SetActive(false);

                    Debug.Log($"{this.gameObject.name} を配列から削除しました。");

                    //StartCoroutine(turnController.NextState());

                }
                imageController.Safe();

                BottonInbisible();
            }
            else if (this.gameObject.tag == "Explosion")
            {

                Debug.Log("Explosionだよ");

                bomb.SetActive(true);

                particle.SetActive(true);

                playerLife -= 1;

                playerPoint = 0;

                isExplosion = true;

                this.gameObject.tag = "Cube";

                animator.SetBool("open", false);

                camController.isCameraMoving = false;

                //StartCoroutine(turnController.NextState());

                StartCoroutine(imageController.ExplosionSwitch());
            }
        }
        else
        {
            //変える
            enemyOpen = false;

            camController.MotionAids();

            camController.isCameraMoving = false;

            if (turnController.randomObject.tag == "Cube")
            {
                Debug.Log("Enemyがcubeを触った");

                enemyPoint += turnController.objectNumberMapping[turnController.randomObject] + 1;

                turnController.randomObject.SetActive(false);

                // 配列から削除
                //配列からlistに変え配列の要素数を削除できるようにする
                List<GameObject> tempList = new List<GameObject>(turnController.objectArray);
                tempList.Remove(turnController.randomObject);
                turnController.objectArray = tempList.ToArray();

                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Explosion");

                foreach (GameObject obj in objectsWithTag)
                {
                    obj.tag = "Cube";
                }

                Debug.Log(enemyPoint);

                if (turnController.turnCount < OptionController.maxTurn)
                {
                    //StartCoroutine(turnController.NextState());
                }

                if (tempList.Contains(this.gameObject))
                {
                    tempList.Remove(this.gameObject);  // リストから削除
                    turnController.objectArray = tempList.ToArray();  // 配列に戻す

                    //cubeだった箱を消す
                    this.gameObject.SetActive(false);

                    Debug.Log($"{this.gameObject.name} を配列から削除しました。");
                }
                imageController.Safe();
            }
            else if (turnController.randomObject.tag == "Explosion")
            {
                bomb.SetActive(true);

                particle.SetActive(true);

                enemyLife -= 1;

                enemyPoint = 0;

                turnController.randomObject.tag = "Cube";

                //Animation Eventを使ってboxOpenを行う
                animator.SetBool("open", false);

                Debug.Log("EnemyがExplosionを触った");

                if (turnController.turnCount < OptionController.maxTurn)
                {
                    //StartCoroutine(turnController.NextState());
                }
                StartCoroutine(imageController.ExplosionSwitch());
            }
        }
    }
}


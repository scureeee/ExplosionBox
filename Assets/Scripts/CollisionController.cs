using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionController : MonoBehaviour
{

    private TurnController turnController;

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

        openTime = 0f;

        animator = GetComponent<Animator>();

        clickController = FindObjectOfType<ClickController>();

        turnController = FindObjectOfType<TurnController>();

        camController = FindObjectOfType<CamController>();

        particleSystem = particle.GetComponent<ParticleSystem>();
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
            //openTime += Time.deltaTime;

            if (openTime >= 7f)
            {
                //Animation Eventを使ってboxOpenを行う

                //改善中
                animator.SetBool("open", true);

                openBotton.SetActive(false);

                buckBotton.SetActive(false);

                openTime = 0f;
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
        //start時に読み込めないのでここに置く
        enemyMoveController = FindObjectOfType<EnemyMoveController>();

        if (other.gameObject.tag == "Player")
        {

            clickController.isMoving = false; // フラグをリセット

            clickController.animator.SetBool("Bool Walk", false);

            // NavMeshAgentの移動を完全に停止
            clickController.agent.isStopped = true; // NavMeshAgentを停止
            clickController.agent.velocity = Vector3.zero; // 移動速度をリセット

            //目的地に移動し終えたplayerを元の場所に戻す
            turnController.playerObject.transform.position = warpPoint.transform.position;


            // カメラを当たったオブジェクトに近づける処理を開始
            camController.targetObject = other.transform; // ターゲットを当たったオブジェクトに設定
            camController.isCameraMoving = true; // カメラ移動を開始


            BottonEmerge();

        }
        else if (other.gameObject.tag == "Enemy")
        {
            enemyMoveController.enemyMoving = false;

            enemyMoveController.enemyAnimator.SetBool("Bool Walk", false);

            turnController.enemyObject.transform.position = warpPoint.transform.position;

            camController.targetObject = other.transform;
            camController.isCameraMoving = true;

            if(camController.targetObject = other.transform)
            {
                Debug.Log("疲れた");

                //Animation Eventを使ってboxOpenを行う
                animator.SetBool("open", true);

                enemyOpen = true;
            }
        }

    }

    public void OpenAnimation()
    {
        //Animation Eventを使ってboxOpenを行う
        animator.SetBool("open", true);

        openBotton.SetActive(false);

        buckBotton.SetActive(false);

        openTime = 0f;
    }

    public void BottonInbisible()

    {

        particle.SetActive(false);

        openTime = 0f;

        turnController.choiceTrigger = true;

        isExplosion = false;

        clickController.ActivateOtherColliders();

        camController.isCameraMoving = false;

        camController.CameraBack();

        if (openBotton.activeSelf && buckBotton.activeSelf)
        {
            openBotton.SetActive(false);

            buckBotton.SetActive(false);
        }
    }

    public void BottonEmerge()
    {

        openBotton.SetActive(true);

        buckBotton.SetActive(true);

        //openCamera.SetActive(true);

        turnController.choiceTrigger = false;

        turnController.choiceTime = 0f;
    }

    public void boxOpen()
    {
        
        if(enemyOpen == false)
        {
            if (this.gameObject.tag == "Cube")
            {

                Debug.Log("cubeだよ");

                turnController.randomObject.tag = "Cube";

                // オブジェクトの番号を取得

                // オブジェクトの番号取得

                int assignedNumber = turnController.objectNumberMapping[this.gameObject];

                // 番号+1をポイントに加算

                turnController.playerPoint += assignedNumber + 1;

                Debug.Log(turnController.playerPoint);

                // 選択したオブジェクトをリストから削除

                List<GameObject> tempList = new List<GameObject>(turnController.objectArray);

                if (tempList.Contains(this.gameObject))

                {

                    tempList.Remove(this.gameObject);  // リストから削除

                    turnController.objectArray = tempList.ToArray();  // 配列に戻す

                    // オブジェクトを非アクティブ化する

                    this.gameObject.SetActive(false);

                    Debug.Log($"{this.gameObject.name} を配列から削除しました。");

                }

                BottonInbisible();

                if (turnController.turnCount < OptionController.maxTurn)

                {

                    //ターンを進める

                    turnController.turnCount += 0.5f;

                }

            }
            else if (this.gameObject.tag == "Explosion")
            {

                Debug.Log("Explosionだよ");

                bomb.SetActive(true);

                particle.SetActive(true);

                turnController.playerLife -= 1;

                turnController.playerPoint = 0;

                isExplosion = true;

                this.gameObject.tag = "Cube";

                if (turnController.turnCount < OptionController.maxTurn)

                {

                    //ターンを進める

                    turnController.turnCount += 0.5f;

                }
                animator.SetBool("open", false);
            }
        }
        else
        {
            enemyOpen = false;

            camController.CameraBack();

            camController.isCameraMoving = false;

            if (turnController.randomObject.tag == "Cube")
            {
                Debug.Log("Enemyがcubeを触った");

                turnController.enemyPoint += turnController.objectNumberMapping[turnController.randomObject] + 1;

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

                Debug.Log(turnController.enemyPoint);

                if (turnController.turnCount < OptionController.maxTurn)
                {
                    turnController.EnemyBombSite();
                }

                if (turnController.turnCount < OptionController.maxTurn)
                {
                    //ターンを進める
                    turnController.turnCount += 0.5f;
                }

                if (tempList.Contains(this.gameObject))
                {
                    tempList.Remove(this.gameObject);  // リストから削除
                    turnController.objectArray = tempList.ToArray();  // 配列に戻す

                    //cubeだった箱を消す
                    this.gameObject.SetActive(false);

                    Debug.Log($"{this.gameObject.name} を配列から削除しました。");
                }
            }
            else if (turnController.randomObject.tag == "Explosion")
            {
                if (turnController.turnCount < OptionController.maxTurn)
                {
                    //ターンを進める
                    turnController.turnCount += 0.5f;
                }

                turnController.enemyLife -= 1;

                turnController.enemyPoint = 0;

                turnController.randomObject.tag = "Cube";


                Debug.Log("EnemyがExplosionを触った");

                if (turnController.turnCount < OptionController.maxTurn)
                {
                    turnController.EnemyBombSite();
                }
            }
        }
    }

}


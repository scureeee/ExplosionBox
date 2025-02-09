using optionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static TurnController;

public class CollisionController : MonoBehaviour
{

    private TurnController turnController;

    private ImageController imageController;

    public GameObject openBotton;

    public GameObject buckBotton;

    public GameObject particle;

    public GameObject bomb;

    public AudioClip openSound;

    public AudioClip explsionSound;

    public AudioClip canselSound;

    private ClickController clickController;

    private CamController camController;

    private EnemyMoveController enemyMoveController;

    private OptionController optionController;

    public Transform warpPoint;

    //アニメーターコンポーネント

    public Animator animator;

    // パーティクルシステムの参照
    public new ParticleSystem particleSystem;

    private bool isExplosion = false;

    private bool enemyOpen = false;

    public bool cameraBuck = true;

    private bool stopCoroutineFlag = false;

    private Coroutine boxOpenCoroutine;

    public bool isPaused = false;  // コルーチンの一時停止フラグ
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        optionController = FindObjectOfType<OptionController>();

        turnController = FindObjectOfType<TurnController>();

        camController = FindObjectOfType<CamController>();

        particleSystem = particle.GetComponent<ParticleSystem>();

        imageController = FindObjectOfType<ImageController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (optionController.clickNext == true)
        {
            if (Input.GetMouseButtonDown(0))  // クリックを検出
            {
                Debug.Log("押され");
                if (isPaused)
                {
                    optionController.clickNext = false;
                    isPaused = false;  // 停止を解除
                    Debug.Log("コルーチン再開！");
                    camController.MotionAids();
                }
            }
        }

        // カメラをターゲットに向けて移動
        if (camController.isCameraMoving && camController.targetObject != null)
        {
            camController.mainCamera.transform.position = Vector3.Lerp(
                camController.mainCamera.transform.position,
                camController.targetObject.position + new Vector3(0, 2, 0), // カメラの目標位置
                Time.deltaTime * camController.cameraMoveSpeed
            );
        }


        //Debug.Log($"open"+optionController.openTime);

        //openBottonが有ったら
        if (openBotton.activeSelf)
        {
            //時間経過でアニメーションが自動で実行
            optionController.openTime -= Time.deltaTime;

            if (optionController.openTime <= 0f)
            {
                //Animation Eventを使ってboxOpenを行う

                //改善中
                animator.SetBool("open", true);

                openBotton.SetActive(false);

                buckBotton.SetActive(false);

                optionController.openTime = 60f;

                turnController.countText.enabled = false;
            }


            if(optionController.openTime <= 30f)
            {
                turnController.countText.enabled = true;

                turnController.countText.text = "" + optionController.openTime;
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

        //selectedObject = this.gameObject; // 衝突したオブジェクトを保存

        if (other.gameObject.tag == "Player")
        {
            if(currentState == PhaseState.PlayerMoveToChoiceBox)
            {
                clickController.isMoving = false; // フラグをリセット

                clickController.animator.SetBool("Bool Walk", false);

                StartCoroutine(MovePlayerToWarpPoint());

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

                StartCoroutine(MovePlayerToWarpPoint());

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

                StartCoroutine(MoveEnemyToWarpPoint());

                camController.targetObject = other.transform;
                camController.isCameraMoving = true;

                if (camController.targetObject == other.transform)
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

                StartCoroutine(MoveEnemyToWarpPoint());

                StartCoroutine(turnController.NextState());

                turnController.randomObject.tag = "Explosion";
                Debug.Log($"Enemyがオブジェクト {turnController.randomObject.name} のタグを 'Explosion' に変更しました。");

                turnController.PlayerTurn();
            }
        }

    }

    private IEnumerator MovePlayerToWarpPoint()
    {
        float duration = 1.0f; // 移動時間（秒）
        float elapsedTime = 0f;

        Vector3 startPosition = turnController.playerObject.transform.position;
        Vector3 targetPosition = warpPoint.transform.position;

        while (elapsedTime < duration)
        {
            turnController.playerObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        turnController.playerObject.transform.position = targetPosition; // 最終位置を確定
    }

    private IEnumerator MoveEnemyToWarpPoint()
    {
        float duration = 1.0f; // 移動時間（秒）
        float elapsedTime = 0f;

        Vector3 startPosition = turnController.enemyObject.transform.position;
        Vector3 targetPosition = warpPoint.transform.position;

        while (elapsedTime < duration)
        {
            turnController.enemyObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        turnController.enemyObject.transform.position = targetPosition; // 最終位置を確定
    }


    public void OpenAnimation()
    {
        //Animation Eventを使ってboxOpenを行う
        animator.SetBool("open", true);

        openBotton.SetActive(false);

        buckBotton.SetActive(false);

        optionController.openTime = 60f;
    }

    public void BottonInbisible()
    {

        particle.SetActive(false);

        optionController.openTime = 60f;

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
        GetComponent<AudioSource>().PlayOneShot(canselSound);

        turnController.BuckState();

        cameraBuck = false;
    }

    public void BottonEmerge()
    {

        openBotton.SetActive(true);

        buckBotton.SetActive(true);

        optionController.choiceTime = 60f;
    }

    public void DeliveryBoxOpen()
    {
        Debug.Log("コルーチン開始！");

        if (boxOpenCoroutine != null)  // 既存のコルーチンがある場合は停止
        {
            Debug.Log("既存のコルーチンを停止して再開します。");
            StopCoroutine(boxOpenCoroutine);
            boxOpenCoroutine = null;
        }

        isPaused = true;
        boxOpenCoroutine = StartCoroutine(BoxOpen());
    }


    IEnumerator BoxOpen()
    {
        Debug.Log("BoxOpen開始");

        while (!stopCoroutineFlag)
        {
            if (isPaused)
            {
                Debug.Log("コルーチン一時停止中...");
                optionController.clickNext = true;
                yield return null;  // 一時停止（次のフレームへ）
                continue;  // ループの最初に戻る
            }

            if (enemyOpen == false)
            {
                turnController.countText.enabled = false;

                if (this.gameObject.tag == "Cube")
                {
                    Debug.Log("cubeだよ");
                    turnController.randomObject.tag = "Cube";

                    // 番号の処理
                    int assignedNumber = turnController.objectNumberMapping[this.gameObject];
                    playerPoint += assignedNumber + 1;

                    BottonInbisible();
                    imageController.Safe();

                    yield return new WaitForSeconds(1f);
                    List<GameObject> tempList = new List<GameObject>(turnController.objectArray);
                    if (tempList.Contains(this.gameObject))
                    {
                        tempList.Remove(this.gameObject);
                        turnController.objectArray = tempList.ToArray();
                        this.gameObject.SetActive(false);
                    }

                    boxOpenCoroutine = null;  // コルーチンの参照をクリア

                    yield break;
                }
                else if (this.gameObject.tag == "Explosion")
                {
                    Debug.Log("Explosionだよ");

                    bomb.SetActive(true);
                    particle.SetActive(true);

                    playerLife -= 1;
                    playerPoint = 0;

                    isExplosion = true;

                    animator.SetBool("open", false);
                    camController.isCameraMoving = false;

                    StartCoroutine(imageController.ExplosionSwitch());
                    GetComponent<AudioSource>().PlayOneShot(explsionSound);

                    this.gameObject.tag = "Cube";
                    yield return new WaitForSeconds(1f);

                    boxOpenCoroutine = null;  // コルーチンの参照をクリア

                    yield break;
                }
            }
            else
            {

                enemyOpen = false;
                camController.isCameraMoving = false;
                if (this.gameObject.tag == "Cube")
                {
                    enemyPoint += turnController.objectNumberMapping[this.gameObject] + 1;
                    imageController.Safe();

                    yield return new WaitForSeconds(1f);
                    this.gameObject.SetActive(false);
                    List<GameObject> tempList = new List<GameObject>(turnController.objectArray);
                    tempList.Remove(this.gameObject);
                    turnController.objectArray = tempList.ToArray();

                    GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Explosion");
                    foreach (GameObject obj in objectsWithTag)
                    {
                        obj.tag = "Cube";
                    }

                    Debug.Log(enemyPoint);

                    boxOpenCoroutine = null;  // コルーチンの参照をクリア

                    yield break;
                }
                else if (this.gameObject.tag == "Explosion")
                {

                    bomb.SetActive(true);

                    particle.SetActive(true);

                    enemyLife -= 1;

                    enemyPoint = 0;

                    //Animation Eventを使ってboxOpenを行う
                    animator.SetBool("open", false);

                    Debug.Log("EnemyがExplosionを触った");

                    GetComponent<AudioSource>().PlayOneShot(explsionSound);

                    this.gameObject.tag = "Cube";
                    yield return new WaitForSeconds(1f);

                    boxOpenCoroutine = null;  // コルーチンの参照をクリア

                    yield break;
                }
            }
            yield return null;  // 次のフレームへ
        }
        // ループを抜ける場合も `boxOpenCoroutine = null;` を実行
        boxOpenCoroutine = null;
    }

    private void OpenSe()
    {
        GetComponent<AudioSource>().PlayOneShot(openSound);
    }
}

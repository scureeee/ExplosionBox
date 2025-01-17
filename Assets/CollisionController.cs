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

    public Transform warpPoint;

    private float openTime;

    //アニメーターコンポーネント

    public Animator animator;

    // パーティクルシステムの参照
    public new ParticleSystem particleSystem;

    private bool isExplosion = false;

    // Start is called before the first frame update

    void Start()
    {

        openTime = 0f;

        animator = GetComponent<Animator>();

        clickController = FindObjectOfType<ClickController>();

        turnController = FindObjectOfType<TurnController>();

        particleSystem = particle.GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
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

        if (other.gameObject.tag == "Player")
        {

            clickController.isMoving = false; // フラグをリセット

            clickController.animator.SetBool("Bool Walk", false);

            // NavMeshAgentの移動を完全に停止
            clickController.agent.isStopped = true; // NavMeshAgentを停止
            clickController.agent.velocity = Vector3.zero; // 移動速度をリセット

            //目的地に移動し終えたplayerを元の場所に戻す
            turnController.playerObject.transform.position = warpPoint.transform.position;

            BottonEmerge();

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

        //openCamera.SetActive(false);

        particle.SetActive(false);

        openTime = 0f;

        turnController.choiceTrigger = true;

        isExplosion = false;

        clickController.ActivateOtherColliders();

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

}


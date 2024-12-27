using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionController : MonoBehaviour
{

    private TurnController turnController;

    public GameObject openBotton;

    public GameObject buckBotton;

    public GameObject openCamera;

    public GameObject bomb;

    private ClickController clickController;

    public Transform warpPoint;

    private float openTime;

    //アニメーターコンポーネント

    public Animator animator;

    // パーティクルシステムの参照
    public new ParticleSystem particleSystem;

    // Start is called before the first frame update

    void Start()
    {

        openTime = 0f;

        animator = GetComponent<Animator>();

        clickController = FindObjectOfType<ClickController>();

        turnController = FindObjectOfType<TurnController>();

        particleSystem = bomb.GetComponent<ParticleSystem>();

    }

    // Update is called once per frame

    void Update()

    {

        //Debug.Log($"open"+openTime);

        //openBottonが有ったら

        if (openBotton.activeSelf)

        {

            //時間経過でアニメーションが自動で実行

            openTime += Time.deltaTime;

            if (openTime >= 7f)

            {

                //Animation Eventを使ってboxOpenを行う

                animator.SetBool("open", true);

            }

        }
        if (particleSystem != null && !particleSystem.IsAlive())
        {
            BottonInbisible();
        }
    }

    void OnTriggerStay(Collider other)

    {

        if (other.gameObject.tag == "Player")

        {

            //Debug.Log("playerが");

            //turnController.playerObject.SetActive(false);

            clickController.isMoving = false; // フラグをリセット

            clickController.animator.SetBool("Bool Walk", false);

            //目的地に移動し終えたplayerを元の場所に戻す

            turnController.playerObject.transform.position = warpPoint.transform.position;

            BottonEmerge();

        }

    }

    public void OpenAnimation()

    {

        //Animation Eventを使ってboxOpenを行う

        animator.SetBool("open", true);

    }

    public void BottonInbisible()

    {

        openBotton.SetActive(false);

        openCamera.SetActive(false);

        buckBotton.SetActive(false);

        bomb.SetActive(false);

        openTime = 0f;

        turnController.choiceTrigger = true;

    }

    public void BottonEmerge()

    {

        openBotton.SetActive(true);

        buckBotton.SetActive(true);

        openCamera.SetActive(true);
    }

    public void boxOpen()

    {


        if (this.gameObject.tag == "Cube")

        {

            //Debug.Log("cubeだよ");

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

            //Debug.Log("Explosionだよ");

            bomb.SetActive(true);

            turnController.playerLife -= 1;

            turnController.playerPoint = 0;

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


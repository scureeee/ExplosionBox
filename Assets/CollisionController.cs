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

    public GameObject Bomb;

    public GameObject Explosion;

    private ClickController clickController;

    //アニメーターコンポーネント
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        clickController = FindObjectOfType<ClickController>();  
        
        turnController = FindObjectOfType<TurnController>();

        Bomb.SetActive(true);

        Explosion.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("playerが");

            turnController.playerObject.SetActive(false);

            clickController.isMoving = false; // フラグをリセット
            clickController.animator.SetBool("Bool Walk", false);

            //目的地に移動し終えたplayerを元の場所に戻す
            turnController.playerObject.transform.position = Vector3.zero;

            BottonEmerge();
        }
    }

    public void BottonInbisible()
    {

        openBotton.SetActive(false);

        openCamera.SetActive(false);

        buckBotton.SetActive(false);

        turnController.playerObject.SetActive(true);
    }

    IEnumerator WaitForAnimationAndExecute()
    {
        // アニメーションのトリガーを設定
        animator.SetTrigger("Open");

        // アニメーションが終了するまで待機
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 3f)
        {
            // アニメーションが再生中の場合は待機
            yield return null;
        }

        // アニメーション終了後に行いたい処理
        Debug.Log("アニメーション終了！");

        // ここにアニメーション終了後に行いたい処理を記述

        Explosion.SetActive(true);

        BottonInbisible();

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


            //StartCoroutine(WaitForAnimationAndExecute());
            //BottonInbisible();

            if (turnController.turnCount < OptionController.maxTurn)
            {
                //ターンを進める
                turnController.turnCount = turnController.turnCount + 0.5f;
            }
        }
        else if (this.gameObject.tag == "Explosion")
        {
            //Debug.Log("Explosionだよ");

            turnController.playerLife = turnController.playerLife - 1;

            this.gameObject.tag = "Cube";

            if (turnController.turnCount < OptionController.maxTurn)
            {
                //ターンを進める
                turnController.turnCount = turnController.turnCount + 0.5f;
            }

            //StartCoroutine(WaitForAnimationAndExecute());
            BottonInbisible();           
        }
    }

    public void BottonEmerge()
    {
        openBotton.SetActive(true);

        buckBotton.SetActive(true);
        
        openCamera.SetActive(true);
    }

    public void boxOpen()
    {
        //animator.SetBool("bounce", true);
        //animator.SetBool("open", true);


        StartCoroutine(WaitForAnimationAndExecute());

    }
}


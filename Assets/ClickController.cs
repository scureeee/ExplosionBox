using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ClickController : MonoBehaviour
{
    [SerializeField] float smooth = 10f;

    public GameObject player;

    //プレイヤーの移動速度
    public float moveSpeed = 5f;

    //移動先のターゲット位置
    private Vector3 targetPosition;

    //プレイヤーが移動中かどうか
    private bool isMoving = false;

    //アニメーターコンポーネント
    private Animator animator;
    //Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Rayを生成
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            //Rayを投射
            if(Physics.Raycast(ray, out hit))
            {
                //タグを比較
                if(hit.collider.CompareTag("Cube"))
                {
                    //プレイヤーをcubeに移動させる
                    targetPosition = hit.point;

                    isMoving = true; // フラグを有効化
                }
                if(hit.collider.CompareTag("Base"))
                {
                    Debug.Log("KKK");
                    //プレイヤーをcubeに移動させる
                    targetPosition = hit.point;
                }
            }
        }

        // プレイヤーを移動させる処理
        if (isMoving)
        {
            MovePlayer();
        }
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
        if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
        {
            isMoving = false; // フラグをリセット
            animator.SetBool("Bool Walk", false);
        }
    }
}

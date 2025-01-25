using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController : MonoBehaviour
{

    //アニメーターコンポーネント
    public Animator enemyAnimator;

    //移動先のターゲット位置
    public Vector3 enemyTarget;

    [SerializeField] float enemySmooth = 10f;

    //プレイヤーの移動速度
    public float enemyMoveSpeed = 5f;

    public bool enemyMoving = false;

    [SerializeField] TurnController turnController;

    // Start is called before the first frame update
    void Start()
    {
        turnController = FindObjectOfType<TurnController>();

        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyMoving == true)
        {
            MoveEnemy();
        }
    }

    public void MoveEnemy()
    {
        // プレイヤーをターゲット位置に向けて移動
        //MoveTowards関数によってスムーズに移動する
        turnController.enemyObject.transform.position = Vector3.MoveTowards(
            turnController.enemyObject.transform.position,
            enemyTarget,
            enemyMoveSpeed * Time.deltaTime
        );

        //体の向きを滑らかに変更する
        Quaternion rotation = Quaternion.LookRotation(enemyTarget);
        //最初に向いている方向からTime.deltaTime * smoothでtargetPositionに向きを変える
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * enemySmooth);

        enemyAnimator.SetBool("Bool Walk", true);
        // ターゲット位置に到達したら移動を終了
    }
}

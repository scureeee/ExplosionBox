using UnityEngine;

/// <summary>
/// 敵キャラクターの移動処理を制御するクラス。
/// ターゲット位置に向かってスムーズに移動し、アニメーションを再生する。
/// </summary>
public class EnemyMoveController : MonoBehaviour
{
    // Animatorの"Bool Walk"パラメータをハッシュ化（最適化のため）
    private static readonly int Property = Animator.StringToHash("Bool Walk");

    // 敵のAnimatorコンポーネント
    public Animator enemyAnimator;

    // 敵が目指すターゲット位置（座標）
    public Vector3 enemyTarget;

    // 敵の回転速度（補間係数）
    [SerializeField] private float enemySmooth = 10f;

    // 敵の移動速度
    private const float EnemyMoveSpeed = 5f;

    // 敵が移動中かどうかのフラグ
    public bool enemyMoving;

    // ゲーム全体のターン制御を行うコントローラーへの参照
    [SerializeField] private TurnController turnController;

    // Start is called before the first frame update
    private void Start()
    {
        // このオブジェクトのAnimatorコンポーネントを取得
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        //敵が移動中であれば移動処理を行う。
        if (enemyMoving)
        {
            MoveEnemy();
        }
    }

    /// <summary>
    /// 敵をターゲットの座標に向けて移動させ、回転させ、アニメーションを再生する。
    /// </summary>
    private void MoveEnemy()
    {
        // 現在位置からターゲット位置へ一定速度で移動させる
        turnController.enemyObject.transform.position = Vector3.MoveTowards(
            turnController.enemyObject.transform.position,
            enemyTarget,
            EnemyMoveSpeed * Time.deltaTime
        );

        // ターゲット方向に向かってスムーズに回転させる
        var rotation = Quaternion.LookRotation(enemyTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * enemySmooth);

        // AnimatorのWalkパラメータをtrueに設定し、移動アニメーションを再生
        enemyAnimator.SetBool(Property, true);
    }
}
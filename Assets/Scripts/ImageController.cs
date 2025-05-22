using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static TurnController;

/// <summary>
/// プレイヤー・敵の行動フェーズに応じてUI画像（Image）を切り替える。
/// また、爆発・セーフ演出のエフェクトと音声再生も制御する。
/// </summary>
public class ImageController : MonoBehaviour
{
    [SerializeField] public Image targetImage;// UIに表示する画像（フェーズに応じて切り替える）
    [SerializeField] public Sprite playerSet;// プレイヤーが爆弾を設置するフェーズ用のスプライト
    [SerializeField] public Sprite playerOpen;// プレイヤーが箱を開けるフェーズ用のスプライト
    [SerializeField] public Sprite enemySet;// 敵が爆弾を設置するフェーズ用のスプライト
    [SerializeField] public Sprite enemyOpen;// 敵が箱を開けるフェーズ用のスプライト
    [SerializeField] public GameObject explosion;// 爆発演出のGameObject
    [SerializeField] public GameObject safe;// セーフ演出のGameObject
    [SerializeField] public AudioClip safeSound;// セーフ演出時の音声

    // 画像変更を許可するフラグ
    public bool imageTrigger;

    // ゲームの状態管理を行うTurnControllerへの参照
    [SerializeField] private TurnController turnController;

    private void Start()
    {
        // TurnControllerを取得し、画像変更フラグを初期化
        imageTrigger = true;
    }

    private void Update()
    {
        // 現在のフェーズを取得
        var currentState = turnController.GetCurrentState();

        // フェーズに応じてスプライトと透明度を変更
        switch (currentState)
        {
            case PhaseState.PlayerChoiceToSetBomb when imageTrigger:
                targetImage.sprite = playerSet;
                targetImage.color = new Color(1f, 1f, 1f, 1f);
                break;
            case PhaseState.PlayerChoiceToOpenBox when imageTrigger:
                targetImage.sprite = playerOpen;
                targetImage.color = new Color(1f, 1f, 1f, 1f);
                break;
            case PhaseState.EnemyChoiceToSetBomb when imageTrigger:
                targetImage.sprite = enemySet;
                targetImage.color = new Color(1f, 1f, 1f, 1f);
                break;
            case PhaseState.EnemyChoiceToOpenBox when imageTrigger:
                targetImage.sprite = enemyOpen;
                targetImage.color = new Color(1f, 1f, 1f, 1f);
                break;
        }
    }

    /// <summary>
    /// 爆発演出を表示するコルーチン。
    /// 5秒間有効にしたあと非表示にする。
    /// </summary>
    public IEnumerator ExplosionSwitch()
    {
        explosion.SetActive(true); // 爆発演出を表示
        yield return new WaitForSeconds(5f);// 5秒待機
        explosion.SetActive(false); // 非表示に戻す
    }

    /// <summary>
    /// セーフ演出と音声を再生する関数。
    /// コルーチンを開始する。
    /// </summary>
    public void Safe()
    {
        StartCoroutine(SafeSwitch());
    }
    
    /// <summary>
    /// セーフ演出用コルーチン。
    /// 音声を再生し、5秒間演出を表示する。
    /// </summary>
    private IEnumerator SafeSwitch()
    {
        // AudioSourceを使って効果音を再生
        GetComponent<AudioSource>().PlayOneShot(safeSound);
        
        safe.SetActive(true);// セーフ演出を表示
        yield return new WaitForSeconds(5f);// 5秒待機
        safe.SetActive(false);// 非表示にする
    }
}
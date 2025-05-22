using System.Collections;
using UnityEngine;

/// <summary>
/// メニューUIのパネルをスライドイン／スライドアウトで表示・非表示にする制御クラス。
/// </summary>
public class MenuController : MonoBehaviour
{
    // スライドアニメーションの動きを定義（ここでは線形：時間に対して直線的に移動）
    private readonly AnimationCurve slideAnimation = AnimationCurve.Linear(0, 0, 1, 1);

    // パネルがスライドインする目標座標（ローカル座標）
    [SerializeField] private Vector3 inPosition;

    // パネルがスライドアウトする目標座標（ローカル座標）
    [SerializeField] private Vector3 outPosition;

    // アニメーションにかける時間（秒）
    private const float Duration = 1.0f;
    
    /// <summary>
    /// パネルをスライドインさせる（表示する）
    /// </summary>
    public void SlideIn()
    {
        // スライドイン用のコルーチンを開始
        StartCoroutine(StartSlidePanel(true));
    }

    /// <summary>
    /// パネルをスライドアウトさせる（非表示にする）
    /// </summary>
    public void SlideOut()
    {
        // スライドアウト用のコルーチンを開始
        StartCoroutine(StartSlidePanel(false));
    }

    /// <summary>
    /// スライドアニメーションを処理するコルーチン
    /// </summary>
    /// <param name="isSlideIn">true: スライドイン, false: スライドアウト</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator StartSlidePanel(bool isSlideIn)
    {
        // アニメーション開始時間を記録
        var startTime = Time.time;

        // 現在のローカル位置を取得（アニメーションの開始地点）
        var startPos = transform.localPosition;

        // 移動量ベクトルを算出（in/outに応じて）
        Vector3 moveDistance;

        if (isSlideIn)
        {
            moveDistance = inPosition - startPos;
        }
        else
        {
            moveDistance= outPosition - startPos;
        }

        // 指定時間（Duration）かけてアニメーションを実行
        while ((Time.time - startTime) < Duration)
        {
            //スライドアニメーション中の現在位置を更新する
            transform.localPosition = startPos + moveDistance * slideAnimation.Evaluate((Time.time - startTime) / Duration);

            yield return null;// 1フレーム待機
        }
        transform.localPosition = startPos + moveDistance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject を使ってゲーム結果（勝敗や引き分け）の状態を保持・共有するクラス。
/// ResultJudge などのスクリプトから状態をセットし、ResultScene で参照される想定。
/// </summary>
[CreateAssetMenu (fileName = "ResultState", menuName = "ScriptableObjects/ResultState")]
public class ResultState : ScriptableObject
{
    /// <summary>
    /// 勝敗・結果の種類を定義した列挙体
    /// </summary>
    public enum ResultStateType
    {
        None,      // 初期状態、未設定
        LifeLose,  // ライフが0になったことによる敗北
        LifeWin,   // 相手のライフが0になったことによる勝利
        PointLose, // 相手が先に最大ポイントに到達したための敗北
        PointWin,  // プレイヤーが先に最大ポイントに到達したための勝利
        TurnLose,  // 規定ターン終了時にポイント負け
        TurnWin,   // 規定ターン終了時にポイント勝ち
        Draw       // 規定ターン終了時にポイントが同点（引き分け）
    }
    
    [Header("現在の結果状態")]
    public ResultStateType currentState;// 現在の結果状態を保持
    
    /// <summary>
    /// 状態をセット
    /// </summary>
    public void SetState(ResultStateType state)
    {
        currentState = state;
    }
}

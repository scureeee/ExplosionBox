using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using optionSpace;

/// <summary>
/// ゲームの勝敗判定と、結果シーンへの遷移を管理するクラス。
/// プレイヤーのライフやポイント、ターン数などを元に判定を行う。
/// </summary>
public class ResultJudge : MonoBehaviour
{
    // 勝敗状態を保持する ResultState スクリプトの参照
    [SerializeField] private ResultState resultState;

    // ターン制御用スクリプトへの参照
    [SerializeField] private TurnController turnController;

    // 最終ターンを知らせる演出オブジェクト
    [SerializeField] private GameObject lastTurn;

    // 最終ターン通知を1回だけ実行するためのフラグ
    private bool lastTrigger = true;

    // Update is called once per frame
    private void Update()
    {
        // プレイヤーのライフが0になった場合 → 負け
        if (TurnController.playerLife == 0)
        {
            resultState.SetState(ResultState.ResultStateType.LifeLose);
            SceneManager.LoadScene("ResultScene");
        }

        // 敵のライフが0になった場合 → 勝ち
        if (TurnController.enemyLife == 0)
        {
            resultState.SetState(ResultState.ResultStateType.LifeWin);
            SceneManager.LoadScene("ResultScene");
        }

        // 敵のポイントが最大に達した場合 → 負け
        if (TurnController.enemyPoint >= OptionController.maxPoint)
        {
            resultState.SetState(ResultState.ResultStateType.PointLose);
            SceneManager.LoadScene("ResultScene");
        }

        // プレイヤーのポイントが最大に達した場合 → 勝ち
        if (TurnController.playerPoint >= OptionController.maxPoint)
        {
            resultState.SetState(ResultState.ResultStateType.PointWin);
            SceneManager.LoadScene("ResultScene");
        }

        // 残りオブジェクトが1つしかない（最終盤面）の場合 → ポイントで勝敗決定
        if (turnController.objectArray.Length == 1)
        {
            if (TurnController.playerPoint < TurnController.enemyPoint)
            {
                resultState.SetState(ResultState.ResultStateType.TurnLose);
                SceneManager.LoadScene("ResultScene");
            }

            else if (TurnController.playerPoint > TurnController.enemyPoint)
            {
                resultState.SetState(ResultState.ResultStateType.TurnWin);
                SceneManager.LoadScene("ResultScene");
            }

            else if (TurnController.playerPoint == TurnController.enemyPoint)
            {
                resultState.SetState(ResultState.ResultStateType.Draw);
                SceneManager.LoadScene("ResultScene");
            }
        }

        // 最大ターン数に達した場合 → ポイントで勝敗決定
        if (turnController.turnCount == OptionController.maxTurn)
        {
            if (TurnController.playerPoint < TurnController.enemyPoint)
            {
                resultState.SetState(ResultState.ResultStateType.TurnLose);
                SceneManager.LoadScene("ResultScene");
            }

            else if (TurnController.playerPoint > TurnController.enemyPoint)
            {
                resultState.SetState(ResultState.ResultStateType.TurnWin);
                SceneManager.LoadScene("ResultScene");
            }

            else if (TurnController.playerPoint == TurnController.enemyPoint)
            {
                resultState.SetState(ResultState.ResultStateType.Draw);
                SceneManager.LoadScene("ResultScene");
            }
        }

        // 最終ターン1つ前のタイミングで「ラストターン演出」を1回だけ表示する
        if (lastTrigger != true) return; // 一度表示したら以後スキップ
        if (turnController.turnCount != OptionController.maxTurn - 1) return; // 条件が一致しないときはスキップ
        lastTrigger = false; // 2度目以降に表示されないようにフラグをOFF

        StartCoroutine(Last()); // ラストターン演出のコルーチンを開始
    }

    /// <summary>
    /// 「ラストターン」のUIや演出を一時的に表示するコルーチン。
    /// </summary>
    private IEnumerator Last()
    {
        lastTurn.SetActive(true); // ラストターンオブジェクトを表示
        yield return new WaitForSeconds(2f); // 2秒間待つ
        lastTurn.SetActive(false); // オブジェクトを非表示に戻す
    }
}
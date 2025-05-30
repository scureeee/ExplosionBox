using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム終了時のリザルト表示を管理するスクリプト。
/// プレイヤーと敵のライフ・ポイント情報を表示し、勝敗・引き分け結果に応じたUIを表示する。
/// </summary>
public class ResultText : MonoBehaviour
{
    // 現在のリザルトの状態（勝ち負け・引き分け）を管理する
    [SerializeField] private ResultState resultState;

    // 各勝敗結果に対応するUIオブジェクト（事前にインスペクターで割り当てておく）
    [SerializeField] private GameObject lifeWin;
    [SerializeField] private GameObject lifeLose;
    [SerializeField] private GameObject pointWin;
    [SerializeField] private GameObject pointLose;
    [SerializeField] private GameObject turnWin;
    [SerializeField] private GameObject turnLose;
    [SerializeField] private GameObject draw;

    // プレイヤーと敵のライフ、ポイントを表示するためのTextMeshPro UI
    [SerializeField] private TextMeshProUGUI playerLifeText;
    [SerializeField] private TextMeshProUGUI enemyLifeText;
    [SerializeField] private TextMeshProUGUI playerPointText;
    [SerializeField] private TextMeshProUGUI enemyPointText;

    // Update is called once per frame
    private void Update()
    {
        // 現在の勝敗状態に応じて、対応するUIを表示
        switch (resultState.currentState)
        {
            case ResultState.ResultStateType.LifeLose:
                lifeLose.SetActive(true);
                break;
            case ResultState.ResultStateType.LifeWin:
                lifeWin.SetActive(true);
                break;
            case ResultState.ResultStateType.PointLose:
                pointLose.SetActive(true);
                break;
            case ResultState.ResultStateType.PointWin:
                pointWin.SetActive(true);
                break;
            case ResultState.ResultStateType.TurnLose:
                turnLose.SetActive(true);
                break;
            case ResultState.ResultStateType.TurnWin:
                turnWin.SetActive(true);
                break;
            case ResultState.ResultStateType.Draw:
                draw.SetActive(true);
                break;
        }

        // プレイヤーと敵の現在のライフ・ポイントを画面に表示（TurnControllerの静的変数を使用）
        playerLifeText.text = "" + TurnController.playerLife;
        enemyLifeText.text = "" + TurnController.enemyLife;
        playerPointText.text = "" + TurnController.playerPoint;
        enemyPointText.text = "" + TurnController.enemyPoint;
    }

    /// <summary>
    /// タイトル画面（オプションシーン）へ戻る処理。
    /// UIボタンなどから呼び出される。
    /// </summary>
    public void BuckTitle()
    {
        SceneManager.LoadScene("OptionScene");
    }
}
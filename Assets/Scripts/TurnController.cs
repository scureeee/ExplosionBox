using System.Collections;
using System.Collections.Generic;
using optionSpace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     TurnController クラスは、ターン制ゲームの進行管理を行う中枢スクリプト。
///     各プレイヤーのフェーズ（爆弾設置・箱選択など）の状態遷移を制御し、
///     UI更新、タイマー管理、ネットワーク同期（Photon）なども担う。
///     プレイヤーが先攻か後攻かによって、進行フェーズの構成が動的に決定される。
/// </summary>
public class TurnController : MonoBehaviour
{
    // プレイヤーが先攻なら0、敵が先攻なら1（ランダムで決定される）
    private int firstTurn;

    // 他scriptの参照
    [SerializeField] private ClickController clickController;
    [SerializeField] private EnemyMoveController enemyMoveController;
    [SerializeField] private ImageController imageController;

    // 現在のターン数
    public int turnCount;

    // 生成するオブジェクトのPrefab
    [SerializeField] public GameObject objectPrefab;

    // 円形に配置する際の半径
    private const float Radius = 10f;

    // 生成したオブジェクトの配列
    public GameObject[] objectArray;

    // 各オブジェクトの一意の番号を格納する辞書
    public Dictionary<GameObject, int> ObjectNumberMapping;

    // プレイヤー・敵・ランダム選択対象のオブジェクト
    public GameObject playerObject;
    public GameObject enemyObject;
    public GameObject randomObject;

    // ターン表示パネル関連
    [SerializeField] public GameObject turnPanel;
    private Vector3 startPosition; // パネル定位置
    private Vector3 targetPosition; // パネル移動場所
    private const float Duration = 1.0f; // 移動の時間

    // UI表示用
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI playerLifeText;
    [SerializeField] private TextMeshProUGUI playerPointText;
    [SerializeField] private TextMeshProUGUI enemyLifeText;
    [SerializeField] private TextMeshProUGUI enemyPointText;
    [SerializeField] public TextMeshProUGUI countText;

    // プレイヤーと敵のステータス(ポイント・ライフ)
    public static int playerLife;
    public static int enemyLife;
    public static int playerPoint;
    public static int enemyPoint;

    // 現在のフェーズインデックス
    private int currentIndex;

    // 次の状態に進めるかどうかのフラグ
    private bool nextTrigger = true;

    // タイマーキャンセル用のフラグ
    public bool canselTrigger;

    // ゲーム全体のフェーズ定義
    public enum PhaseState
    {
        EnemyChoiceToSetBomb,
        EnemyMoveToSetBox,
        EnemySetBomb,
        PlayerChoiceToOpenBox,
        PlayerMoveToChoiceBox,
        PlayerOpenBox,
        PlayerChoiceToSetBomb,
        PlayerMoveToSetBox,
        PlayerSetBomb,
        EnemyChoiceToOpenBox,
        EnemyMoveToChoiceBox,
        EnemyOpenBox
    }

    // ターンの順序(プレイヤー先行 or 敵先行)
    private Dictionary<int, PhaseState> currentState;

    // 敵が先行のフェーズ順
    private readonly Dictionary<int, PhaseState> firstEnemyState = new()
    {
        { 0, PhaseState.EnemyChoiceToSetBomb },
        { 1, PhaseState.EnemyMoveToSetBox },
        { 2, PhaseState.EnemySetBomb },
        { 3, PhaseState.PlayerChoiceToOpenBox },
        { 4, PhaseState.PlayerMoveToChoiceBox },
        { 5, PhaseState.PlayerOpenBox },
        { 6, PhaseState.PlayerChoiceToSetBomb },
        { 7, PhaseState.PlayerMoveToSetBox },
        { 8, PhaseState.PlayerSetBomb },
        { 9, PhaseState.EnemyChoiceToOpenBox },
        { 10, PhaseState.EnemyMoveToChoiceBox },
        { 11, PhaseState.EnemyOpenBox }
    };

    // プレイヤーが先行のフェーズ順
    private readonly Dictionary<int, PhaseState> firstPlayerState = new()
    {
        { 0, PhaseState.PlayerChoiceToSetBomb },
        { 1, PhaseState.PlayerMoveToSetBox },
        { 2, PhaseState.PlayerSetBomb },
        { 3, PhaseState.EnemyChoiceToOpenBox },
        { 4, PhaseState.EnemyMoveToChoiceBox },
        { 5, PhaseState.EnemyOpenBox },
        { 6, PhaseState.EnemyChoiceToSetBomb },
        { 7, PhaseState.EnemyMoveToSetBox },
        { 8, PhaseState.EnemySetBomb },
        { 9, PhaseState.PlayerChoiceToOpenBox },
        { 10, PhaseState.PlayerMoveToChoiceBox },
        { 11, PhaseState.PlayerOpenBox }
    };

    private void Start()
    {
        // ターン初期化
        turnCount = 1;

        // ポイント初期化
        playerPoint = 0;
        enemyPoint = 0;

        // ライフ設定(オプション画面で設定された最大値)
        playerLife = OptionController.maxLife;
        enemyLife = OptionController.maxLife;

        // プレイヤーが先行の順序を仮にセット(後でランダムに決定)
        SetFirstPlayerOrder(true);

        // DataManagerから設定されたオブジェクト数を取得
        var numberOfObjects = DataManager.Instance.objectCount;

        if (numberOfObjects <= 0)
        {
            Debug.LogWarning("オブジェクト数が0または負の値です。生成をスキップします。");
            return;
        }

        // 配列と辞書を初期化
        objectArray = new GameObject[numberOfObjects];
        ObjectNumberMapping = new Dictionary<GameObject, int>();

        // オブジェクトを円形に生成
        GenerateObjectsInCircle(numberOfObjects);

        // 先行をランダムに決定
        DecideFirstTurn();

        // 選択時間初期化
        OptionController.Instance.choiceTime = 60f;

        // パネルアニメーション用に位置記録
        startPosition = turnPanel.transform.position;
        targetPosition = new Vector3(startPosition.x, startPosition.y - 1, startPosition.z);
        StartCoroutine(AnimatePanel());
    }

    private void Update()
    {
        // 現在のstateを取得
        var phaseState = GetCurrentState();

        // 時間制限で箱をランダムで選択
        if (phaseState == PhaseState.PlayerChoiceToSetBomb || phaseState == PhaseState.PlayerChoiceToOpenBox)
        {
            // キャンセルされていない状態であれば、残り選択時間を減少
            if (!canselTrigger)
                // 待機時間
                OptionController.Instance.choiceTime -= Time.deltaTime;

            // 時間切れになったらランダムで選択処理を実行
            if (OptionController.Instance.choiceTime <= 0f)
            {
                // 残り時間をリセット
                OptionController.Instance.choiceTime = 60f;

                // タイマー表示を非表示
                countText.enabled = false;

                // ランダムに対象オブジェクト選択
                NumberRandom();

                switch (phaseState)
                {
                    // プレイヤーの爆弾設置フェーズの場合
                    case PhaseState.PlayerChoiceToSetBomb:
                    {
                        // cubeタグオブジェクトを対象にする
                        if (randomObject.CompareTag("Cube"))
                        {
                            // タグをExplosionに変更(爆弾設置)
                            randomObject.gameObject.tag = "Explosion";

                            // 移動先の位置を設定
                            clickController.targetPosition = randomObject.transform.position;

                            // プレイヤー移動を有効化
                            clickController.isMoving = true;

                            // 選択時間をリセット
                            OptionController.Instance.choiceTime = 60f;

                            // 次のフェイズへ移行
                            StartCoroutine(NextState());

                            // クリックしたオブジェクト以外のコライダーを無効化
                            clickController.DeactivateOtherColliders(randomObject);
                        }

                        break;
                    }
                    // プレイヤーの箱を開けるフェーズの場合
                    case PhaseState.PlayerChoiceToOpenBox:

                        // 移動先の位置を設定
                        clickController.targetPosition = randomObject.transform.position;

                        // プレイヤーの移動を有効化
                        clickController.isMoving = true;

                        // open時間の減少を停止
                        canselTrigger = true;

                        // open時間をリセット
                        OptionController.Instance.openTime = 0f;

                        // 次のフェーズへ移行
                        StartCoroutine(NextState());

                        // クリックしたオブジェクト以外のコライダーを無効化
                        clickController.DeactivateOtherColliders(randomObject);
                        break;
                }
            }
        }

        // UIテキストの更新
        playerPointText.text = playerPoint + ""; // プレイヤーの得点
        enemyPointText.text = enemyPoint + ""; // 敵の得点
        turnText.text = turnCount + ""; // 現在のターン数
        playerLifeText.text = "" + playerLife; // プレイヤーの残りライフ
        enemyLifeText.text = "" + enemyLife; // 敵の残りライフ

        // 箱を開けるフェーズの中で、残り時間が30秒以下ならカウントダウン表示
        if (phaseState == PhaseState.PlayerChoiceToOpenBox)
            if (OptionController.Instance.choiceTime <= 30)
            {
                countText.enabled = true;
                countText.text = "" + OptionController.Instance.choiceTime;
            }
    }

    /// <summary>
    ///     指定された数のオブジェクトの円形に配置しながら生成する
    /// </summary>
    /// <param name="numberOfObjects">生成するオブジェクトの数</param>
    private void GenerateObjectsInCircle(int numberOfObjects)
    {
        for (var i = 0; i < numberOfObjects; i++)
        {
            // 配置角度を計算
            var angle = i * Mathf.PI * 2 / numberOfObjects;
            var position = new Vector3(Mathf.Cos(-angle) * Radius, 0, Mathf.Sin(-angle) * Radius);

            // オブジェクト生成
            var obj = Instantiate(objectPrefab, position, Quaternion.identity, transform);
            objectArray[i] = obj;

            // 各オブジェクトに一意の番号を割り当て
            ObjectNumberMapping[obj] = i;

            // オブジェクトの名前に番号を設定
            obj.name = $"Object_{i}";

            // TextMeshProの追加
            var text = new GameObject("NumberText");

            // 親をオブジェクトに設定
            text.transform.SetParent(obj.transform);

            // 表示位置調整
            text.transform.localPosition = new Vector3(0, 2f, 0);

            var tmp = text.AddComponent<TextMeshPro>();
            tmp.text = (i + 1).ToString(); // 番号を+1して表示
            tmp.fontSize = 10; // フォントサイズ指定
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.red;
        }
    }

    /// <summary>
    ///     ランダムで先行プレイヤーを決定する
    /// </summary>
    private void DecideFirstTurn()
    {
        // 0か1をランダムに決定
        firstTurn = Random.Range(0, 2);

        if (firstTurn == 0)
        {
            // プレイヤーが先行
            SetFirstPlayerOrder(true);
            PlayerTurn();
        }
        else
        {
            // 敵が先行
            SetFirstPlayerOrder(false);
            EnemyBombSet();
        }
    }

    /// <summary>
    ///     プレイヤーの先攻/後攻に応じて状態遷移リストを切り替える
    /// </summary>
    /// <param name="isFirst">trueならplayerが先攻、falseならplayerが後攻</param>
    private void SetFirstPlayerOrder(bool isFirst)
    {
        currentState = isFirst ? firstPlayerState : firstEnemyState;
    }

    /// <summary>
    ///     現在のフェーズ状態を取得
    /// </summary>
    /// <returns></returns>
    public PhaseState GetCurrentState()
    {
        return currentState[currentIndex];
    }

    /// <summary>
    ///     状態に応じて次の状態へ遷移させるコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator NextState()
    {
        // 現在のstateを取得
        var phaseState = GetCurrentState();

        // プレイヤーまたは敵が箱を開けるフェーズか判定
        if (phaseState is PhaseState.EnemyOpenBox or PhaseState.PlayerOpenBox)
        {
            if (nextTrigger != true) yield break; // フラグが立っていない場合は停止
            nextTrigger = false;
            yield return new WaitForSeconds(5f); // 5秒待機してから遷移
            Next();
        }
        else
        {
            // 通常は即座に遷移
            Next();
            yield return null;
        }
    }

    /// <summary>
    ///     状態リストの次に進め、ターン数も適切に更新する
    /// </summary>
    private void Next()
    {
        // 次のインデックスに進む
        currentIndex++;

        // インデックスが順序の範囲外ならリセット
        if (currentIndex >= currentState.Count)
        {
            currentIndex = 0;
            turnCount++;
            StartCoroutine(AnimatePanel());
        }

        //特定のタイミングでターンを追加カウント
        if ((currentIndex + 1) % 7 == 0)
        {
            turnCount++;
            StartCoroutine(AnimatePanel()); // UI演出
        }

        // イメージの更新やフラグのリセット
        imageController.imageTrigger = true;
        nextTrigger = true;
        OptionController.Instance.clickNext = false;
    }

    /// <summary>
    ///     状態を一つ戻す(2つ戻すことで再度同じ状態にできる)
    /// </summary>
    public void BuckState()
    {
        // インデックスを -2 する（ただし、範囲外にならないように調整）
        currentIndex = Mathf.Max(0, currentIndex - 2);
    }

    /// <summary>
    ///     ターン表示パネルの演出
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimatePanel()
    {
        yield return StartCoroutine(ExpansionPanel(1f, 2f, startPosition, targetPosition, Duration)); // 拡大しながら中央へ
        yield return StartCoroutine(ExpansionPanel(2f, 1f, targetPosition, startPosition, Duration)); // 縮小しながら元の位置へ
    }

    /// <summary>
    ///     指定されたスケールと位置でターンパネルを補間アニメーション
    /// </summary>
    /// <param name="startScale"></param>
    /// <param name="endScale"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator ExpansionPanel(float startScale, float endScale, Vector3 startPos, Vector3 endPos, float time)
    {
        var elapsedTime = 0f;
        while (elapsedTime < time)
        {
            var t = elapsedTime / time;
            turnPanel.transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, t);
            turnPanel.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最終位置とスケールの固定
        turnPanel.transform.localScale = Vector3.one * endScale;
        turnPanel.transform.position = endPos;
    }

    /// <summary>
    ///     箱の中からランダムに1つ選択し、randomObjectに設定する
    /// </summary>
    private void NumberRandom()
    {
        // ランダムにオブジェクトを選択
        var randomIndex = Random.Range(0, objectArray.Length);
        randomObject = objectArray[randomIndex];
    }

    /// <summary>
    ///     敵の爆弾設置フェーズの処理(ランダムな箱に移動)
    /// </summary>
    public void EnemyBombSet()
    {
        // キャラの切り替え(敵を表示、プレイヤーを非表示)
        if (playerObject)
        {
            enemyObject.SetActive(true);

            playerObject.SetActive(false);
        }

        // オブジェクトがない場合は処理しない
        if (objectArray == null || objectArray.Length == 0) return;

        // ターン上限に達していたら処理を中断
        if (turnCount >= OptionController.maxTurn) return;

        // ランダムに箱を選択
        NumberRandom();

        // 敵の移動先と移動フラグを設定
        enemyMoveController.enemyTarget = randomObject.transform.position;
        enemyMoveController.enemyMoving = true;
    }

    /// <summary>
    ///     敵の箱選択フェーズの処理
    /// </summary>
    public void EnemyBoxChoice()
    {
        playerObject.SetActive(false);
        enemyObject.SetActive(true);

        // ランダムに箱を選択
        NumberRandom();

        // 次のフェーズへ移行
        StartCoroutine(NextState());

        // 敵の移動設定
        enemyMoveController.enemyTarget = randomObject.transform.position;
        enemyMoveController.enemyMoving = true;
    }

    /// <summary>
    ///     プレイヤーのターン開始処理
    /// </summary>
    public void PlayerTurn()
    {
        playerObject.SetActive(true);
        enemyObject.SetActive(false);
    }

    /// <summary>
    ///     リタイア
    /// </summary>
    public void Retirement()
    {
        SceneManager.LoadScene("OptionScene");
    }
}
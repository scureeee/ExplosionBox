using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq.Expressions;
using JetBrains.Annotations;

public class TurnController : MonoBehaviour
{
    private int firstTurn;

    private ClickController clickController;

    private EnemyMoveController enemyMoveController;

    private ImageController imageController;

    public float turnCount = 0f;

    // 生成するオブジェクトのPrefab
    public GameObject objectPrefab;

    // 円の半径
    public float radius = 10f;

    // 生成したオブジェクトの配列
    public GameObject[] objectArray;

    // 各オブジェクトの一意の番号を格納する辞書
    public Dictionary<GameObject, int> objectNumberMapping;

    public GameObject playerObject;

    public GameObject enemyObject;

    public GameObject randomObject;

    [SerializeField] private TextMeshProUGUI turnText;

    [SerializeField] private TextMeshProUGUI playerLifeText;

    [SerializeField] private TextMeshProUGUI playerPointText;

    [SerializeField] private TextMeshProUGUI enemyLifeText;

    [SerializeField] private TextMeshProUGUI enemyPointText;

    [SerializeField] public TextMeshProUGUI countText;

    public static int playerLife = 0;

    public static int enemyLife = 0;

    public static int playerPoint = 0;

    public static int enemyPoint = 0;

    public float choiceTime = 60f;

    private int currentIndex;

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

    //あとでアクセッサ入れる
    public Dictionary<int, PhaseState> currentState;

    private Dictionary<int, PhaseState> firstEnemyState = new Dictionary<int, PhaseState>
    {
        {0,PhaseState.EnemyChoiceToSetBomb},
        {1,PhaseState.EnemyMoveToSetBox},
        {2,PhaseState.EnemySetBomb},
        {3,PhaseState.PlayerChoiceToOpenBox},
        {4,PhaseState.PlayerMoveToChoiceBox},
        {5,PhaseState.PlayerOpenBox},
        {6,PhaseState.PlayerChoiceToSetBomb},
        {7,PhaseState.PlayerMoveToSetBox},
        {8,PhaseState.PlayerSetBomb},
        {9,PhaseState.EnemyChoiceToOpenBox},
        {10,PhaseState.EnemyMoveToChoiceBox},
        {11,PhaseState.EnemyOpenBox},
    };

    private Dictionary<int, PhaseState> firstPlayerState = new Dictionary<int, PhaseState>
    {
        {0,PhaseState.PlayerChoiceToSetBomb},
        {1,PhaseState.PlayerMoveToSetBox},
        {2,PhaseState.PlayerSetBomb},
        {3,PhaseState.EnemyChoiceToOpenBox},
        {4,PhaseState.EnemyMoveToChoiceBox},
        {5,PhaseState.EnemyOpenBox},
        {6,PhaseState.EnemyChoiceToSetBomb},
        {7,PhaseState.EnemyMoveToSetBox},
        {8,PhaseState.EnemySetBomb},
        {9,PhaseState.PlayerChoiceToOpenBox},
        {10,PhaseState.PlayerMoveToChoiceBox},
        {11,PhaseState.PlayerOpenBox},
    };

    void Start()
    {
        playerPoint = 0;

        enemyPoint = 0;

        playerLife = OptionController.maxLife;

        enemyLife = OptionController.maxLife;

        clickController = FindObjectOfType<ClickController>();

        enemyMoveController = FindObjectOfType<EnemyMoveController>();

        imageController = FindObjectOfType<ImageController>();

        SetFirstPlayerOrder(true);

        // サンプル：現在の順序をデバッグ出力
        foreach (var pair in currentState)
        {
            Debug.Log($"Index: {pair.Key}, State: {pair.Value}");
        }

        // DataManagerから設定されたオブジェクト数を取得
        int numberOfObjects = DataManager.Instance.objectCount;
        Debug.Log($"DataManager.instance.objectCount: {numberOfObjects}");

        if (numberOfObjects <= 0)
        {
            Debug.LogWarning("オブジェクト数が0または負の値です。生成をスキップします。");
            return;
        }

        // 配列と辞書を初期化
        objectArray = new GameObject[numberOfObjects];
        objectNumberMapping = new Dictionary<GameObject, int>();

        // オブジェクト生成
        GenerateObjectsInCircle(numberOfObjects);

        // ターンの決定
        Debug.Log("オブジェクト生成が完了しました。DecideFirstTurnを実行します。");
        DecideFirstTurn();

        choiceTime = 60f;
    }

    private void Update()
    {
        PhaseState currentState = GetCurrentState();

        //Debug.Log(currentIndex);

        //Debug.Log(turnCount);
        //Debug.Log(enemyPoint);
        //Debug.Log(objectArray.Length);
        //Debug.Log("choice"+choiceTime);
        //時間制限で箱をランダムで選択
        if (currentState == PhaseState.PlayerChoiceToSetBomb || currentState == PhaseState.PlayerChoiceToOpenBox)
        {
            //待機時間
            choiceTime -= Time.deltaTime;

            if (choiceTime <= 0f)
            {

                choiceTime = 60f;

                countText.enabled = false;

                NumberRandom();

                //Phaseを確認
                if (currentState == PhaseState.PlayerChoiceToSetBomb)
                {
                    if (randomObject.CompareTag("Cube"))
                    {
                        randomObject.gameObject.tag = "Explosion";

                        Debug.Log($"オブジェクト{randomObject.gameObject.name}のタグを'Explosion'に変更しました。");

                        clickController.targetPosition = randomObject.transform.position;

                        // フラグを有効化
                        clickController.isMoving = true;

                        choiceTime = 60f;

                        StartCoroutine(NextState());

                        // クリックしたオブジェクト以外のコライダーを無効化
                        clickController.DeactivateOtherColliders(randomObject);
                    }
                }
                else if(currentState == PhaseState.PlayerChoiceToOpenBox)
                {
                    Debug.Log("きたぞー");


                    clickController.targetPosition = randomObject.transform.position;

                    // フラグを有効化
                    clickController.isMoving = true;

                    choiceTime = 60f;

                    StartCoroutine(NextState());

                    // クリックしたオブジェクト以外のコライダーを無効化
                    clickController.DeactivateOtherColliders(randomObject);
                }
            }
        }

        playerPointText.text = playerPoint + "点";

        enemyPointText.text = enemyPoint + " 点";

        turnText.text = turnCount +"ターン";

        playerLifeText.text = "Player Life:" + playerLife;

        enemyLifeText.text = "CPU Life:" + enemyLife;

        if(choiceTime <= 30)
        {
            countText.enabled = true;
            countText.text = "" + choiceTime;
        }
    }

    void GenerateObjectsInCircle(int numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // 配置角度を計算
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 position = new Vector3(Mathf.Cos(-angle) * radius, 0, Mathf.Sin(-angle) * radius);

            // オブジェクト生成
            GameObject obj = Instantiate(objectPrefab, position, Quaternion.identity, transform);
            objectArray[i] = obj;

            // 各オブジェクトに一意の番号を割り当て
            objectNumberMapping[obj] = i;

            // オブジェクトの名前に番号を設定
            obj.name = $"Object_{i}";

            //TextMeshProの追加
            GameObject textobj = new GameObject("NumberText");
            //親をオブジェクトに設定
            textobj.transform.SetParent(obj.transform);
            //表示位置調整
            textobj.transform.localPosition = new Vector3(0, 2f, 0);

            TextMeshPro tmp = textobj.AddComponent<TextMeshPro>();
            //番号を+1して表示
            tmp.text = (i + 1).ToString();
            tmp.fontSize = 10;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.red;


            //Debug.Log($"Object created: {obj.name}, Assigned Number: {i}");
        }
        Debug.Log($"Total objects generated: {objectArray.Length}");
    }

    void DecideFirstTurn()
    {
        firstTurn = Random.Range(0, 1);

        if (firstTurn == 0)
        {
            Debug.Log("プレイヤーが先行です");
            SetFirstPlayerOrder(true);
            PlayerTurn();
        }
        else
        {
            Debug.Log("敵が先行です");
            SetFirstPlayerOrder(false);
            EnemyBombSet();
        }
    }

    /// <summary>
    /// 先攻/後攻の順序を切り替える
    /// </summary>
    /// <param name="isFirst">trueならplayerが先攻、falseならplayerが後攻</param>
    public void SetFirstPlayerOrder(bool isFirst)
    {
        currentState = isFirst ? firstPlayerState : firstEnemyState;
    }

    // 現在のstateを取得する
    public PhaseState GetCurrentState()
    {
        return currentState[currentIndex];
    }

    public IEnumerator NextState()
    {
        Debug.Log("state");

        PhaseState currentState = GetCurrentState();
        
        if(currentState == PhaseState.EnemyOpenBox || currentState == PhaseState.PlayerOpenBox)
        {
            Debug.Log("松");
            //yield return new WaitForSeconds(4f);
            Next();
            yield return null;
        }
        else
        {
            Debug.Log("next");
            Next();
            yield return null;
        }
    }

    private void Next()
    {
        // 次のインデックスに進む
        currentIndex++;

        // インデックスが順序の範囲外ならリセット
        if (currentIndex >= currentState.Count)
        {
            currentIndex = 0; // 最初の状態に戻る場合
                              // または、進行終了なら以下のコードにする
                              // Debug.Log("すべての状態が終了しました。");
                              // return;
            turnCount++;
        }

        if ((currentIndex + 1) % 7 == 0)
        {
            turnCount++;
        }

        imageController.imageTrigger = true;

        Debug.Log("違法");

        // 現在の状態をログ出力
        Debug.Log($"今の状態: {currentState[currentIndex]}");
    }

    public void BuckState()
    {
        currentIndex = currentIndex - 2;
        // 現在の状態をログ出力
        Debug.Log($"今の状態: {currentState[currentIndex]}");
    }

    public void NumberRandom()
    {
        // ランダムにオブジェクトを選択
        int randomIndex = Random.Range(0, objectArray.Length);
        randomObject = objectArray[randomIndex];

        Debug.Log($"ランダムで{randomObject.name}を抽選");
    }

    public void EnemyBombSet()
    {
        Debug.Log("BombSite");

        // プレイヤーオブジェクトを非アクティブ化
        if (playerObject != null)
        {
            enemyObject.SetActive(true);

            playerObject.SetActive(false);
            Debug.Log("Player object deactivated.");
        }

        if (objectArray == null || objectArray.Length == 0)
        {
            Debug.LogWarning("objectArray が空です！");
            return;
        }

        if (turnCount < OptionController.maxTurn)
        {
            NumberRandom();

            enemyMoveController.enemyTarget = randomObject.transform.position;

            enemyMoveController.enemyMoving = true;

            // ランダムに選ばれたオブジェクトの情報を表示
            //Debug.Log($"ランダムに選ばれたオブジェクト: {randomObject.name}, 割り当て番号: {assignedNumber}");
        }
    }

    public void EnemyBoxChoice()
    {
        playerObject.SetActive(false);

        enemyObject.SetActive(true);

        //Enemyがboxを選択する
        NumberRandom();

        StartCoroutine(NextState());

        enemyMoveController.enemyTarget = randomObject.transform.position;

        enemyMoveController.enemyMoving = true;
    }

   public void PlayerTurn()
    {
        Debug.Log("hiukuhjguigui");

        playerObject.SetActive(true);

        enemyObject.SetActive(false);
    }
}
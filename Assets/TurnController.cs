using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq.Expressions;

public class TurnController : MonoBehaviour
{
    private int firstTurn;

    private ClickController clickController;

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

    [SerializeField] private TextMeshProUGUI lifeText;

    [SerializeField] private TextMeshProUGUI pointText;

    public int playerLife = 0;

    public int enemyLife = 0;

    public int playerPoint = 0;

    public int enemyPoint = 0;

    private bool playerTurn = false;

    private bool enemyTurn = false;

    public bool choiceTrigger = false;

    public float choiceTime = 0f;
    void Start()
    {
        turnCount = 0f;

        playerLife = OptionController.maxLife;

        enemyLife = OptionController.maxLife;

        clickController = FindObjectOfType<ClickController>();

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
    }

    private void Update()
    {
        //Debug.Log(objectArray.Length);
        Debug.Log("choice"+choiceTime);
        //時間制限で箱をランダムで選択
        if (choiceTrigger == true)
        {
            //待機時間
            choiceTime += Time.deltaTime;

            if (choiceTime >= 8f)
            {

                choiceTime = 0f;

                choiceTrigger = false;

                NumberRandom();

                //turnCountが整数か判別
                if ((int)turnCount == turnCount)
                {
                    if (randomObject.CompareTag("Cube"))
                    {
                        randomObject.gameObject.tag = "Explosion";

                        Debug.Log($"オブジェクト{randomObject.gameObject.name}のタグを'Explosion'に変更しました。");

                        turnCount += 0.5f;

                        EnemyBoxChoice();
                    }
                }
                else
                {
                    Debug.Log("きたぞー");


                    clickController.targetPosition = randomObject.transform.position;

                    // フラグを有効化
                    clickController.isMoving = true;
                }
            }
        }

        pointText.text = playerPoint + "点";

        turnText.text = turnCount / 2 + "ターン";
        if (playerTurn == true)
        {
            lifeText.text = "Player Life:" + playerLife;
        }
        else if (enemyTurn == true)
        {
            lifeText.text = "CPU Life:" + enemyLife;
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
        firstTurn = Random.Range(0, 2);

        if (firstTurn == 0)
        {
            Debug.Log("プレイヤーが先行です");
            StartPlayerTurn();
        }
        else
        {
            Debug.Log("敵が先行です");
            StartEnemyTurn();
        }
    }

    void StartPlayerTurn()
    {
        Debug.Log("Player turn started.");

        PlayerTurn();
    }

    void StartEnemyTurn()
    {
        Debug.Log("Enemy turn started.");
        if (objectArray == null || objectArray.Length == 0)
        {
            Debug.LogWarning("EnemyBombSiteを実行する前に配列が空です。生成が完了しているか確認してください。");
            return;
        }

        EnemyBombSite();
    }

    public void NumberRandom()
    {
        // ランダムにオブジェクトを選択
        int randomIndex = Random.Range(0, objectArray.Length);
        randomObject = objectArray[randomIndex];

        Debug.Log($"ランダムで{randomObject.name}を抽選");
    }

    void EnemyBombSite()
    {
        enemyTurn = true;

        playerTurn = false;

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

            // 対応する番号を辞書から取得
            //int assignedNumber = objectNumberMapping[randomObject];

            // ランダムに選ばれたオブジェクトの情報を表示
            //Debug.Log($"ランダムに選ばれたオブジェクト: {randomObject.name}, 割り当て番号: {assignedNumber}");

            // 選ばれたオブジェクトのタグを変更
            randomObject.tag = "Explosion";
            Debug.Log($"Enemyがオブジェクト {randomObject.name} のタグを 'Explosion' に変更しました。");
        }

        if (turnCount < OptionController.maxTurn)
        {
            //ターンを進める
            turnCount += 0.5f;
        }
        PlayerTurn();
    }

    public void EnemyBoxChoice()
    {
        enemyTurn = true;

        playerTurn = false;

        playerObject.SetActive(false);

        enemyObject.SetActive(true);

        //Enemyがboxを選択する
        NumberRandom();

        if (randomObject.tag == "Cube")
        {
            Debug.Log("Enemyがcubeを触った");

            enemyPoint += objectNumberMapping[randomObject] + 1;

            randomObject.SetActive(false);

            // 配列から削除
            //配列からlistに変え配列の要素数を削除できるようにする
            List<GameObject> tempList = new List<GameObject>(objectArray);
            tempList.Remove(randomObject);
            objectArray = tempList.ToArray();

            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Explosion");

            foreach (GameObject obj in objectsWithTag)
            {
                // タグを新しいタグに変更
                obj.tag = "Cube";
            }

            Debug.Log(enemyPoint);

            if (turnCount < OptionController.maxTurn)
            {
                EnemyBombSite();
            }

            if (turnCount < OptionController.maxTurn)
            {
                //ターンを進める
                turnCount += 0.5f;
            }

            if (tempList.Contains(this.gameObject))
            {
                tempList.Remove(this.gameObject);  // リストから削除
                objectArray = tempList.ToArray();  // 配列に戻す

                //cubeだった箱を消す
                this.gameObject.SetActive(false);

                Debug.Log($"{this.gameObject.name} を配列から削除しました。");
            }
        }
        else if (randomObject.tag == "Explosion")
        {
            if (turnCount < OptionController.maxTurn)
            {
                //ターンを進める
                turnCount += 0.5f;
            }

            enemyLife -= 1;

            enemyPoint = 0;

            randomObject.tag = "Cube";


            Debug.Log("EnemyがExplosionを触った");

            if (turnCount < OptionController.maxTurn)
            {
                EnemyBombSite();
            }
        }
    }

    void PlayerTurn()
    {
        Debug.Log("hiukuhjguigui");

        choiceTrigger = true;

        playerTurn = true;

        enemyTurn = false;

        playerObject.SetActive(true);

        enemyObject.SetActive(false);
    }
}
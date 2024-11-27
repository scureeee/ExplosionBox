using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnController : MonoBehaviour
{
    private int firstTurn;

    public float turnCount = 0f;

    // 生成するオブジェクトのPrefab
    public GameObject objectPrefab;

    // 円の半径
    public float radius = 10f;

    // 生成したオブジェクトの配列
    private GameObject[] objectArray;

    // 各オブジェクトの一意の番号を格納する辞書
    private Dictionary<GameObject, int> objectNumberMapping;

    public GameObject playerObject;

    public GameObject enemyObject;

    public GameObject randomObject;

    void Start()
    {
        turnCount = 0f;

        // DataManagerから設定されたオブジェクト数を取得
        int numberOfObjects = DataManager.instance.objectCount;
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

    void GenerateObjectsInCircle(int numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // 配置角度を計算
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

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
            tmp.alignment =TextAlignmentOptions.Center;
            tmp.color = Color.red;


            Debug.Log($"Object created: {obj.name}, Assigned Number: {i}");
        }
        Debug.Log($"Total objects generated: {objectArray.Length}");
    }

    void DecideFirstTurn()
    {
        firstTurn = Random.Range(2, 3);

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

    void PlayerBombSite()
    {
        enemyObject.SetActive(false);
    }

    public void NumberRandom()
    {
        // ランダムにオブジェクトを選択
        int randomIndex = Random.Range(0, objectArray.Length);
        randomObject = objectArray[randomIndex];
    }

    void EnemyBombSite()
    {
        Debug.Log("BombSite");

        // プレイヤーオブジェクトを非アクティブ化
        if (playerObject != null)
        {
            playerObject.SetActive(false);
            Debug.Log("Player object deactivated.");
        }

        if (objectArray == null || objectArray.Length == 0)
        {
            Debug.LogWarning("objectArray が空です！");
            return;
        }

        NumberRandom();

        // 対応する番号を辞書から取得
        int assignedNumber = objectNumberMapping[randomObject];

        // ランダムに選ばれたオブジェクトの情報を表示
        Debug.Log($"ランダムに選ばれたオブジェクト: {randomObject.name}, 割り当て番号: {assignedNumber}");

        // 選ばれたオブジェクトのタグを変更
        randomObject.tag = "Explosion";
        Debug.Log($"オブジェクト {randomObject.name} のタグを 'Explosion' に変更しました。");

        //相手のターン移る
        turnCount = turnCount + 0.5f;

        Debug.Log(turnCount);

        PlayerTurn();
    }

    public void EnemyBoxChoice()
    {
        playerObject.SetActive(false );

        enemyObject.SetActive(true);

        //Enemyがboxを選択する
        NumberRandom();

        if(randomObject.tag == "Cube")
        {

        }else if(randomObject.tag == "Explosion")
        {

        }
    }

    void PlayerTurn()
    {
        playerObject.SetActive(true);

        enemyObject.SetActive(false);
    }
}
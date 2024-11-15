using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    // 0がプレイヤー、1が敵として設定する例
    private int firstTurn;

    //生成するオブジェクトのprefab
    public GameObject objectPrefab;

    //円の半径
    public float radius = 5f;

    //OptionSceneで設定した数の配列
    private GameObject[] objectArray;

    // Start is called before the first frame update
    void Start()
    {
        //先行と後攻決める
        DecideFirstTurn();

        //DataManagerから設定されたオブジェクト数を取得
        int numberOfObjects = DataManager.instance.objectCount;

        //オブジェクト数に基づいて配列を作成
        objectArray = new GameObject[numberOfObjects];
        GenerateObjectsInCircle(numberOfObjects);
    }

    void GenerateObjectsInCircle(int numberOfObjects)
    {
        for(int i = 0; i < numberOfObjects; i++)
        {
            float angle = i *Mathf.PI * 2 /numberOfObjects;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

            //オブジェクト生成と配列への格納
            GameObject obj = Instantiate(objectPrefab, position, Quaternion.identity, transform);
            objectArray[i] = obj;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void DecideFirstTurn()
    {
        // 0または1のランダムな値を取得
        firstTurn = Random.Range(0, 2);

        if (firstTurn == 0)
        {
            Debug.Log("プレイヤーが先行です");
            // プレイヤーのターン開始処理
            StartPlayerTurn();
        }
        else
        {
            Debug.Log("敵が先行です");
            // 敵のターン開始処理
            StartEnemyTurn();
        }
    }

    void StartPlayerTurn()
    {
        EnemyBombSite();
    }

    void StartEnemyTurn()
    {

    }

    void EnemyBombSite()
    {

    }
}

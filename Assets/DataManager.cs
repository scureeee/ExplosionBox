using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }

    //シーン間で共有するオブジェクト数を保持する変数
    public int objectCount;
    // Start is called before the first frame update

    private void Awake()
    {
        //シングルトンパターンでインスタンスを一つだけに制限
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

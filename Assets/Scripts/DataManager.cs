using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    //シーン間で共有するオブジェクト数を保持する変数
    public int objectCount;
    // Start is called before the first frame update

    [SerializeField] private string[] destroyInScenes;

    private void Awake()
    {
        //シングルトンパターンでインスタンスを一つだけに制限
        if (Instance == null)
        {
            Instance = this;
            //シーンをまたいでも破棄されない
            DontDestroyOnLoad(gameObject);

            // シーンがロードされた時のイベントを登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // イベント登録解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 現在のシーン名が `destroyInScenes` に含まれている場合、オブジェクトを削除
        foreach (var sceneName in destroyInScenes)
        {
            if (scene.name == "ResultScene")
            {
                Destroy(gameObject);
                Instance = null;
                break;
            }
        }
    }
}

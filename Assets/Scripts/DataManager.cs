using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム中の一部データ（objectCountなど）をシーンを跨いで保持するためのシングルトンクラス。
/// 一部のシーン（ResultSceneなど）に遷移した場合には自動的に破棄される。
/// </summary>
public class DataManager : MonoBehaviour
{
    //インスタンスへのアクセス用プロパティ
    public static DataManager Instance { get; private set; }
    
    // ゲーム中に記録するオブジェクト数などのデータ
    public int objectCount;

    // このシーン名がロードされた場合は、このオブジェクトを破棄する対象とする
    [SerializeField] private string[] destroyInScenes;

    /// <summary>
    /// ゲーム開始時（またはシーン遷移で再生成されたとき）に呼ばれる。
    /// Singletonパターンの管理と、シーンロードイベント登録を行う。
    /// </summary>
    private void Awake()
    {
        // まだInstanceが設定されていない場合はこのオブジェクトを唯一のインスタンスとして確定
        if (!Instance)
        {
            Instance = this;
            
            // シーン遷移してもこのオブジェクトを破棄しないように設定
            DontDestroyOnLoad(gameObject);
            
            // シーンロード時のイベントを登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// オブジェクトが破棄されるときに呼ばれる。
    /// イベント登録を解除してメモリリークを防ぐ。
    /// </summary>
    private void OnDestroy()
    {
        // すでにInstanceがある場合は、重複防止のため自身を破棄
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 新しいシーンがロードされたときに呼ばれるイベントハンドラ。
    /// 特定のシーン名（destroyInScenesに含まれる）であれば、このDataManagerを破棄する。
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 現在ロードされたシーン名が、破棄対象のシーン名に含まれるかを判定
        if (destroyInScenes.All(_ => scene.name != "ResultScene")) return;
        
        // 対象のシーンであれば、DataManagerのインスタンスを破棄しnullに戻す
        Destroy(gameObject);
        Instance = null;
    }
}

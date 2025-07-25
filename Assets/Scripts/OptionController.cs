using UnityEngine;


namespace optionSpace
{
    /// <summary>
    /// ゲームの設定やオプション（ターン数、ライフ数、ポイントなど）を管理するクラス。
    /// シーン内に1つだけ存在し、他のスクリプトからゲーム設定を参照・使用できるようにする。
    /// </summary>
    public class OptionController : MonoBehaviour
    {
        // インスタンス（シングルトンパターン）
        public static OptionController Instance { get; private set; }

        // ゲーム開始時に生成するオブジェクト数
        private int objectCountToSet;

        // ゲーム内の最大ターン数
        public static int maxTurn;

        // プレイヤーの最大ライフ
        public static int maxLife;

        // 最大ポイント数（静的）
        public static int maxPoint;

        // 爆弾設置や開封の選択時間（秒）
        public float choiceTime = 60f;

        // 箱を開けるまでの時間（秒）
        public float openTime = 60f;

        // 次のフェーズへ進む入力が可能かどうか
        public bool clickNext;

        // タイマーをキャンセルするかどうか
        public bool canselTime;

        //キャッシュ
        private DataManager dataManager;

        private void Awake()
        {
            //キャッシュ
            dataManager = DataManager.Instance;
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            // すでにインスタンスが存在しない場合はこのオブジェクトを使う
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                // 既に存在する場合は破棄（シングルトンとして機能させるため）
                Destroy(gameObject);
            }

            maxPoint = 18; // 最大スコア（勝利条件などに使用）
            maxTurn = 10; // 最大ターン数
            maxLife = 2; // 最大ライフ数（2回までミス可能）
            objectCountToSet = 8; // 初期に設置されるオブジェクト数
        }

        // Update is called once per frame
        private void Update()
        {
            // DataManagerのobjectCountを最新の設定に反映させる
            DataManager.Instance.objectCount = objectCountToSet;
        }
    }
}
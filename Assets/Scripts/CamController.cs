using optionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TurnController;

public class CamController : MonoBehaviour
{
    private TurnController turnController;

    // カメラ関連

    private CollisionController collisionController;

    private OptionController optionController;

    // メインカメラをアサインする
    public Camera mainCamera;

    // カメラが動いているかのフラグ
    public bool isCameraMoving = false;

    // カメラの初期位置
    public Vector3 cameraStartPosition;

    // カメラが近づくターゲット
    public Transform targetObject;

    // カメラの移動速度
    public float cameraMoveSpeed = 2f;

    //フェードイン・フェードアウト

    //フェードイン・アウト用のパネル
    public GameObject panelFade;

    //パネルのimageの取得変数
    Image fadeAlpha;

    //パネルのalpha値の取得変数
    private float alpha;

    private bool fadeInTrigger;

    // Start is called before the first frame update
    void Start()
    {
        collisionController = FindObjectOfType<CollisionController>();

        turnController = FindObjectOfType<TurnController>();

        optionController = FindObjectOfType<OptionController>();

        // 初期カメラ位置を記録
        cameraStartPosition = mainCamera.transform.position;

        //パネルのイメージ取得
        fadeAlpha = panelFade.GetComponent<Image>();

        //パネルのalpha値
        alpha = fadeAlpha.color.a;

        fadeInTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 現在のstateを取得
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        if (currentState == PhaseState.PlayerSetBomb || currentState == PhaseState.EnemyChoiceToSetBomb)
        {
            FadeOut();
        }

        if(fadeInTrigger == true || currentState == PhaseState.EnemySetBomb)
        {
            FadeIn();
        }
    }

    public void MotionAids()
    {
        // 現在のstateを取得
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        Debug.Log("遠く");
        StartCoroutine(CameraBack());

        if(currentState == PhaseState.EnemyOpenBox || currentState == PhaseState.PlayerOpenBox)
        {
            if(collisionController.cameraBuck == true)
            {
                StartCoroutine(turnController.NextState());
            }
        }
    }

    public IEnumerator CameraBack()
    {
        if(optionController.canselTime == false)
        {
            yield return new WaitForSeconds(5f);
        }
        else if(optionController.canselTime == true)
        {
            optionController.canselTime = false;
            yield return new WaitForSeconds(2f);
        }
        mainCamera.transform.position = cameraStartPosition;

        collisionController.cameraBuck = true;
    }

    public void FadeIn()
    {
        // 現在のstateを取得
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        alpha -= 0.01f;

        fadeAlpha.color = new Color(0,0,0,alpha);

        if(alpha <= 0)
        {
            fadeInTrigger = false;

            Debug.Log("明るく");

            if (currentState == PhaseState.EnemyChoiceToOpenBox)
            {
                Debug.Log(",dak,lda,l");

                turnController.EnemyBoxChoice();
            }
            else if(currentState == PhaseState.EnemySetBomb)
            {
                StartCoroutine(turnController.NextState());
            }
        }
    }

    public void FadeOut()
    {
        // 現在のstateを取得
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        alpha += 1f;

        fadeAlpha.color = new Color(0,0,0,alpha);

        if(alpha >= 1)
        {
            if(currentState == PhaseState.PlayerSetBomb)
            {
                StartCoroutine(turnController.NextState());

                fadeInTrigger = true;

            }
            else if (currentState == PhaseState.EnemyChoiceToSetBomb)
            {
                Debug.Log("enemy暗く");

                StartCoroutine(turnController.NextState());

                turnController.EnemyBombSet();
            }
        }
    }
}

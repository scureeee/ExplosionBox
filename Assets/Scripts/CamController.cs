using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TurnController;

public class CamController : MonoBehaviour
{
    private TurnController turnController;

    // �J�����֘A

    // ���C���J�������A�T�C������
    public Camera mainCamera;

    // �J�����������Ă��邩�̃t���O
    public bool isCameraMoving = false;

    // �J�����̏����ʒu
    public Vector3 cameraStartPosition;

    // �J�������߂Â��^�[�Q�b�g
    public Transform targetObject;

    // �J�����̈ړ����x
    public float cameraMoveSpeed = 2f;

    //�t�F�[�h�C���E�t�F�[�h�A�E�g

    //�t�F�[�h�C���E�A�E�g�p�̃p�l��
    public GameObject panelFade;

    //�p�l����image�̎擾�ϐ�
    Image fadeAlpha;

    //�p�l����alpha�l�̎擾�ϐ�
    private float alpha;

    private bool fadeInTrigger;

    // Start is called before the first frame update
    void Start()
    {
        turnController = FindObjectOfType<TurnController>();

        // �����J�����ʒu���L�^
        cameraStartPosition = mainCamera.transform.position;

        //�p�l���̃C���[�W�擾
        fadeAlpha = panelFade.GetComponent<Image>();

        //�p�l����alpha�l
        alpha = fadeAlpha.color.a;

        fadeInTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        // ���݂�state���擾
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
        // ���݂�state���擾
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        Debug.Log("����");
        StartCoroutine(CameraBack());

        if(currentState == PhaseState.EnemyOpenBox || currentState == PhaseState.PlayerOpenBox)
        {
            turnController.Next();
        }
    }

    public IEnumerator CameraBack()
    {
        yield return new WaitForSeconds(2f);
        mainCamera.transform.position = cameraStartPosition;
    }

    public void FadeIn()
    {
        // ���݂�state���擾
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        alpha -= 0.01f;

        fadeAlpha.color = new Color(0,0,0,alpha);

        if(alpha <= 0)
        {
            fadeInTrigger = false;

            Debug.Log("���邭");

            if (currentState == PhaseState.EnemyChoiceToOpenBox)
            {
                Debug.Log(",dak,lda,l");

                turnController.EnemyBoxChoice();
            }
            else if(currentState == PhaseState.EnemySetBomb)
            {
                turnController.Next();
            }
        }
    }

    public void FadeOut()
    {
        // ���݂�state���擾
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        alpha += 0.01f;

        fadeAlpha.color = new Color(0,0,0,alpha);

        if(alpha >= 1)
        {
            if(currentState == PhaseState.PlayerSetBomb)
            {
                turnController.Next();

                fadeInTrigger = true;

            }
            else if (currentState == PhaseState.EnemyChoiceToSetBomb)
            {
                Debug.Log("enemy�Â�");

                turnController.Next();

                turnController.EnemyBombSet();
            }
        }
    }
}

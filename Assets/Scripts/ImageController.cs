using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TurnController;
using ImageSpace;

public class ImageController : MonoBehaviour
{
    public Image targetImage; // �ύX���� Image �R���|�[�l���g
    public Sprite playerSet;
    public Sprite playerOpen;
    public Sprite enemySet;
    public Sprite enemyOpen;

    // �F��ς���܂ł̒x�����ԁi�b�j
    private float delayTime = 1f;

    public GameObject explosion;

    public GameObject safe;


    public bool imageTrigger;

    TurnController turnController;

    ConstanceContrller constanceContrller;

    void Start()
    {
        imageTrigger = true;
        turnController = FindObjectOfType<TurnController>();
        constanceContrller = FindObjectOfType<ConstanceContrller>();
    }

    void Update()
    {
        // ���݂�state���擾
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        if (currentState == PhaseState.PlayerChoiceToSetBomb && imageTrigger)
        {
            targetImage.sprite = playerSet;
            targetImage.color = new Color(1f, 1f, 1f, 1f);

            // �w�肵�����Ԍ�ɓ����x��ύX
            StartCoroutine(ChangeColorAfterDelay(delayTime));
        }
        else if (currentState == PhaseState.PlayerChoiceToOpenBox && imageTrigger)
        {
            targetImage.sprite = playerOpen;
            targetImage.color = new Color(1f, 1f, 1f, 1f);

            // �w�肵�����Ԍ�ɓ����x��ύX
            StartCoroutine(ChangeColorAfterDelay(delayTime));
        }
        else if (currentState == PhaseState.EnemyChoiceToSetBomb && imageTrigger)
        {
            targetImage.sprite = enemySet;
            targetImage.color = new Color(1f, 1f, 1f, 1f);

            // �w�肵�����Ԍ�ɓ����x��ύX
            StartCoroutine(ChangeColorAfterDelay(delayTime));
        }
        else if (currentState == PhaseState.EnemyChoiceToOpenBox && imageTrigger)
        {
            targetImage.sprite = enemyOpen;
            targetImage.color = new Color(1f, 1f, 1f, 1f);

            // �w�肵�����Ԍ�ɓ����x��ύX
            StartCoroutine(ChangeColorAfterDelay(delayTime));
        }
    }


    // Coroutine���g���Ďw�莞�Ԍ�ɉ摜�̐F��ς���
    IEnumerator ChangeColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        imageTrigger = false;

        // 2�b��ɉ摜�̓����x��ύX
        targetImage.color = new Color(1f, 1f, 1f, 0f); // �������ɕύX
    }

    public IEnumerator ExplosionSwitch()
    {
        explosion.SetActive(true);
        yield return new WaitForSeconds(constanceContrller.waitTime);
        explosion.SetActive(false);
    }

    public void Safe()
    {
        StartCoroutine(SafeSwitch());
    }
    private IEnumerator SafeSwitch()
    {
        safe.SetActive(true);
        yield return new WaitForSeconds(2f);
        safe.SetActive(false);
    }
}
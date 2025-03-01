using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TurnController;

public class ImageController : MonoBehaviour
{
    public Image targetImage; // �ύX���� Image �R���|�[�l���g
    public Sprite playerSet;
    public Sprite playerOpen;
    public Sprite enemySet;
    public Sprite enemyOpen;

    public GameObject explosion;

    public GameObject safe;

    public AudioClip safeSound;

    public bool imageTrigger;

    TurnController turnController;

    CollisionController collisionController;

    void Start()
    {
        imageTrigger = true;
        turnController = FindObjectOfType<TurnController>();
        collisionController = FindObjectOfType<CollisionController>();
    }

    void Update()
    {
        // ���݂�state���擾
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        if (currentState == PhaseState.PlayerChoiceToSetBomb && imageTrigger)
        {
            targetImage.sprite = playerSet;
            targetImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else if(currentState == PhaseState.PlayerChoiceToOpenBox && imageTrigger)
        {
            targetImage.sprite = playerOpen;
            targetImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else if(currentState == PhaseState.EnemyChoiceToSetBomb && imageTrigger)
        {
            targetImage.sprite = enemySet;
            targetImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else if(currentState == PhaseState.EnemyChoiceToOpenBox && imageTrigger)
        {
            targetImage.sprite = enemyOpen;
            targetImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public IEnumerator ExplosionSwitch()
    {
        explosion.SetActive(true);
        yield return new WaitForSeconds(5f);
        explosion.SetActive(false);
    }

    public void Safe()
    {
        StartCoroutine(SafeSwitch());
    }
    private IEnumerator SafeSwitch()
    {
        GetComponent<AudioSource>().PlayOneShot(safeSound);
        safe.SetActive(true);
        yield return new WaitForSeconds(5f);
        safe.SetActive(false);
    }
}
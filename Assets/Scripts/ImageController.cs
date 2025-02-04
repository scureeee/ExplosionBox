using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TurnController;

public class ImageController : MonoBehaviour
{
    public Image targetImage; // 変更する Image コンポーネント
    public Sprite playerSet;
    public Sprite playerOpen;
    public Sprite enemySet;
    public Sprite enemyOpen;

    private float delayTime = 1f; // 色を変えるまでの遅延時間（秒）

    public GameObject explosion;

    public GameObject safe;


    public bool imageTrigger;

    TurnController turnController;

    void Start()
    {
        imageTrigger = true;
        turnController = FindObjectOfType<TurnController>();
    }

    void Update()
    {
        // 現在のstateを取得
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        if (currentState == PhaseState.PlayerChoiceToSetBomb && imageTrigger)
        {
            targetImage.sprite = playerSet;
            targetImage.color = new Color(1f, 1f, 1f, 1f);

            // 指定した時間後に透明度を変更
            StartCoroutine(ChangeColorAfterDelay(delayTime));
        }
        else if(currentState == PhaseState.PlayerChoiceToOpenBox && imageTrigger)
        {
            targetImage.sprite = playerOpen;
            targetImage.color = new Color(1f, 1f, 1f, 1f);

            // 指定した時間後に透明度を変更
            StartCoroutine(ChangeColorAfterDelay(delayTime));
        }
        else if(currentState == PhaseState.EnemyChoiceToSetBomb && imageTrigger)
        {
            targetImage.sprite = enemySet;
            targetImage.color = new Color(1f, 1f, 1f, 1f);

            // 指定した時間後に透明度を変更
            StartCoroutine(ChangeColorAfterDelay(delayTime));
        }
        else if(currentState == PhaseState.EnemyChoiceToOpenBox && imageTrigger)
        {
            targetImage.sprite = enemyOpen;
            targetImage.color = new Color(1f, 1f, 1f, 1f);

            // 指定した時間後に透明度を変更
            StartCoroutine(ChangeColorAfterDelay(delayTime));
        }
    }


    // Coroutineを使って指定時間後に画像の色を変える
    IEnumerator ChangeColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        imageTrigger = false;

        // 2秒後に画像の透明度を変更
        targetImage.color = new Color(1f, 1f, 1f, 0f); // 半透明に変更
    }

    public IEnumerator ExplosionSwitch()
    {
        explosion.SetActive(true);
        yield return new WaitForSeconds(2f);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using optionSpace;

public class ResultJudge : MonoBehaviour
{

    private TurnController turnController;

    public GameObject lastTurn;

    private bool lastTrigger = true;

    // Start is called before the first frame update

    void Start()
    {
        turnController = FindObjectOfType<TurnController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnController.playerLife == 0)
        {
            ResultText.resultNumber = 1;
            Debug.Log("Life‚È‚µ”s–k‚Å‚·");
            SceneManager.LoadScene("ResultScene");
        }

        if (TurnController.enemyLife == 0)
        {
            ResultText.resultNumber = 2;
            Debug.Log("Life‚È‚µŸ—˜‚Å‚·");
            SceneManager.LoadScene("ResultScene");
        }

        if(TurnController.enemyPoint >= OptionController.maxPoint)
        {
            ResultText.resultNumber = 3;
            Debug.Log("Point You LOSE");
            SceneManager.LoadScene("ResultScene");
        }

        if(TurnController.playerPoint >= OptionController.maxPoint)
        {
            ResultText.resultNumber = 4;
            Debug.Log("Point You WIN");
            SceneManager.LoadScene("ResultScene");
        }

        if(turnController.objectArray.Length == 1)
        {
            if (TurnController.playerPoint < TurnController.enemyPoint)
            {
                ResultText.resultNumber = 5;
                Debug.Log("Turn You LOSE");
                SceneManager.LoadScene("ResultScene");
            }

            else if (TurnController.playerPoint > TurnController.enemyPoint)
            {
                ResultText.resultNumber = 6;
                Debug.Log("Turn You WIN");
                SceneManager.LoadScene("ResultScene");
            }

            else if (TurnController.playerPoint == TurnController.enemyPoint)
            {
                ResultText.resultNumber = 7;
                Debug.Log("Draw");
                SceneManager.LoadScene("ResultScene");
            }
        }

        if (turnController.turnCount == OptionController.maxTurn)
        {

            if (TurnController.playerPoint < TurnController.enemyPoint)
            {
                ResultText.resultNumber = 5;
                Debug.Log("Turn You LOSE");
                SceneManager.LoadScene("ResultScene");
            }

            else if (TurnController.playerPoint > TurnController.enemyPoint)
            {
                ResultText.resultNumber = 6;
                Debug.Log("Turn You WIN");
                SceneManager.LoadScene("ResultScene");
            }

            else if (TurnController.playerPoint == TurnController.enemyPoint)
            {
                ResultText.resultNumber = 7;
                Debug.Log("Draw");
                SceneManager.LoadScene("ResultScene");

            }
        }

        if(lastTrigger == true)
        {
            if(turnController.turnCount == OptionController.maxTurn - 1)
            {
                lastTrigger = false;

                StartCoroutine(Last());
            }
        }

    }

    IEnumerator Last()
    {
        lastTurn.SetActive(true);

        yield return new WaitForSeconds(2f);

        lastTurn.SetActive(false);
    }
} 
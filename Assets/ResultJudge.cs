using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultJudge : MonoBehaviour
{

    private TurnController turnController;

    // Start is called before the first frame update

    void Start()

    {

        turnController = FindObjectOfType<TurnController>();

    }

    // Update is called once per frame

    void Update()
    {

        if (turnController.playerLife == 0)

        {
            ResultText.resultNumber = 1;
            Debug.Log("îsñkÇ≈Ç∑");
            SceneManager.LoadScene("ResultScene");
        }

        if (turnController.enemyLife == 0)

        {
            ResultText.resultNumber = 2;
            Debug.Log("èüóòÇ≈Ç∑");
            SceneManager.LoadScene("ResultScene");
        }
        if(turnController.objectArray.Length == 1)
        {
            if (turnController.playerPoint < turnController.enemyPoint)

            {
                ResultText.resultNumber = 1;
                Debug.Log("You LOSE");
                SceneManager.LoadScene("ResultScene");
            }

            else if (turnController.playerPoint > turnController.enemyPoint)

            {
                ResultText.resultNumber = 2;
                Debug.Log("You WIN");
                SceneManager.LoadScene("ResultScene");
            }

            else if (turnController.playerPoint == turnController.enemyPoint)
            {
                ResultText.resultNumber = 3;
                Debug.Log("Draw");
                SceneManager.LoadScene("ResultScene");
            }
        }

        if (turnController.turnCount == OptionController.maxTurn)
        {

            if (turnController.playerPoint < turnController.enemyPoint)
            {
                ResultText.resultNumber = 1;
                Debug.Log("You LOSE");
                SceneManager.LoadScene("ResultScene");
            }

            else if (turnController.playerPoint > turnController.enemyPoint)
            {
                ResultText.resultNumber = 2;
                Debug.Log("You WIN");
                SceneManager.LoadScene("ResultScene");
            }

            else if (turnController.playerPoint == turnController.enemyPoint)
            {
                ResultText.resultNumber = 3;
                Debug.Log("Draw");
                SceneManager.LoadScene("ResultScene");

            }

        }

    }
} 
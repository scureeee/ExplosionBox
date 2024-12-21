using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            Debug.Log("îsñkÇ≈Ç∑");

        }

        if (turnController.enemyLife == 0)

        {

            Debug.Log("èüóòÇ≈Ç∑");

        }

        if (turnController.turnCount == OptionController.maxTurn)

        {

            if (turnController.playerPoint < turnController.enemyPoint)

            {

                Debug.Log("You LOSE");

            }

            else if (turnController.playerPoint > turnController.enemyPoint)

            {

                Debug.Log("You WIN");

            }

            else if (turnController.playerPoint == turnController.enemyPoint)

            {

                Debug.Log("Draw");

            }

        }

    }

}



 
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultText : MonoBehaviour
{
    public static int resultNumber = 0;

    [SerializeField] GameObject lifeWin;

    [SerializeField] GameObject lifeLose;

    [SerializeField] GameObject pointWin;

    [SerializeField] GameObject pointLose;

    [SerializeField] GameObject turnWin;

    [SerializeField] GameObject turnLose;

    [SerializeField] GameObject draw;

    [SerializeField] TextMeshProUGUI playerLifeText;

    [SerializeField] TextMeshProUGUI enemyLifeText;

    [SerializeField] TextMeshProUGUI playerPointText;

    [SerializeField] TextMeshProUGUI enemyPointText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(resultNumber == 1)
        {
            lifeLose.SetActive(true);
        }
        else if(resultNumber == 2)
        {
            lifeWin.SetActive(true);
        }
        else if( resultNumber == 3)
        {
            pointLose.SetActive(true);
        }
        else if (resultNumber == 4)
        {
            pointWin.SetActive(true);
        }
        else if(resultNumber == 5)
        {
            turnLose.SetActive(true);
        }
        else if(resultNumber == 6)
        {
            turnWin.SetActive(true);
        }
        else if(resultNumber == 7)
        {
            draw.SetActive(true);
        }

        playerLifeText.text = ""  + TurnController.playerLife;

        enemyLifeText.text = "" + TurnController.enemyLife;

        playerPointText.text = "" + TurnController.playerPoint;

        enemyPointText.text = "" + TurnController.enemyPoint;
    }

    public void BuckTitle()
    {
        SceneManager.LoadScene("OptionScene");
    }
}

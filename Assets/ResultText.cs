using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultText : MonoBehaviour
{
    public static int resultNumber = 0;

    [SerializeField] private TextMeshProUGUI resultText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(resultNumber == 1)
        {
            resultText.text = "You Lose";
        }
        else if(resultNumber == 2)
        {
            resultText.text = "You Win";
        }
        else if( resultNumber == 3)
        {
            resultText.text = "Draw";
        }
    }
}

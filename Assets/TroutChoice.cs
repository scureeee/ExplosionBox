using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroutChoice : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown boxChoice;

    [SerializeField] private TMP_Dropdown turnChoice;

    private OptionController optionController;


    private void Start()
    {
        optionController = FindObjectOfType<OptionController>();
    }
    // オプションが変更されたときに実行するメソッド
    void Update()
    {
        //DropdownのValueが0のとき
        if (boxChoice.value == 0)
        {
            optionController.objectCountToSet = 8;
            //Debug.Log($"{optionController.objectCountToSet}個");
        }
        //DropdownのValueが1のとき
        else if (boxChoice.value == 1)
        {
            optionController.objectCountToSet = 6;
            //Debug.Log($"{optionController.objectCountToSet}個");
        }
        //DropdownのValueが2のとき
        else if (boxChoice.value == 2)
        {
            optionController.objectCountToSet = 4;
            //Debug.Log($"{optionController.objectCountToSet}個");
        }

        if (turnChoice.value == 0)
        {
            OptionController.maxTurn = 2;
        }
        else if(turnChoice.value == 1)
        {
            OptionController.maxTurn = 4;
        }
        else if(turnChoice.value == 2)
        {
            OptionController.maxTurn = 6;
        }
    }
}

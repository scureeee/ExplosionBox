using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroutChoice : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private OptionController optionController;


    private void Start()
    {
        optionController = FindObjectOfType<OptionController>();
    }
    // オプションが変更されたときに実行するメソッド
    void Update()
    {
        //DropdownのValueが0のとき
        if (dropdown.value == 0)
        {
            optionController.objectCountToSet = 8;
            //Debug.Log($"{optionController.objectCountToSet}個");
        }
        //DropdownのValueが1のとき
        else if (dropdown.value == 1)
        {
            optionController.objectCountToSet = 6;
            //Debug.Log($"{optionController.objectCountToSet}個");
        }
        //DropdownのValueが2のとき
        else if (dropdown.value == 2)
        {
            optionController.objectCountToSet = 4;
            //Debug.Log($"{optionController.objectCountToSet}個");
        }
    }
}

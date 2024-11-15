using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroutChoice : MonoBehaviour
{
    [SerializeField] private Dropdown dropdown;

    private OptionController optionController;

    // オプションが変更されたときに実行するメソッド
    public void ChangeTrout()
    {
        //DropdownのValueが0のとき
        if (dropdown.value == 0)
        {
            optionController.objectCountToSet = 8;
        }
        //DropdownのValueが1のとき
        else if (dropdown.value == 1)
        {
            optionController.objectCountToSet = 6;
        }
        //DropdownのValueが2のとき
        else if (dropdown.value == 2)
        {
            optionController.objectCountToSet = 4;
        }
    }
}

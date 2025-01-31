using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroutChoice : MonoBehaviour
{
    private OptionController optionController;

    private void Start()
    {
        optionController = FindObjectOfType<OptionController>();

        optionController.objectCountToSet = 8;
    }
    // オプションが変更されたときに実行するメソッド
    void Update()
    {
        
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroutChoice : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown boxChoice;

    [SerializeField] private TMP_Dropdown turnChoice;

    [SerializeField] private TMP_Dropdown lifeChoice;

    private OptionController optionController;


    private void Start()
    {
        optionController = FindObjectOfType<OptionController>();
    }
    // �I�v�V�������ύX���ꂽ�Ƃ��Ɏ��s���郁�\�b�h
    void Update()
    {
        //Dropdown��Value��0�̂Ƃ�
        if (boxChoice.value == 0)
        {
            optionController.objectCountToSet = 8;
            //Debug.Log($"{optionController.objectCountToSet}��");
        }
        //Dropdown��Value��1�̂Ƃ�
        else if (boxChoice.value == 1)
        {
            optionController.objectCountToSet = 6;
            //Debug.Log($"{optionController.objectCountToSet}��");
        }
        //Dropdown��Value��2�̂Ƃ�
        else if (boxChoice.value == 2)
        {
            optionController.objectCountToSet = 4;
            //Debug.Log($"{optionController.objectCountToSet}��");
        }

        if (turnChoice.value == 0)
        {
            OptionController.maxTurn = 6;
        }
        else if (turnChoice.value == 1)
        {
            OptionController.maxTurn = 10;
        }
        else if (turnChoice.value == 2)
        {
            OptionController.maxTurn = 14;
        }

        if (lifeChoice.value == 0)
        {
            OptionController.maxLife = 3;
        }
        else if (lifeChoice.value == 1)
        {
            OptionController.maxLife = 4;
        }
        else if (lifeChoice.value == 2)
        {
            OptionController.maxLife = 5;
        }
    }
}
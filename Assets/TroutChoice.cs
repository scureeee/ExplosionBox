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
    // �I�v�V�������ύX���ꂽ�Ƃ��Ɏ��s���郁�\�b�h
    void Update()
    {
        //Dropdown��Value��0�̂Ƃ�
        if (dropdown.value == 0)
        {
            optionController.objectCountToSet = 8;
            //Debug.Log($"{optionController.objectCountToSet}��");
        }
        //Dropdown��Value��1�̂Ƃ�
        else if (dropdown.value == 1)
        {
            optionController.objectCountToSet = 6;
            //Debug.Log($"{optionController.objectCountToSet}��");
        }
        //Dropdown��Value��2�̂Ƃ�
        else if (dropdown.value == 2)
        {
            optionController.objectCountToSet = 4;
            //Debug.Log($"{optionController.objectCountToSet}��");
        }
    }
}

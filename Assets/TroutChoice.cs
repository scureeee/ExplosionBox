using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroutChoice : MonoBehaviour
{
    [SerializeField] private Dropdown dropdown;

    private OptionController optionController;

    // �I�v�V�������ύX���ꂽ�Ƃ��Ɏ��s���郁�\�b�h
    public void ChangeTrout()
    {
        //Dropdown��Value��0�̂Ƃ�
        if (dropdown.value == 0)
        {
            optionController.objectCountToSet = 8;
        }
        //Dropdown��Value��1�̂Ƃ�
        else if (dropdown.value == 1)
        {
            optionController.objectCountToSet = 6;
        }
        //Dropdown��Value��2�̂Ƃ�
        else if (dropdown.value == 2)
        {
            optionController.objectCountToSet = 4;
        }
    }
}

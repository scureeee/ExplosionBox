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
    // �I�v�V�������ύX���ꂽ�Ƃ��Ɏ��s���郁�\�b�h
    void Update()
    {
        
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    //�V�[���Ԃŋ��L����I�u�W�F�N�g����ێ�����ϐ�
    public int objectCount;
    // Start is called before the first frame update

    private void Awake()
    {
        //�V���O���g���p�^�[���ŃC���X�^���X��������ɐ���
        if (Instance == null)
        {
            Instance = this;
            //�V�[�����܂����ł��j������Ȃ�
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

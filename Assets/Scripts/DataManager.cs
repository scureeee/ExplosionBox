using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    //�V�[���Ԃŋ��L����I�u�W�F�N�g����ێ�����ϐ�
    public int objectCount;
    // Start is called before the first frame update

    [SerializeField] private string[] destroyInScenes;

    private void Awake()
    {
        //�V���O���g���p�^�[���ŃC���X�^���X��������ɐ���
        if (Instance == null)
        {
            Instance = this;
            //�V�[�����܂����ł��j������Ȃ�
            DontDestroyOnLoad(gameObject);

            // �V�[�������[�h���ꂽ���̃C�x���g��o�^
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // �C�x���g�o�^����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���݂̃V�[������ `destroyInScenes` �Ɋ܂܂�Ă���ꍇ�A�I�u�W�F�N�g���폜
        foreach (var sceneName in destroyInScenes)
        {
            if (scene.name == "ResultScene")
            {
                Destroy(gameObject);
                Instance = null;
                break;
            }
        }
    }
}

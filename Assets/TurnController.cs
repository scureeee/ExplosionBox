using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    // 0���v���C���[�A1���G�Ƃ��Đݒ肷���
    private int firstTurn;

    //��������I�u�W�F�N�g��prefab
    public GameObject objectPrefab;

    //�~�̔��a
    public float radius = 5f;

    //OptionScene�Őݒ肵�����̔z��
    private GameObject[] objectArray;

    // Start is called before the first frame update
    void Start()
    {
        //��s�ƌ�U���߂�
        DecideFirstTurn();

        //DataManager����ݒ肳�ꂽ�I�u�W�F�N�g�����擾
        int numberOfObjects = DataManager.instance.objectCount;

        //�I�u�W�F�N�g���Ɋ�Â��Ĕz����쐬
        objectArray = new GameObject[numberOfObjects];
        GenerateObjectsInCircle(numberOfObjects);
    }

    void GenerateObjectsInCircle(int numberOfObjects)
    {
        for(int i = 0; i < numberOfObjects; i++)
        {
            float angle = i *Mathf.PI * 2 /numberOfObjects;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

            //�I�u�W�F�N�g�����Ɣz��ւ̊i�[
            GameObject obj = Instantiate(objectPrefab, position, Quaternion.identity, transform);
            objectArray[i] = obj;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void DecideFirstTurn()
    {
        // 0�܂���1�̃����_���Ȓl���擾
        firstTurn = Random.Range(0, 2);

        if (firstTurn == 0)
        {
            Debug.Log("�v���C���[����s�ł�");
            // �v���C���[�̃^�[���J�n����
            StartPlayerTurn();
        }
        else
        {
            Debug.Log("�G����s�ł�");
            // �G�̃^�[���J�n����
            StartEnemyTurn();
        }
    }

    void StartPlayerTurn()
    {
        EnemyBombSite();
    }

    void StartEnemyTurn()
    {

    }

    void EnemyBombSite()
    {

    }
}

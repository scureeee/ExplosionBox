using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    // �J�����֘A
    public Camera mainCamera; // ���C���J�������A�T�C������
    public bool isCameraMoving = false; // �J�����������Ă��邩�̃t���O
    public Vector3 cameraStartPosition; // �J�����̏����ʒu
    public Transform targetObject; // �J�������߂Â��^�[�Q�b�g
    public float cameraMoveSpeed = 2f; // �J�����̈ړ����x

    // Start is called before the first frame update
    void Start()
    {
        cameraStartPosition = mainCamera.transform.position; // �����J�����ʒu���L�^
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraBack()
    {
        Debug.Log("hbhbhb");
        mainCamera.transform.position = cameraStartPosition;
    }
}

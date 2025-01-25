using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    // カメラ関連
    public Camera mainCamera; // メインカメラをアサインする
    public bool isCameraMoving = false; // カメラが動いているかのフラグ
    public Vector3 cameraStartPosition; // カメラの初期位置
    public Transform targetObject; // カメラが近づくターゲット
    public float cameraMoveSpeed = 2f; // カメラの移動速度

    // Start is called before the first frame update
    void Start()
    {
        cameraStartPosition = mainCamera.transform.position; // 初期カメラ位置を記録
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

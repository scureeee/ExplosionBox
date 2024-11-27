using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionController : MonoBehaviour
{
    private TurnController turnController;

    public GameObject openBotton;

    public GameObject openCamera;

    private ClickController clickController;

    // Start is called before the first frame update
    void Start()
    {
        clickController = FindObjectOfType<ClickController>();  
        
        turnController = FindObjectOfType<TurnController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("playerが");

            turnController.playerObject.SetActive(false);

            clickController.isMoving = false; // フラグをリセット
            clickController.animator.SetBool("Bool Walk", false);

            //目的地に移動し終えたplayerを元の場所に戻す
            turnController.playerObject.transform.position = Vector3.zero;

            BottonEmerge();
        }
    }

    public void BottonInbisible()
    {
        openBotton.SetActive(false);

        openCamera.SetActive(false);

        turnController.playerObject.SetActive(true);
    }

    public void BottonEmerge()
    {
        openBotton.SetActive(true);

        openCamera.SetActive(true);
    }

    public void boxOpen()
    {
        if (this.gameObject.tag == "Cube")
        {
            //Debug.Log("cubeだよ");

            turnController.randomObject.tag = "Cube";

            BottonInbisible();

            //ターンを進める
            turnController.turnCount = turnController.turnCount + 0.5f;


        }
        else if (this.gameObject.tag == "Explosion")
        {
            //Debug.Log("Explosionだよ");

            this.gameObject.tag = "Cube";

            BottonInbisible();

            //ターンを進める
            turnController.turnCount = turnController.turnCount + 0.5f;
        }
    }
}


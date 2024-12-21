using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionController : MonoBehaviour
{
    private TurnController turnController;

    public GameObject openBotton;

    public GameObject buckBotton;

    public GameObject openCamera;

    public GameObject Bomb;

    public GameObject Explosion;

    private ClickController clickController;

    private float openTime;

    //�A�j���[�^�[�R���|�[�l���g
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        openTime = 0f;

        animator = GetComponent<Animator>();

        clickController = FindObjectOfType<ClickController>();  
        
        turnController = FindObjectOfType<TurnController>();

        Bomb.SetActive(true);

        Explosion.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"open"+openTime);

        //openBotton���L������
        if(openBotton.activeSelf)
        {
            //���Ԍo�߂ŃA�j���[�V�����������Ŏ��s
            openTime += Time.deltaTime;
            if(openTime >= 7f)
            {
                //Animation Event���g����boxOpen���s��
                animator.SetBool("open", true);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("player��");

            turnController.playerObject.SetActive(false);

            clickController.isMoving = false; // �t���O�����Z�b�g
            clickController.animator.SetBool("Bool Walk", false);

            //�ړI�n�Ɉړ����I����player�����̏ꏊ�ɖ߂�
            turnController.playerObject.transform.position = Vector3.zero;

            BottonEmerge();
        }
    }

    public void BottonInbisible()
    {

        openBotton.SetActive(false);

        openCamera.SetActive(false);

        buckBotton.SetActive(false);

        openTime = 0f;

        turnController.choiceTrigger = true;
    }

    IEnumerator WaitForAnimationAndExecute()
    {
        // �A�j���[�V�����̃g���K�[��ݒ�
        animator.SetTrigger("Open");

        // �A�j���[�V�������I������܂őҋ@
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 3f)
        {
            // �A�j���[�V�������Đ����̏ꍇ�͑ҋ@
            yield return null;
        }

        // �A�j���[�V�����I����ɍs����������
        Debug.Log("�A�j���[�V�����I���I");

        // �����ɃA�j���[�V�����I����ɍs�������������L�q

        Explosion.SetActive(true);

        BottonInbisible();

        if (this.gameObject.tag == "Cube")
        {
            //Debug.Log("cube����");

            turnController.randomObject.tag = "Cube";

            // �I�u�W�F�N�g�̔ԍ����擾
            // �I�u�W�F�N�g�̔ԍ��擾
            int assignedNumber = turnController.objectNumberMapping[this.gameObject];

            // �ԍ�+1���|�C���g�ɉ��Z
            turnController.playerPoint += assignedNumber + 1;

            Debug.Log(turnController.playerPoint);
            // �I�������I�u�W�F�N�g�����X�g����폜
            List<GameObject> tempList = new List<GameObject>(turnController.objectArray);

            if (tempList.Contains(this.gameObject))
            {
                tempList.Remove(this.gameObject);  // ���X�g����폜
                turnController.objectArray = tempList.ToArray();  // �z��ɖ߂�

                // �I�u�W�F�N�g���A�N�e�B�u������
                this.gameObject.SetActive(false);

                Debug.Log($"{this.gameObject.name} ��z�񂩂�폜���܂����B");
            }


            //StartCoroutine(WaitForAnimationAndExecute());
            //BottonInbisible();

            if (turnController.turnCount < OptionController.maxTurn)
            {
                //�^�[����i�߂�
                turnController.turnCount += 0.5f;
            }
        }
        else if (this.gameObject.tag == "Explosion")
        {
            //Debug.Log("Explosion����");

            turnController.playerLife -= 1;

            this.gameObject.tag = "Cube";

            if (turnController.turnCount < OptionController.maxTurn)
            {
                //�^�[����i�߂�
                turnController.turnCount += 0.5f;
            }

            //StartCoroutine(WaitForAnimationAndExecute());
            BottonInbisible();           
        }
    }

    public void BottonEmerge()
    {
        openBotton.SetActive(true);

        buckBotton.SetActive(true);
        
        openCamera.SetActive(true);
    }

    public void boxOpen()
    {
        //animator.SetBool("bounce", true);
        //animator.SetBool("open", true);


        StartCoroutine(WaitForAnimationAndExecute());

    }
}


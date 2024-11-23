using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ClickController : MonoBehaviour
{
    [SerializeField] float smooth = 10f;

    public GameObject player;

    //�v���C���[�̈ړ����x
    public float moveSpeed = 5f;

    //�ړ���̃^�[�Q�b�g�ʒu
    private Vector3 targetPosition;

    //�v���C���[���ړ������ǂ���
    private bool isMoving = false;

    //�A�j���[�^�[�R���|�[�l���g
    private Animator animator;
    //Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Ray�𐶐�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            //Ray�𓊎�
            if(Physics.Raycast(ray, out hit))
            {
                //�^�O���r
                if(hit.collider.CompareTag("Cube"))
                {
                    //�v���C���[��cube�Ɉړ�������
                    targetPosition = hit.point;

                    isMoving = true; // �t���O��L����
                }
                if(hit.collider.CompareTag("Base"))
                {
                    Debug.Log("KKK");
                    //�v���C���[��cube�Ɉړ�������
                    targetPosition = hit.point;
                }
            }
        }

        // �v���C���[���ړ������鏈��
        if (isMoving)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        // �v���C���[���^�[�Q�b�g�ʒu�Ɍ����Ĉړ�
        //MoveTowards�֐��ɂ���ăX���[�Y�Ɉړ�����
        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        //�̂̌��������炩�ɕύX����
        Quaternion rotation = Quaternion.LookRotation(targetPosition);
        //�ŏ��Ɍ����Ă����������Time.deltaTime * smooth��targetPosition�Ɍ�����ς���
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);

        animator.SetBool("Bool Walk", true);
        // �^�[�Q�b�g�ʒu�ɓ��B������ړ����I��
        if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
        {
            isMoving = false; // �t���O�����Z�b�g
            animator.SetBool("Bool Walk", false);
        }
    }
}

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
    public bool isMoving = false;

    //�A�j���[�^�[�R���|�[�l���g
    public Animator animator;

    private TurnController turnController;

    //Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        turnController = FindObjectOfType<TurnController>();

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
                //turnCount������������
                if ((int)turnController.turnCount == turnController.turnCount)
                {
                    Debug.Log("����");

                    if(hit.collider.CompareTag("Cube"))
                    {
                        hit.collider.gameObject.tag = "Explosion";

                        Debug.Log($"�I�u�W�F�N�g{hit.collider.gameObject.name}�̃^�O��'Explosion'�ɕύX���܂����B");

                        turnController.EnemyBoxChoice();
                    }
                }
                else
                {
                    Debug.Log("�񐮐�");

                    //�^�O���r
                    //explosion���t���Ă��Ȃ�
                    if (hit.collider.CompareTag("Cube") || hit.collider.CompareTag("Explosion"))
                    {
                        //�v���C���[��cube�Ɉړ�������
                        targetPosition = hit.point;

                        isMoving = true; // �t���O��L����
                    }
                    if (hit.collider.CompareTag("Base"))
                    {
                        Debug.Log("KKK");
                        //�v���C���[��cube�Ɉړ�������
                        targetPosition = hit.point;
                    }
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
    }
}

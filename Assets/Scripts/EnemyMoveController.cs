using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController : MonoBehaviour
{

    //�A�j���[�^�[�R���|�[�l���g
    public Animator enemyAnimator;

    //�ړ���̃^�[�Q�b�g�ʒu
    public Vector3 enemyTarget;

    [SerializeField] float enemySmooth = 10f;

    //�v���C���[�̈ړ����x
    public float enemyMoveSpeed = 5f;

    public bool enemyMoving = false;

    [SerializeField] TurnController turnController;

    // Start is called before the first frame update
    void Start()
    {
        turnController = FindObjectOfType<TurnController>();

        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyMoving == true)
        {
            MoveEnemy();
        }
    }

    public void MoveEnemy()
    {
        // �v���C���[���^�[�Q�b�g�ʒu�Ɍ����Ĉړ�
        //MoveTowards�֐��ɂ���ăX���[�Y�Ɉړ�����
        turnController.enemyObject.transform.position = Vector3.MoveTowards(
            turnController.enemyObject.transform.position,
            enemyTarget,
            enemyMoveSpeed * Time.deltaTime
        );

        //�̂̌��������炩�ɕύX����
        Quaternion rotation = Quaternion.LookRotation(enemyTarget);
        //�ŏ��Ɍ����Ă����������Time.deltaTime * smooth��targetPosition�Ɍ�����ς���
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * enemySmooth);

        enemyAnimator.SetBool("Bool Walk", true);
        // �^�[�Q�b�g�ʒu�ɓ��B������ړ����I��
    }
}

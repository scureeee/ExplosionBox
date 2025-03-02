using System.Collections;
using UnityEngine;
using static TurnController;
using optionSpace;

public class ClickController : MonoBehaviour
{
    [SerializeField] float smooth = 10f;

    public GameObject player;

    //�v���C���[�̈ړ����x
    public float moveSpeed = 5f;

    //�ړ���̃^�[�Q�b�g�ʒu
    public Vector3 targetPosition;

    //�v���C���[���ړ������ǂ���
    public bool isMoving = false;

    //�A�j���[�^�[�R���|�[�l���g
    public Animator animator;

    private TurnController turnController;

    private OptionController optionController;

    // �Đ������� NavMesh �͈͔̔��a
    public float navMeshUpdateRadius = 5f;

    //Start is called before the first frame update
    void Start()
    {
        isMoving = false;

        animator = GetComponent<Animator>();

        optionController = FindObjectOfType<OptionController>();

        turnController = FindObjectOfType<TurnController>();

        StartCoroutine(WaitForTurnController());

        if (turnController != null)
        {
            turnController.InitializeObjectArray();
        }

        StartCoroutine(BuildNavMeshAsync());
    }

    // Update is called once per frame
    void Update()
    {

        // ���݂�state���擾
        TurnController.PhaseState currentState = turnController.GetCurrentState();

        if (Input.GetMouseButtonDown(0))
        {
            //Ray�𐶐�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            //Ray�𓊎�
            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject; // �N���b�N�����I�u�W�F�N�g

                //turnCount������������
                if (currentState == PhaseState.PlayerChoiceToSetBomb)
                {
                    if (hit.collider.CompareTag("Cube"))
                    {
                        hit.collider.gameObject.tag = "Explosion";

                        Debug.Log($"�I�u�W�F�N�g{hit.collider.gameObject.name}�̃^�O��'Explosion'�ɕύX���܂����B");

                        // NavMesh�̔񓯊��\�z���J�n
                        StartCoroutine(BuildNavMeshAsync());

                        // �N���b�N�����I�u�W�F�N�g�ȊO�̃R���C�_�[�𖳌���
                        DeactivateOtherColliders(clickedObject);

                        targetPosition = hit.point;

                        optionController.choiceTime = 60f;

                        // �t���O��L����
                        isMoving = true;

                        turnController.countText.enabled = false;

                        StartCoroutine(turnController.NextState());
                    }
                }
                //��ŕς���
                else if(currentState == PhaseState.PlayerChoiceToOpenBox)
                {
                    //�^�O���r
                    //explosion���t���Ă��Ȃ�
                    if (hit.collider.CompareTag("Cube") || hit.collider.CompareTag("Explosion"))
                    {
                        // NavMesh�̔񓯊��\�z���J�n
                        StartCoroutine(BuildNavMeshAsync());

                        // �N���b�N�����I�u�W�F�N�g�ȊO�̃R���C�_�[�𖳌���
                        DeactivateOtherColliders(clickedObject);

                        targetPosition = hit.point;

                        // �t���O��L����
                        isMoving = true;

                        turnController.countText.enabled = false;

                        StartCoroutine(turnController.NextState());
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

    IEnumerator WaitForTurnController()
    {
        while (turnController == null || turnController.objectArray == null)
        {
            Debug.Log("WaitForTurnController: TurnController �܂��� objectArray �� null �ł��B�ҋ@��...");
            yield return new WaitForSeconds(0.5f);
            turnController = FindObjectOfType<TurnController>();
        }
        Debug.Log("WaitForTurnController: TurnController �̏��������I");
    }

    // �N���b�N�����I�u�W�F�N�g�ȊO�̃R���C�_�[�𖳌������郁�\�b�h
    public void DeactivateOtherColliders(GameObject clickedObject)
    {
        if (turnController == null)
        {
            Debug.LogError("DeactivateOtherColliders: turnController �� null �ł��B");
            return;
        }

        if (turnController.objectArray == null)
        {
            Debug.LogError("DeactivateOtherColliders: objectArray �� null �ł��B");
            return;
        }

        foreach (GameObject obj in turnController.objectArray)
        {
            if (obj == null)
            {
                Debug.LogWarning("DeactivateOtherColliders: objectArray �� null �̗v�f������܂��B");
                continue;
            }

            if (obj != clickedObject)
            {
                Collider collider = obj.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false; // �R���C�_�[�𖳌���
                }
            }
        }

        Debug.Log("�N���b�N�����I�u�W�F�N�g�ȊO�̃R���C�_�[�𖳌������܂����B");
    }

    public void ActivateOtherColliders()
    {
        // "�ΏۃI�u�W�F�N�g" �𔻒肷������Ɋ�Â��擾����
        // �K�v�ɉ����ă^�O�▼�O�ōi�荞��
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true; // �R���C�_�[��L����
            }

            obj.SetActive(true); // �I�u�W�F�N�g���̂��A�N�e�B�u��
        }

        Debug.Log("�V�[�����̂��ׂĂ̑ΏۃI�u�W�F�N�g���A�N�e�B�u�ɂ��܂����B");
    }

    public IEnumerator BuildNavMeshAsync()
    {
        yield return new WaitForSeconds(0.1f); // NavMesh�\�z�O�ɏ����ҋ@
        Debug.Log("NavMesh�̍\�z���������܂����B");
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
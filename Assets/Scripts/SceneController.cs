using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI statusText;
    private const int MaxPlayerPerRoom = 2;

    void Awake()
    {
        //�}�X�^�[�N���C�A���g���V�[����ύX�����ہA���̃v���C���[�̃V�[������������
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //photon�T�[�o�[�ɐڑ�
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnGUI()
    {
        //���݂̐ڑ���Ԃ�\������
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    public void FindPlayer()
    {
        //�T�[�o�[�ڑ�����Ă��邩�m�F
        if(PhotonNetwork.IsConnected)
        {
            //���[���ɎQ������
            PhotonNetwork.JoinRandomRoom();
        }
    }

    //Photon�̃R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        Debug.Log("�}�X�^�[�Ɍq���܂����B");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{cause}�̗��R�Ōq���܂���ł����B");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���[�����쐬");

        //PhotonNetwork.JoinRandomRoom�Ń��[����������Ȃ������ꍇ�ɌĂ΂��
        //�V�������[�����쐬���A���[���̍ő�v���C���[��2�l�ɂ���
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerPerRoom });
    }

    //���[���ɎQ���������ɌĂ΂��
    public override void OnJoinedRoom()
    {
        Debug.Log("���[���ɎQ��");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        //���[���ɋK��l���W�܂��Ă邩���f
        if(playerCount != MaxPlayerPerRoom)
        {
            statusText.text = "stay";
            //statusText.text = "�ΐ푊���҂��Ă��܂��B";
        }
        else
        {
            statusText.text = "go";
            //statusText.text = "�ΐ푊�肪�����܂����B�o�g���V�[���Ɉړ����܂��B";
        }
    }

    //���̃v���C���[�����[���ɓ��������ɌĂ΂��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //�}�X�^�[�N���C�A���g�����f
        if(PhotonNetwork.IsMasterClient)
        {
            //�K��l���ɂȂ�����Scene���ړ�����
            if(PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
            {
                //�V�Kplayer��h��
                PhotonNetwork.CurrentRoom.IsOpen = false;

                statusText.text = "Go";
                //statusText.text = "�ΐ푊�肪�����܂����B�o�g���V�[���Ɉړ����܂��B";

                PhotonNetwork.LoadLevel("SampleScene");
            }
        }
    }
}

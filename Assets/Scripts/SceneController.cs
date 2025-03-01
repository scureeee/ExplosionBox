using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI connectText;

    // ���[���̍ő�l����2�l�ɂ���
    private const int MaxPlayerPerRoom = 2;

    private void Awake()
    {
        //�}�X�^�[�N���C�A���g���V�[����ύX�����ۂɁA���̃v���C���[�̃V�[��������
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        //�Q�[���J�n���T�[�o�[�ɐڑ�
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnGUI()
    {
        //���݂̐ڑ���Ԃ�\��
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    public void FindOponent()
    {
        //�T�[�o�[�ڑ����Ă邩����
        if (PhotonNetwork.IsConnected)
        {
            //���[���ɎQ��
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("�}�X�^�[�Ɍq���܂����B");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{cause}�̗��R�Ōq���܂���ł����B");
    }

    /// <summary>
    /// ���[����������Ȃ������ۂɐV�������[�����쐬
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���[�����쐬���܂��B");

        //���[���̍ő�l����2�l�ɂ���
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerPerRoom });
    }

    /// <summary>
    /// ���[���Q�������ێ��s�����
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("���[���ɎQ�����܂���");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        //�v���C���[��2�l�����Ă�����
        if (playerCount != MaxPlayerPerRoom)
        {
            connectText.text = "stay";
        }
        else
        {
            connectText.text = "go";
        }
    }

    /// <summary>
    /// ���v���C���[���������ہA���s�����
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //�}�X�^�[�N���C�A���g������
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
            {
                //���[���ɐV�K�v���C���[���Q���o���Ȃ�����
                PhotonNetwork.CurrentRoom.IsOpen = false;

                connectText.text = "go";

                PhotonNetwork.LoadLevel("SampleScene");
            }
        }
    }
}

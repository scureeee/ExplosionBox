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

    // ルームの最大人数を2人にする
    private const int MaxPlayerPerRoom = 2;

    private void Awake()
    {
        //マスタークライアントがシーンを変更した際に、他のプレイヤーのシーンも同期
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        //ゲーム開始時サーバーに接続
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnGUI()
    {
        //現在の接続状態を表示
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    public void FindOponent()
    {
        //サーバー接続してるか判別
        if (PhotonNetwork.IsConnected)
        {
            //ルームに参加
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターに繋ぎました。");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{cause}の理由で繋げませんでした。");
    }

    /// <summary>
    /// ルームが見つからなかった際に新しいルームを作成
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ルームを作成します。");

        //ルームの最大人数を2人にする
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerPerRoom });
    }

    /// <summary>
    /// ルーム参加した際実行される
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加しました");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        //プレイヤーが2人揃ってか判別
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
    /// 他プレイヤーが入った際、実行される
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //マスタークライアントか判別
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
            {
                //ルームに新規プレイヤーを参加出来なくする
                PhotonNetwork.CurrentRoom.IsOpen = false;

                connectText.text = "go";

                PhotonNetwork.LoadLevel("SampleScene");
            }
        }
    }
}

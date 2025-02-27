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
        //マスタークライアントがシーンを変更した際、他のプレイヤーのシーンも同期する
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //photonサーバーに接続
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnGUI()
    {
        //現在の接続状態を表示する
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    public void FindPlayer()
    {
        //サーバー接続されているか確認
        if(PhotonNetwork.IsConnected)
        {
            //ルームに参加する
            PhotonNetwork.JoinRandomRoom();
        }
    }

    //Photonのコールバック
    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターに繋ぎました。");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{cause}の理由で繋げませんでした。");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ルームを作成");

        //PhotonNetwork.JoinRandomRoomでルームが見つからなかった場合に呼ばれる
        //新しいルームを作成し、ルームの最大プレイヤーを2人にする
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerPerRoom });
    }

    //ルームに参加した時に呼ばれる
    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        //ルームに規定人数集まってるか判断
        if(playerCount != MaxPlayerPerRoom)
        {
            statusText.text = "stay";
            //statusText.text = "対戦相手を待っています。";
        }
        else
        {
            statusText.text = "go";
            //statusText.text = "対戦相手が揃いました。バトルシーンに移動します。";
        }
    }

    //他のプレイヤーがルームに入った時に呼ばれる
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //マスタークライアントか判断
        if(PhotonNetwork.IsMasterClient)
        {
            //規定人数になったらSceneを移動する
            if(PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
            {
                //新規playerを防ぐ
                PhotonNetwork.CurrentRoom.IsOpen = false;

                statusText.text = "Go";
                //statusText.text = "対戦相手が揃いました。バトルシーンに移動します。";

                PhotonNetwork.LoadLevel("SampleScene");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowserScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button findRoomButton;

    [Header("Lobby")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;

    [Header("Lobby Browser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();

    void Start ()
    {
        //disable the menu buttons at the start
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        //enable the cursor
        Cursor.lockState = CursorLockMode.None;

        //are we in a game
        if(PhotonNetwork.InRoom)
        {
            //go to lobby

            //make the room visible again
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    void SetScreen (GameObject screen)
    {
        //disable all other screens
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);

        //activate requested screen
        screen.SetActive(true);
    }

    public void OnBackButton ()
    {
        SetScreen(mainScreen);
    }

        // MAIN SCREEN

    public void OnPlayerNameValueChanged (TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;
    }

    public void OnCreateRoomButton ()
    {
        SetScreen(createRoomScreen);
    }

    public void OnFindRoomButton ()
    {
        SetScreen(lobbyBrowserScreen);
    }

        //CREATE ROOM SCREEN

    public void OnCreateButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    //LOBBY SCREEN

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    [PunRPC]
    void UpdateLobbyUI ()
    {
        // enable or disable start game button if host
        startGameButton.interactable = PhotonNetwork.IsMasterClient;

        //display all players
        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
            playerListText.text += player.NickName + "\n";

        // set room info text
        roomInfoText.text = "<b>Room Name</b> \n" + PhotonNetwork.CurrentRoom.Name;
    }

    public void OnStartGameButton ()
    {
        //hide the room
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        //tell everyone to load into game scene
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    public void OnLeaveLobbyButton ()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
}

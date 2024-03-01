using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks {
    [Header("Connection Status Text")]
    [SerializeField] public TextMeshProUGUI connectionStatusText;

    [Header("Login UI Panel")]
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] GameObject loginUIPanel;

    [Header("Option UI Panel")]
    [SerializeField] GameObject optionUIPanel;

    [Header("Create Room UI Panel")]
    [SerializeField] GameObject createRoomUIPanel;
    [SerializeField] TMP_InputField roomNameInputField;

    [Header("List Player UI Panel")]
    [SerializeField] GameObject listPlayerUIPanel;

    [Header("Join Random Room UI Panel")]
    [SerializeField] GameObject joinRandomRoomUIPanel;
    [SerializeField] TMP_Text roomNameText;

    private Dictionary<string, RoomInfo> cachedRoomList;

    void Start() {
        ActivePanel(loginUIPanel.name);

        cachedRoomList = new Dictionary<string, RoomInfo>();
    }

    void Update() {
        connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
    }

// UI starts here
    public void OnLoginButtonClicked() {
        string playerName = playerNameInput.text;

        if (!string.IsNullOrEmpty(playerName)) {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        } else {
            Debug.Log("Player Name is invalid");
        }
    }

    public void ActivePanel(string panelToBeActive) {
        loginUIPanel.SetActive(panelToBeActive.Equals(loginUIPanel.name));
        optionUIPanel.SetActive(panelToBeActive.Equals(optionUIPanel.name));
        createRoomUIPanel.SetActive(panelToBeActive.Equals(createRoomUIPanel.name));
        listPlayerUIPanel.SetActive(panelToBeActive.Equals(listPlayerUIPanel.name));
        joinRandomRoomUIPanel.SetActive(panelToBeActive.Equals(joinRandomRoomUIPanel.name));
    }

    public void OnCreateRoomButtonClicked() {
        string roomName = roomNameInputField.text;

        if (string.IsNullOrEmpty(roomName)) {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked() {
        ActivePanel(optionUIPanel.name);
    }

    public void OnShowRoomListButtonClicked() {
        if (!PhotonNetwork.InLobby) {
            PhotonNetwork.JoinLobby();
        }
        
    }

// UI ends here
// Photon Callbacks starts here

    public override void OnConnected() {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster() {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server");
        ActivePanel(optionUIPanel.name);
    }

    public override void OnCreatedRoom() {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom() {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivePanel(listPlayerUIPanel.name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        foreach (RoomInfo room in roomList) {
            Debug.Log("Room Name: " + room.Name);
            if (!room.IsOpen || room.IsVisible || room.RemovedFromList) {
                if (cachedRoomList.ContainsKey(room.Name)) {
                    cachedRoomList.Remove(room.Name);
                }
            }
            cachedRoomList.Add(room.Name, room);
        }
    }
    
// Photon Callbacks ends here
}

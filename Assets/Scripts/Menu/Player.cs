using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MirrorBasics
{
    public class Player : NetworkBehaviour
    {
        public static Player localPlayer;
        NetworkMatch networkMatch;
        [SyncVar] public string matchID;
        [SyncVar] public int playerIndex;
        void Start()
        {
            networkMatch = GetComponent<NetworkMatch>();
            if (isLocalPlayer)
            {
                localPlayer = this;
               
            }
            else
            {
                UILobby.instance.SpawnPlayerUIPrefab(this);
            }
        }
        // JOIN GAME
        public void JoinGame(string _inputID)
        {
            CmdJoinGame(_inputID);
        }
    
        [Command]
        void CmdJoinGame(string _matchID)
        {
            matchID = _matchID;
            if (MatchMaker.Instance.JoinGame(_matchID, GetComponent<NetworkIdentity>(), out playerIndex))
            {
                Debug.Log($"<color=green>Game hosted successfully!</color>");
                networkMatch.matchId = _matchID.ToGuid();
                TargetJoinGame(true,_matchID);
            }
            else
            {
                Debug.Log($"<color=red>Game hosted failed!</color>");
                TargetJoinGame(false,_matchID);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID)
        {
            Debug.Log($"<color=green>Match ID: {matchID} == {_matchID}</color>");
            UILobby.instance.JoinSuccess(success);
        }
        public void HostGame()
        {
            string matchID = MatchMaker.GetRandomMatchID();
            CmdHostGame(matchID);
        }
        // HOST GAME
        [Command]
        void CmdHostGame(string _matchID)
        {
            matchID = _matchID;
            if (MatchMaker.Instance.HostGame(_matchID, GetComponent<NetworkIdentity>(), out playerIndex))
            {
                Debug.Log($"<color=green>Game hosted successfully!</color>");
                networkMatch.matchId = _matchID.ToGuid();
                TargetHostGame(true,_matchID);
            }
            else
            {
                Debug.Log($"<color=red>Game hosted failed!</color>");
                TargetHostGame(false,_matchID);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string _matchID)
        {
            Debug.Log($"<color=green>Match ID: {matchID} == {_matchID}</color>");
            UILobby.instance.HostSuccess(success);
        }
        // BEGIN GAME
        public void BeginGame()
        {
            Debug.Log($"<color=green>here!</color>");
            CmdBeginGame();
        }

        [Command]
        void CmdBeginGame()
        {
                Debug.Log("started instancing game");
                MatchMaker.Instance.BeginGame(matchID);
                Debug.Log($"<color=green>Game beginning!</color>");
           

        }

        public void StartGame()
        {
            Debug.Log($"<color=green>Game started!</color>");
            TargetBeginGame();
        }
        [TargetRpc]
        void TargetBeginGame()
        {
            Debug.Log($"<color=green>Match ID: {matchID} | Beginning</color>");
           SceneManager.LoadScene(2,LoadSceneMode.Additive);
        }
    }
}
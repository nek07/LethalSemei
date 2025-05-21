using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Mirror;

namespace MirrorBasics
{
    [Serializable]
    public class Match
    {
        public string matchID;
        public List<NetworkIdentity> players = new List<NetworkIdentity>();

        public Match(string matchID, NetworkIdentity player)
        {
            this.matchID = matchID;
            players.Add(player);
        }

        public Match() { }
    }

    public class MatchMaker : NetworkBehaviour
    {
        public static MatchMaker Instance { get; private set; }

        public List<Match> matches = new List<Match>();
        public List<string> matchIDs = new List<string>();
        [SerializeField] GameObject turnManagerPrefab;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public bool HostGame(string _matchID, NetworkIdentity _player,out int playerIndex)
        {
            playerIndex = -1;
            if (!matchIDs.Contains(_matchID))
            {
                matchIDs.Add(_matchID);
                matches.Add(new Match(_matchID, _player));
                Debug.Log($"Match generated: {_matchID}");
                playerIndex = 0;
                return true;
            }
            else
            {
                Debug.Log($"Match ID {_matchID} already exists");
                return false;
            }
        }
        public bool JoinGame(string _matchID, NetworkIdentity _player, out int playerIndex)
        {
            playerIndex = -1;
            if (matchIDs.Contains(_matchID))
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].matchID == _matchID)
                    {
                        matches[i].players.Add(_player);
                        playerIndex = matches[i].players.Count -1;
                        break;
                    }
                }
                Debug.Log($"Match joined");
            
                return true;
            }
            else
            {
                Debug.Log($"Match ID not exist");
                return false;
            }
        }
        public void BeginGame(string _matchID)
        {
            Debug.Log($"Begin game {_matchID}");
            GameObject newTurnManager = Instantiate(turnManagerPrefab);
            NetworkServer.Spawn(newTurnManager);
            newTurnManager.GetComponent<NetworkMatch>().matchId = _matchID.ToGuid();
            TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();
            
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    foreach (var player in matches[i].players)
                    {
                        Player _player = player.GetComponent<Player>();
                        turnManager.AddPlayer(_player);
                        _player.StartGame();
                    }
                    break;
                }
            }
        }
        public static string GetRandomMatchID()
        {
            string _id = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int random = UnityEngine.Random.Range(0, 36);
                if (random < 26)
                {
                    _id += (char)(random + 65);
                }
                else
                {
                    _id += (random - 26).ToString();
                }
            }
            Debug.Log($"Random Match ID: {_id}");
            return _id;
        }
        
    }
   
    public static class MatchMakerExtensions
    {
        public static Guid ToGuid(this string id)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(bytes);
            return new Guid(hashBytes);
        }
    }
}
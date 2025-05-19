using System.Collections.Generic;
using Mirror;
using Unity;
using UnityEngine;

namespace MirrorBasics
{
    public class TurnManager : NetworkBehaviour
    {
        List<Player> players = new List<Player>();
        
        public void AddPlayer(Player _player)
        {
         players.Add(_player);   
        }
    }
}
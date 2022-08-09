using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Room
    {
        public PlayerClient player1;
        public PlayerClient player2;
        
        public Room(PlayerClient _player1, PlayerClient _player2, int id)
        {
            player1 = _player1;
            player2 = _player2;
           
            player1.BeginCommunicate(player2);
            player2.BeginCommunicate(player1);

            player1.currentRoom = this;
            player2.currentRoom = this;

            player1.WriteData($"{player1.name} {id} 0");
            player2.WriteData($"{player2.name} {id} 1");
        }
        public void Abandon()
        {
            DedicatedServer.roomList.Remove(this);           
        }
        public void SetDisconnectedPlayer(int id, PlayerClient sub)
        {
            sub.currentRoom = this;
            if(id == 0)
            {
                player1 = sub;
                player2.partner = player1;
                player1.BeginCommunicate(player2);
            }
            else
            {
                player2 = sub;
                player1.partner = player2;
                player2.BeginCommunicate(player2);
            }
            
        }
        public PlayerClient GetPlayer(int id)
        {
            return id == 0 ? player1 : player2;
        }
    }
}

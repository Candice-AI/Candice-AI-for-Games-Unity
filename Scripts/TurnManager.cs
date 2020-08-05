using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViridaxGameStudios
{
    public class TurnManager : MonoBehaviour
    {
        public List<TacticsPlayer> players;
        private static Queue<TacticsPlayer> playerTurnQueue;
        public static TacticsPlayer currentPlayer;
        bool gameRunning;
        // Start is called before the first frame update
        void Start()
        {


            if (players != null)
            {
                playerTurnQueue = new Queue<TacticsPlayer>();
                foreach (TacticsPlayer player in players)
                {
                    playerTurnQueue.Enqueue(player);
                }
                gameRunning = true;
                BeginTurn();

            }
            else
            {
                Debug.Log("No players.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (gameRunning)
            {
                foreach (TacticsPlayer player in players)
                {
                    if (player.units.Count < 0)
                    {
                        gameRunning = false;
                        Debug.Log("Game Over.");
                    }
                }
            }
        }

        public static void BeginTurn()
        {
            if (playerTurnQueue.Count > 0)
            {
                currentPlayer = playerTurnQueue.Dequeue();
                currentPlayer.BeginTurn();
            }
            else
            {
                Debug.Log("No players in queue");
            }

        }

        public static void EndTurn(TacticsPlayer player)
        {
            if (player != null)
            {
                playerTurnQueue.Enqueue(player);
            }
            BeginTurn();
        }
    }

}

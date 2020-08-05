using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViridaxGameStudios.AI;

namespace ViridaxGameStudios
{
    public class TacticsPlayer : MonoBehaviour
    {
        public string playerName;
        public List<AIController> units;
        private AIController selectedUnit;
        public bool turn = false;

        void Start()
        {
            CandiceAIManager.getInstance().OnDestinationReached += onDestinationReached;
            foreach (AIController agent in units)
            {
                agent._player = this;
            }
        }
        void Update()
        {
            if (turn)
            {
                CheckClicked();
            }

            /*
            if(selectedUnit != null)
            {
                PlayerMove playerMove = selectedUnit.GetComponent<PlayerMove>();
                if(playerMove != null)
                {
                    playerMove.enabled = true;
                    playerMove.turn = true;
                }
            }
            */
        }

        public bool IsAlly(AIController agent)
        {
            bool isAlly = false;
            int count = 0;
            while (!isAlly && count < units.Count)
            {
                if (units[count].agentID == agent.agentID)
                {
                    isAlly = true;
                }
                count++;
            }

            return isAlly;
        }
        void onDestinationReached(AIController agent)
        {
            foreach (AIController unit in units)
            {
                if (agent.agentID == unit.agentID)
                {
                    EndTurn();
                }
            }
        }
        public void BeginTurn()
        {
            Debug.Log("Begiining " + playerName + " turn");
            turn = true;
        }

        public void EndTurn()
        {
            ClearSelectedUnit();
            turn = false;
            TurnManager.EndTurn(this);
        }

        void CheckClicked()
        {
            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("Clicked");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject go = hit.collider.gameObject;
                    AIController agent = go.GetComponent<AIController>();
                    if (agent != null)
                    {
                        foreach (AIController ai in units)
                        {
                            if (agent.agentID == ai.agentID)
                            {
                                CandiceAIManager.getInstance().ComputeAdjacencyList(agent.jumpHeight);
                                Debug.Log("Clicked agent " + agent.agentID);
                                if (selectedUnit != null)
                                {
                                    ClearSelectedUnit();
                                }
                                selectedUnit = agent;
                                selectedUnit.isSelected = true;
                                selectedUnit.turn = true;
                                selectedUnit.isPlayerControlled = true;
                            }
                        }
                    }
                }
            }
        }

        void ClearSelectedUnit()
        {
            selectedUnit.isSelected = false;
            selectedUnit.turn = false;
            selectedUnit.RemoveSelectableTiles();
            selectedUnit.isPlayerControlled = false;
        }
    }

}

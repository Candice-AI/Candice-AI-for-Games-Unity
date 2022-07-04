using CandiceAIforGames.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    [System.Serializable]
    public abstract class CandiceBehaviorNode
    {
        public Transform transform;
        public CandiceAIController aiController;
        public int patrolCount = 0;
        public int id;
        /* Delegate that returns the state of the node.*/
        public delegate CandiceBehaviorStates NodeReturn();

        /* The current state of the node */
        protected CandiceBehaviorStates m_nodeState;

        public CandiceBehaviorStates nodeState
        {
            get { return m_nodeState; }
        }

        /* The constructor for the node */
        public CandiceBehaviorNode() { }
        public void Initialise(Transform transform, CandiceAIController aiController)
        {
            this.transform = transform;
            this.aiController = aiController;
        }

        /* Implementing classes use this method to evaluate the desired set of conditions */
        /// <summary>
        /// Inherited classes use this method to evaluate the desired set of conditions
        /// </summary>
        public abstract CandiceBehaviorStates Evaluate();

    }
}

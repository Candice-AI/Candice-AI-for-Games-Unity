using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    [Serializable]
    public class CandiceBehaviorInverter : CandiceBehaviorNode
    {
        /* Child node to evaluate */
        private CandiceBehaviorNode m_node;

        public CandiceBehaviorNode node
        {
            get { return m_node; }
        }

        /* The constructor requires the child node that this CandiceBehaviorInverter decorator 
         * wraps*/
        public CandiceBehaviorInverter(CandiceBehaviorNode node)
        {
            m_node = node;
        }

        /* Reports a success if the child fails and 
         * a failure if the child succeeds. Running will report 
         * as running */
        public override CandiceBehaviorStates Evaluate()
        {
            switch (m_node.Evaluate())
            {
                case CandiceBehaviorStates.FAILURE:
                    m_nodeState = CandiceBehaviorStates.SUCCESS;
                    return m_nodeState;
                case CandiceBehaviorStates.SUCCESS:
                    m_nodeState = CandiceBehaviorStates.FAILURE;
                    return m_nodeState;
                case CandiceBehaviorStates.RUNNING:
                    m_nodeState = CandiceBehaviorStates.RUNNING;
                    return m_nodeState;
            }
            m_nodeState = CandiceBehaviorStates.SUCCESS;
            return m_nodeState;
        }
    }
}
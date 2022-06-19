using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    [Serializable]
    public class CandiceBehaviorSequence : CandiceBehaviorNode
    {
        /** Children nodes that belong to this sequence */
        private List<CandiceBehaviorNode> m_nodes = new List<CandiceBehaviorNode>();

        /** Must provide an initial set of children nodes to work */
        public CandiceBehaviorSequence()
        {

        }
        public void SetNodes(List<CandiceBehaviorNode> nodes)
        {
            m_nodes = nodes;
        }
        public void AddNode(CandiceBehaviorNode node)
        {
            m_nodes.Add(node);
        }
        /* If any child node returns a failure, the entire node fails. Whence all  
         * nodes return a success, the node reports a success. */
        public override CandiceBehaviorStates Evaluate()
        {
            bool anyChildRunning = false;

            foreach (CandiceBehaviorNode node in m_nodes)
            {
                switch (node.Evaluate())
                {
                    case CandiceBehaviorStates.FAILURE:
                        m_nodeState = CandiceBehaviorStates.FAILURE;
                        return m_nodeState;
                    case CandiceBehaviorStates.SUCCESS:
                        continue;
                    case CandiceBehaviorStates.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        m_nodeState = CandiceBehaviorStates.SUCCESS;
                        return m_nodeState;
                }
            }
            m_nodeState = anyChildRunning ? CandiceBehaviorStates.RUNNING : CandiceBehaviorStates.SUCCESS;
            return m_nodeState;
        }
    }
}

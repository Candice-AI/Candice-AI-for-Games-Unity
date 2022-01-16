using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    [Serializable]
    public class CandiceBehaviorSelector : CandiceBehaviorNode
    {
        /** The child nodes for this CandiceBehaviorSelector */
        protected List<CandiceBehaviorNode> m_nodes = new List<CandiceBehaviorNode>();


        /** The constructor requires a lsit of child nodes to be  
         * passed in*/
        public CandiceBehaviorSelector()
        {

        }
        public void SetNodes(List<CandiceBehaviorNode> nodes)
        {
            m_nodes = nodes;
        }
        public List<CandiceBehaviorNode> GetNodes()
        {
            return m_nodes;
        }
        public void AddNode(CandiceBehaviorNode node)
        {
            m_nodes.Add(node);
        }
        /* If any of the children reports a success, the CandiceBehaviorSelector will 
         * immediately report a success upwards. If all children fail, 
         * it will report a failure instead.*/
        public override CandiceBehaviorStates Evaluate()
        {
            foreach (CandiceBehaviorNode node in m_nodes)
            {
                switch (node.Evaluate())
                {
                    case CandiceBehaviorStates.FAILURE:
                        continue;
                    case CandiceBehaviorStates.SUCCESS:
                        m_nodeState = CandiceBehaviorStates.SUCCESS;
                        return m_nodeState;
                    case CandiceBehaviorStates.RUNNING:
                        m_nodeState = CandiceBehaviorStates.RUNNING;
                        return m_nodeState;
                    default:
                        continue;
                }
            }
            m_nodeState = CandiceBehaviorStates.FAILURE;
            return m_nodeState;
        }
    }
}

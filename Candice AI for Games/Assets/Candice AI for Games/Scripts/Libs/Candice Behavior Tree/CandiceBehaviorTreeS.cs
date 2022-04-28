using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    [Serializable]
    public class CandiceBehaviorTreeS
    {
        public string name;
        [SerializeField]
        private List<CandiceBehaviorNodeS> nodes;

        public void SetNodes(List<CandiceBehaviorNodeS> _nodes)
        {
            nodes = new List<CandiceBehaviorNodeS>();
            foreach (CandiceBehaviorNodeS node in _nodes)
            {
                CandiceBehaviorNodeS newNode = new CandiceBehaviorNodeS(node.id, node.type, node.childrenIDs, node.function, node.isRoot, node.x, node.y, node.width, node.height, node.number);
                nodes.Add(newNode);
            }
        }
        public List<CandiceBehaviorNodeS> GetNodes()
        {
            return nodes;
        }
        public void AddNode(CandiceBehaviorNodeS node)
        {
            nodes.Add(node);
        }
    }
}


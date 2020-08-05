using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViridaxGameStudios.AI
{
    public class Tile : MonoBehaviour
    {
        public bool walkable = true;
        public bool current = false;
        public bool target = false;
        public bool selectable = false;
        public List<Tile> adjacencyList = new List<Tile>();

        //BFS (Breadth first search)
        public bool visited = false;
        public Tile parent = null;
        public int distance = 0;

        public float f = 0;
        public float g = 0;
        public float h = 0;

        void Update()
        {
            if (current)
            {
                GetComponent<Renderer>().material.color = Color.magenta;
            }
            else if (target)
            {
                GetComponent<Renderer>().material.color = Color.green;
            }
            else if (selectable)
            {
                GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.blue;
            }
        }

        public void Reset()
        {
            adjacencyList.Clear();
            current = false;
            target = false;
            selectable = false;
            visited = false;
            parent = null;
            distance = 0;
            f = g = h = 0;
        }
        public void FindNeighbors(float jumpHeight, Tile target)
        {
            Reset();
            CheckTile(Vector3.forward, jumpHeight, target);
            CheckTile(-Vector3.forward, jumpHeight, target);
            CheckTile(Vector3.right, jumpHeight, target);
            CheckTile(-Vector3.right, jumpHeight, target);
        }

        public void CheckTile(Vector3 direction, float jumpHeight, Tile target)
        {
            Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
            Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);
            foreach (Collider item in colliders)
            {
                Tile tile = item.GetComponent<Tile>();
                if (tile != null && tile.walkable)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
                    {
                        adjacencyList.Add(tile);
                    }
                }
            }
        }
    }
}
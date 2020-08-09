using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace ViridaxGameStudios.AI
{
    [DisallowMultipleComponent()]
    public class CandiceAIManager : MonoBehaviour
    {
        public bool enableDebug;//   
        //public static CandiceAIManager getInstance;
        private static CandiceAIManager instance;
        private Queue<PathResult> results = new Queue<PathResult>();//Data strucure containing a collection of all paths requested by all AI Agents/Controllers
        private PathFinding pathFinding;//Pathfinding module that does the actual calculations to find a path.
        public Grid grid;//The grid that contains all the nodes
        private Dictionary<string,Faction> factions;
        
        Queue<RegistrationRequest> registrationQueue = new Queue<RegistrationRequest>();

        public Dictionary<int, GameObject> agents = new Dictionary<int, GameObject>();
        public int agentCount = 0;

        public static string[] arrNodeTypes = { "Selector", "Sequence", "Inverter", "Action" };
        public static string[] arrFunctions = { "None", "MoveTo", "LookAt", "Attack", "EnemyDetected" };//This is managed by the Candice Behavior Designer.
        public const int NODE_TYPE_SELECTOR = 0;
        public const int NODE_TYPE_SEQUENCE = 1;
        public const int NODE_TYPE_INVERTER = 2;
        public const int NODE_TYPE_ACTION = 3;

        #region Events
        public event Action<GameObject, GameObject> OnPlayerDetected = delegate { };
        public event Action<GameObject> OnPlayerHealthLow = delegate { };
        public event Action<AIController> OnDestinationReached = delegate { };
        public event Action<GameObject> OnCharacterDead = delegate { };

        public void PlayerDetected(GameObject source, GameObject player)
        {
            OnPlayerDetected(source, player);
        }
        public void CharacterDead(GameObject go)
        {
            OnCharacterDead(go);
        }
        public void PlayerHealthLow(GameObject player)
        {
            OnPlayerHealthLow(player);
        }
        public void DestinationReached(AIController agent)
        {
            OnDestinationReached(agent);
        }
        #endregion

        public static CandiceAIManager getInstance()
        {
            if(instance == null)
            {
                instance = new CandiceAIManager();
            }
            return instance;
        }
        public CandiceAIManager()
        {

        }
        void VerifyFactionList()
        {
            if(factions == null)
            {
                factions = new Dictionary<string, Faction>();
            }
        }
        public bool AddFaction(Faction faction)
        {
            VerifyFactionList();
            bool isAdded = false;
            if(!factions.ContainsKey(faction.name))
            {
                factions.Add(faction.name, faction);
                isAdded = true;
            }

            return isAdded;
        }
        public Faction GetFaction(string name)
        {
            Faction faction = null;
            if(factions.ContainsKey(name))
            {
                faction = factions[name];
            }
            return faction;
        }
        public List<Faction> GetAllFactions()
        {
            VerifyFactionList();
            List<Faction> lst = new List<Faction>();
            foreach(KeyValuePair<string,Faction> item in factions)
            {
                lst.Add(item.Value);
            }

            return lst;
        }
        private void Awake()
        {
            Initialise();
        }
        
        private void Start()
        {
            
        }
        private void Initialise()
        {
            CandiceConfig.enableDebug = enableDebug;
            instance = this;
            if(grid == null)
            {
                grid = GetComponent<Grid>();
            }
            if(grid != null)
            {
                pathFinding = new PathFinding(grid);
            }
            else
            {
                if(enableDebug)
                {
                    Debug.LogError("Cannot initialise Candice Pathfinding. Please make sure to set the Grid variable.");
                }
            }
            
        }
        private void Update()
        {
            CandiceConfig.enableDebug = enableDebug;
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
                }
            }

            if(registrationQueue.Count > 0)
            {
                int itemsInQueue = registrationQueue.Count;
                lock (registrationQueue)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        RegistrationRequest rr = registrationQueue.Dequeue();
                        bool isRegistered;
                        try
                        {
                            agentCount++;
                            agents.Add(agentCount, rr.agent);
                            isRegistered = true;
                        }
                        catch(Exception e)
                        {
                            isRegistered = false;
                        }
                        rr.callback(isRegistered, agentCount);
                    }
                }
            }
        }
        #region BFS AND TILE
        public Tile GetCurrentTile(GameObject go, Tile collidedObject)
        {
            Tile currentTile = GetTargetTile(go);
            if (currentTile == null)
                currentTile = collidedObject;
            currentTile.current = true;
            return currentTile;
        }
        public Tile GetTargetTile(GameObject target)
        {
            RaycastHit hit;
            Tile tile = null;
            if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
            {
                tile = hit.collider.GetComponent<Tile>();
            }
            return tile;
        }
        public void FindSelectableTiles(Tile currentTile, float maxMovePoints, Action<List<Tile>> selectableTiles)
        {
            pathFinding.FindSelectableTiles(currentTile, maxMovePoints, selectableTiles);
        }

        public void ComputeAdjacencyList(float jumpHeight)
        {
            pathFinding.ComputeAdjacencyList(jumpHeight, null);
        }
        #endregion
        #region A* Pathfinding
        //This method is called by the AI agents in order to receive a path to their goal, using the Pathfinding module.
        public static void RequestASTARPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                getInstance().pathFinding.FindASTARPath(request, getInstance().FinishedProcessingPath);
            };
            threadStart.Invoke();
        }
        public static void RequestBFSPath(Tile tile, Action<Stack<Tile>> callback)
        {
            ThreadStart threadStart = delegate
            {
                getInstance().pathFinding.FindBFSPath(tile, callback);
            };
            threadStart.Invoke();
        }
        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                //Add the result to the queue.
                results.Enqueue(result);
            }

        }
        #endregion

        public void RegisterAgent(GameObject agent, Action<bool,int> callback)
        {
            getInstance().registrationQueue.Enqueue(new RegistrationRequest(agent, callback));
        }
    }
    public struct PathResult
    {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }
    }
    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }

    public struct RegistrationRequest
    {
        public GameObject agent;
        public Action<bool, int> callback;

        public RegistrationRequest(GameObject _agent, Action<bool, int> _callback)
        {
            agent = _agent;
            callback = _callback;
        }
    }

}

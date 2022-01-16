using CandiceAIforGames.AI.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    public class CandiceAIManager : MonoBehaviour
    {
        [SerializeField]
        private bool drawGridGizmos = true;
        [SerializeField]
        private bool drawAllAgentPaths = true;
        //public static CandiceAIManager getInstance;
        private static CandiceAIManager instance;
        private Queue<PathResult> results = new Queue<PathResult>();//Data strucure containing a collection of all paths requested by all AI Agents/Controllers
        private CandicePathFinding pathFinding;//Pathfinding module that does the actual calculations to find a path.
        public CandiceGrid grid;//The grid that contains all the nodes


        Queue<RegistrationRequest> registrationQueue = new Queue<RegistrationRequest>();

        public Dictionary<int, GameObject> agents = new Dictionary<int, GameObject>();
        public int agentCount = 0;

        public static string[] arrNodeTypes = { "Selector", "Sequence", "Inverter", "Action" };
        public static string[] arrFunctions = { "None", "MoveTo", "LookAt", "Attack", "EnemyDetected" };//This is managed by the Candice Behavior Designer.
        public const int NODE_TYPE_SELECTOR = 0;
        public const int NODE_TYPE_SEQUENCE = 1;
        public const int NODE_TYPE_INVERTER = 2;
        public const int NODE_TYPE_ACTION = 3;

        public bool DrawGridGizmos { get => drawGridGizmos; set => drawGridGizmos = value; }
        public bool DrawAllAgentPaths { get => drawAllAgentPaths; set => drawAllAgentPaths = value; }

        #region Events
        public event Action<GameObject, GameObject> OnPlayerDetected = delegate { };
        public event Action<GameObject> OnPlayerHealthLow = delegate { };
        public event Action<CandiceAIController> OnDestinationReached = delegate { };
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
        public void DestinationReached(CandiceAIController agent)
        {
            OnDestinationReached(agent);
        }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            //Process all agent pathfinding requests
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
            //Process all agent registration requests
            if (registrationQueue.Count > 0)
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
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                            isRegistered = false;
                        }
                        rr.callback(isRegistered, agentCount);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Process all agent registration requests
            if (registrationQueue.Count > 0)
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
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                            isRegistered = false;
                        }
                        rr.callback(isRegistered, agentCount);
                    }
                }
            }
            //Process all agent pathfinding requests
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
            
        }
        private void Awake()
        {
            Initialise();
        }
        public static CandiceAIManager getInstance()
        {
            if (instance == null)
            {
                instance = new CandiceAIManager();

            }
            return instance;
        }
        private void Initialise()
        {
            //CandiceConfig.enableDebug = enableDebug;
            instance = this;
            if (grid == null)
            {
                grid = GetComponent<CandiceGrid>();
            }
            if (grid != null)
            {
                pathFinding = new CandicePathFinding(grid);
            }
            else
            {
                Debug.LogError("Cannot initialise Candice Pathfinding. Please make sure to set the Grid variable.");
            }

        }
        public bool IsPointWalkable(Vector3 point)
        {
            return grid.isWalkable(point);
        }

        public void RegisterAgent(GameObject agent, Action<bool, int> callback)
        {
            getInstance().registrationQueue.Enqueue(new RegistrationRequest(agent, callback));
        }

        #region A* Pathfinding
        //This method is called by the AI agents in order to receive a path to their goal, using the Pathfinding module.
        public void RequestASTARPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                Debug.Log("Getting Path");
                getInstance().pathFinding.FindASTARPath(request, getInstance().FinishedProcessingPath);
            };
            threadStart.Invoke();
        }
        /*public static void RequestBFSPath(Tile tile, Action<Stack<Tile>> callback)
        {
            ThreadStart threadStart = delegate
            {
                getInstance().pathFinding.FindBFSPath(tile, callback);
            };
            threadStart.Invoke();
        }*/
        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                //Add the result to the queue.
                results.Enqueue(result);
            }

        }
        #endregion

        private void OnDrawGizmos()
        {

            if (DrawGridGizmos)
                grid.DrawGrid();
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


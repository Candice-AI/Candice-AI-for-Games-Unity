using CandiceAIforGames.AI.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    public class CandiceAIManager : MonoBehaviour
    {
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

        }

        // Update is called once per frame
        void Update()
        {

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


using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace ViridaxGameStudios.AI
{
    public enum PatrolType
    {
        PatrolPoints,
        Waypoints,
    }
    public enum MovementType
    {
        STATIC,
        DYNAMIC,
        TILE_BASED
    }
    public enum PathfindSource
    {
        None,
        Candice,
        UnityNavmesh,
    }
    public class AIController : Character
    {
        #region Variables
        public int agentID;
        public BehaviorTree behaviorTree;
        private ObstacleAvoidance obstacleAvoidance;//Obstacle avoidance module to allow the agent to move and evade obstacles.
        private ObjectScanner objectScanner;
        public SensorType sensorType;
        bool playerDead = false;

        //Detection Variables
        public float m_DetectionRadius = 10f;
        public float m_DetectionHeight = 2f;
        public float m_LineOfSight = 180.0f;
        public bool enemyFound = false;//Bolean to know if the enemy has been found    
        public bool allyFound = false;
        public bool playerFound = false;
        public List<GameObject> patrolPoints = new List<GameObject>();
        public int patrolCount = 0;
        public bool patrolInOrder = true;
        public bool isPatrolling = false;
        public bool pointReached = false;//Boolean to know if the agent reached to targeted patrol point.
        public PatrolType patrolType;
        

        

        
        //public bool followPlayer;

        public bool stopBehaviorTree = false;
        Coroutine updatePathCoroutine;

        //Movement Variables
        public float oaDistance = 10f;//Obstacle Avoidance Distance. How far the agent will detect objects to avoid.

        /*
         * Pathfinding Variables
         */
        Path path;//The path that the Agent will use to follow.
        public float minPathUpdateTime = .2f;//Minimum time it will take for the agent before attempting to request a new updated path from Candice.
        public float pathUpdateMoveThreshold = .5f;// Minimum distance the target can move by before requesting a new Updated path from Candice.
        public float turnSpeed;//The speed the agent will turn between waypoints by when pathfinding.
        public float turnDist;//The ditance the agent will start to turn while moving to the next node.
        public float stoppingDist;//How far away from the target the agent will start to slow down and stop.

        public bool fallingDown = false;
        public bool jumpingUp = false;
        public bool movingEdge = false;
        public Vector3 jumpTarget;



        //public Vector3 velocity = new Vector3();
        //public Vector3 heading = new Vector3(); // direction heading
        

        public List<GameObject> enemies;
        public List<GameObject> allies;
        public GameObject player;
        public List<Tile> selectableTiles = new List<Tile>();
        public Stack<Tile> tilePath = new Stack<Tile>();
        bool followingPath;
        public Waypoint waypoint;
        public bool hasAnimations;
        public float oaAOE = 0.5f;

        #endregion


        #region Main Methods
        private void Awake()
        {

        }//End Awake()


        public override void Start()
        {
            //Call start from the base class.
            base.Start();
            obstacleAvoidance = new ObstacleAvoidance();
            objectScanner = new ObjectScanner(transform, onObjectFound);
            ragdoll = GetComponentsInChildren<Rigidbody>();
            
            //Check if there is a Candice AI Manager Component in the scene.
            CandiceAIManager candice = FindObjectOfType<CandiceAIManager>();
            if (candice == null)
            {
                Debug.LogError(CandiceConfig.CONTROLLER_NAME + ": You need to attach a Candice AI Manager Component to an Empty GameObject.");
            }
            if (is3D)
            {
                if (enableRagdoll)
                {
                    EnableRagdoll();
                }


            }
            if (healthBar != null)
            {
                healthBar.SetAgentName(name);
            }
            if (!isPlayerControlled)
            {
                if (behaviorTree != null)
                {
                    if (behaviorTree.behaviorTree != null)
                    {
                        behaviorTree.CreateBehaviorTree(this);
                    }
                }
            }

            //Subscribe to Events
            CandiceAIManager.getInstance().OnDestinationReached += onDestinationReached;
            CandiceAIManager.getInstance().OnCharacterDead += onCharacterDead;
            CandiceAIManager.getInstance().OnPlayerHealthLow += onPlayerHealthLow;
            CandiceAIManager.getInstance().OnPlayerDetected += onPlayerDetected;
            //Register this agent with Candice
            CandiceAIManager.getInstance().RegisterAgent(gameObject, onRegistrationComplete);

        }//End Start()

        void LateUpdate()
        {
            if (isPlayerControlled)
            {
                if (moveType == MovementType.TILE_BASED)
                {
                    if (isSelected)
                    {
                        currentTile = CandiceAIManager.getInstance().GetCurrentTile(gameObject, collidedObject);
                        CandiceAIManager.getInstance().FindSelectableTiles(currentTile, movePoints, onSelectableTilesFound);
                        CheckMouse();
                        if (followingPath)
                        {
                            FollowSimplePath();
                        }
                    }
                }
                else
                {
                    ProcessInput();
                }

            }
            else
            {
                DetectionRequest req = new DetectionRequest(sensorType, enemyTags, allyTags, m_DetectionRadius, m_DetectionHeight, m_LineOfSight, is3D);
                objectScanner.ScanForObjects(req);//Scan for objects and ultimately enemies using the m_DetectionRadius.
                if (behaviorTree != null)
                {
                    if (behaviorTree.rootNode != null)
                        behaviorTree.rootNode.Evaluate();
                }
            }
        }
        private void OnAnimatorIK(int layerIndex)
        {
            if (enableHeadLook && headLookTarget != null)
            {
                Animator.SetLookAtPosition(headLookTarget.transform.position);//Enable the agent to look at the target object
                Animator.SetLookAtWeight(headLookIntensity);//Set the intensity of how much the agent will turn their head to look at the target.
            }
        }
        #endregion


        #region Override Methods
        public override void CharacterDead()
        {
            //Unsubscribe Events
            Debug.Log("Character Dead");
            CandiceAIManager.getInstance().OnDestinationReached -= onDestinationReached;
            CandiceAIManager.getInstance().OnCharacterDead -= onCharacterDead;
            CandiceAIManager.getInstance().OnPlayerHealthLow -= onPlayerHealthLow;
            CandiceAIManager.getInstance().OnPlayerDetected -= onPlayerDetected;

            CandiceAIManager.getInstance().CharacterDead(gameObject);
            base.CharacterDead();
        }

        #endregion

        #region Helper Methods 
        private void onRegistrationComplete(bool isRegistered, int id)
        {
            if (isRegistered)
            {
                agentID = id;
                /*
                if (!isPlayerControlled)
                {
                    if (healthBar != null)
                        healthBar.SetAgentName("AI Agent " + id);
                }
                */
                if (CandiceConfig.enableDebug)
                    Debug.Log("Agent " + agentID + " successfully registered with Candice.");
            }
            //Debug.Log("Agent " + agentID + " successfully registered with Candice.");

        }

        public void onPlayerHealthLow(GameObject player)
        {
            if (CandiceConfig.enableDebug)
                Debug.Log("The players health is low");
        }
        void onCharacterDead(GameObject go)
        {
            if (gameObject.tag.Equals("Player"))
            {
                playerDead = true;
            }
        }
        public void EnableRagdoll()
        {
            if (ragdoll != null && ragdoll.Length > 0)
            {
                foreach (Rigidbody rb in ragdoll)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    //rb.WakeUp();
                    //rb.gameObject.GetComponent<Collider>().isTrigger = false;
                    rb.gameObject.GetComponent<Collider>().enabled = true;
                }
            }
            if(hasAnimations)
            {
                Animator.enabled = false;
            }
            
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;

        }
        void DisableRagdoll()
        {
            if (hasAnimations)
            {
                Animator.enabled = true;
            }
            if (ragdoll != null && ragdoll.Length > 0)
            {
                foreach (Rigidbody rb in ragdoll)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    //rb.Sleep();
                    //rb.gameObject.GetComponent<Collider>().isTrigger = true;
                    rb.gameObject.GetComponent<Collider>().enabled = false;
                }
            }
            if (hasAnimations)
            {
                Animator.enabled = true;
            }
            gameObject.GetComponent<Collider>().enabled = true;
            //gameObject.GetComponent<Rigidbody>().useGravity = true;
            //gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        public void StartFinding()
        {
            if (updatePathCoroutine == null)
            {
                //Start moving the agent using Candice's pathfinding module.
                updatePathCoroutine = StartCoroutine(UpdatePath());
            }
        }
        public void StopFinding()
        {
            //Stop moving the agent using Candice's pathfinding module.
            StopCoroutine(UpdatePath());
            StopCoroutine("FollowPath");
        }

        void onDestinationReached(AIController agent)
        {
            if (agent.agentID == agentID)
            {
                fsm.ChangeToState(idleState);
                //isMoving = false;
                //moveToTile = false;

                RemoveSelectableTiles();

                if (CandiceConfig.enableDebug)
                    Debug.Log("Dstination reached");
            }

        }
        void onObjectFound(DetectionResults results)
        {
            if (results.enemies != null && results.enemies.Count > 0)
            {
                enemyFound = true;
                enemies = results.enemies;
            }
            else
            {
                enemyFound = false;
            }

            if (results.allies != null && results.allies.Count > 0)
            {
                allyFound = true;
                allies = results.allies;
            }
            else
            {
                allyFound = false;
            }

            if (results.player != null && results.player.Count > 0)
            {
                playerFound = true;
                player = results.player[0];
                CandiceAIManager.getInstance().PlayerDetected(gameObject, player);
            }
            else
            {
                playerFound = false;
            }



            if (!isPatrolling && !enemyFound)
            {
                attackTarget = null;
                /*
                if (!isMoving)
                {
                    moveTarget = null;
                    movePoint = new Vector3();
                }
                */
            }
        }
        void TargetLost()
        {

        }
        void onPlayerDetected(GameObject source, GameObject player)
        {
            //Implement logic here for when the player is detected
            //Debug.Log("Player Detected");
        }
        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, turnDist, stoppingDist);
                StopCoroutine("FollowAStarPath");
                StartCoroutine("FollowAStarPath");
            }
        }


        IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < .3f)
            {
                yield return new WaitForSeconds(.3f);
            }
            CandiceAIManager.RequestASTARPath(new PathRequest(transform.position, moveTarget.transform.position, OnPathFound));
            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = moveTarget.transform.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                if (moveTarget != null)
                {
                    if ((moveTarget.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                    {
                        CandiceAIManager.RequestASTARPath(new PathRequest(transform.position, moveTarget.transform.position, OnPathFound));
                        targetPosOld = moveTarget.transform.position;
                    }
                }
                if (!isMoving)
                {
                    StopCoroutine(updatePathCoroutine);
                    StopCoroutine("FollowAStarPath");
                    updatePathCoroutine = null;
                }
            }

        }

        IEnumerator FollowAStarPath()
        {
            bool followingPath = true;
            int pathIndex = 0;
            if (is3D)
                transform.LookAt(path.lookPoints[0]);
            float speedPercent = 1;
            while (followingPath)
            {

                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
                while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                    {
                        pathIndex++;
                        if (is3D)
                            transform.LookAt(path.lookPoints[pathIndex]);
                    }
                }
                if (followingPath)
                {
                    if (pathIndex >= path.slowDownIndex && stoppingDist > 0)
                    {
                        speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDist);
                        if (speedPercent < 0.01f)
                        {
                            followingPath = false;
                        }
                    }

                    if (is3D)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                        transform.Translate(Vector3.forward * Time.deltaTime * statMoveSpeed.value * speedPercent, Space.Self);
                    }
                    else
                    {
                        //Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                        //Debug.Log(path.lookPoints[pathIndex]);
                        //Vector3 lookPoint = path.lookPoints[pathIndex];
                        //transform.position += path.lookPoints[pathIndex];
                        MoveTo(path.lookPoints[pathIndex], statMoveSpeed.value, is3D);
                        //transform.Translate( Vector3.forward * Time.deltaTime * movementSpeed * speedPercent, Space.Self);
                    }
                }
                yield return null;
            }
        }

        public void RemoveSelectableTiles()
        {
            //Reset the selctable tiles list

            if (currentTile != null)
            {
                currentTile.current = false;
                currentTile = null;
            }

            foreach (Tile tile in selectableTiles)
            {
                tile.Reset();
            }
            selectableTiles.Clear();
            CandiceAIManager.getInstance().ComputeAdjacencyList(jumpHeight);
        }

        void CheckMouse()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Tile")
                    {
                        Tile t = hit.collider.GetComponent<Tile>();
                        if (t.selectable)
                        {
                            //MoveToTile(t);
                            //movePoint.GetComponent<Collider>().enabled = true;
                            //movePoint.transform.position = hit.point;
                            moveTarget = hit.collider.gameObject;
                            movePoint = hit.point;
                            //moveToTile = true;
                            MoveToTile(t);
                            //CandiceAIManager.SimpleMove(transform, target.transform, movementSpeed, true);
                        }
                    }
                    else
                    {
                        AIController agent = hit.collider.gameObject.GetComponent<AIController>();
                        if (agent != null)
                        {
                            if (!_player.IsAlly(agent))
                            {
                                transform.LookAt(agent.transform);
                                if (hasAnimations)
                                {
                                    Animator.SetTrigger(attackAnimParameter);
                                }
                                
                            }
                        }
                    }
                }
            }
        }
        void onSelectableTilesFound(List<Tile> selectableTiles)
        {
            this.selectableTiles = selectableTiles;
        }
        public void onBFSPathFound(Stack<Tile> bfsPath)
        {
            if (bfsPath != null)
            {
                tilePath = bfsPath;
                //Put code to follow the path
                followingPath = true;



            }
        }
        public static void SimpleMove(Transform transform, Vector3 target, float movementSpeed, bool is3D)
        {
            if (is3D)
            {
                transform.LookAt(target);
                transform.position += transform.forward * movementSpeed * Time.deltaTime;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
            }
        }
        public static void SimpleMove(Transform transform, Transform target, float movementSpeed, bool is3D)
        {
            SimpleMove(transform, target.position, movementSpeed, is3D);

        }
        public void MoveToTile(Tile tile)
        {
            //CandiceAIManager.RequestBFSPath(tile, onBFSPathFound);
            tilePath.Clear();
            tile.target = true;
            //isMoving = true;

            Tile next = tile;
            while (next != null)
            {
                tilePath.Push(next);
                next = next.parent;
            }
            tilePath.Pop();
            followingPath = true;

            //Debug.Log("Tiles: " + tilePath.Count);
        }
        void FollowSimplePath()
        {
            followingPath = true;
            if (tilePath.Count > 0)
            {
                Tile t = tilePath.Peek();
                Vector3 target = t.transform.position;
                //Calculate the units position on top of the target tile
                target.y = transform.position.y;

                if (Vector3.Distance(transform.position, target) >= 0.2f)
                {
                    if (hasAnimations)
                    {
                        Animator.SetBool(moveAnimParameter, true);
                    }
                    

                    //Locomotion
                    transform.LookAt(target, Vector3.up);
                    //float angle = Vector3.Angle(transform.position, target);
                    //transform.Rotate(Vector3.up, angle);
                    MoveForward();

                    //CalculateHeading(target);
                    //SetHorizontalVelocity();
                    //transform.forward = heading;
                    //transform.position += velocity * Time.deltaTime;
                }
                else
                {
                    if (tilePath.Count == 1)
                    {
                        transform.position = target;
                    }
                    tilePath.Pop();

                }
            }
            else
            {
                followingPath = false;
                CandiceAIManager.getInstance().DestinationReached(this);
                //RemoveSelectableTiles();
                if(hasAnimations)
                {
                    Animator.SetBool(moveAnimParameter, false);
                }
                
            }
            //Debug.Log("Following: " )
            //followPathCoroutine = null;
        }
        public void MoveForward()
        {
            if (is3D)
            {
                if(moveType == MovementType.STATIC)
                {
                    transform.position += transform.forward * statMoveSpeed.value * Time.deltaTime;
                }
                else if (moveType == MovementType.DYNAMIC)
                {
                    obstacleAvoidance.Move(moveTarget.transform, transform, halfHeight + oaAOE, statMoveSpeed.value, is3D, oaDistance);
                }
                
                //rb.velocity += direction *( movementSpeed * Time.deltaTime);
            }
            else
            {
                if (CandiceConfig.enableDebug)
                    Debug.LogError("cannot call MoveForward if is3D is false");
            }

        }

        public void MoveTo(Vector3 target, float movementSpeed, bool is3D)
        {
            if (is3D)
            {
                //transform.position += Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
                MoveForward();
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
            }
        }
        public void Patrol()
        {
            if(patrolType == PatrolType.PatrolPoints)
            {
                NormalPatrol();
            }
            else if( patrolType == PatrolType.Waypoints)
            {
                WaypointPatrol();
            }
            
        }
        void NormalPatrol()
        {
            if (pointReached)
            {
                if (patrolInOrder)
                {
                    if (patrolCount < patrolPoints.Count - 1)
                    {
                        patrolCount++;
                    }
                    else
                    {
                        patrolCount = 0;
                    }
                }
                else
                {
                    UnityEngine.Random rnd = new UnityEngine.Random();
                    patrolCount = UnityEngine.Random.Range(0, patrolPoints.Count);
                }
                mainTarget = patrolPoints[patrolCount];
                moveTarget = mainTarget;
                movePoint = moveTarget.transform.position;
                pointReached = false;
            }
            else
            {
                mainTarget = patrolPoints[patrolCount];
                moveTarget = mainTarget;
            }
        }
        void WaypointPatrol()
        {
            if(Vector3.Distance(transform.position,movePoint) < .5f)
            {
                pointReached = true;
            }
            if(pointReached)
            {
                waypoint = waypoint.nextWaypoint;
                if(waypoint != null)
                {
                    mainTarget = waypoint.gameObject;
                    moveTarget = waypoint.gameObject;
                    movePoint = waypoint.GetPosition();
                }
                pointReached = false;
                
            }
            else
            {
                if(waypoint == null)
                {
                    Debug.LogError("No waypoint assigned.");
                    return;
                }
                mainTarget = waypoint.gameObject;
                moveTarget = waypoint.gameObject;
                movePoint = waypoint.GetPosition();
            }
        }
        #endregion
        #region Override Methods
        public void OnDrawGizmos()
        {
            if (path != null)
            {
                path.DrawWithGizmos();
            }

            if (path != null)
            {
                for (int i = 0; i < path.lookPoints.Length; i++)
                {
                    Gizmos.color = Color.white;
                    if (i != 0)
                    {
                        Gizmos.DrawLine(path.lookPoints[i - 1], path.lookPoints[i]);
                    }

                }
            }
        }
        void OnCollisionEnter(Collision col)
        {
            GameObject go = col.gameObject;
            if (go.tag == "Tile")
            {
                Tile t = go.GetComponent<Tile>();
                if (t != null && collidedObject != t)
                {
                    collidedObject = t;
                    collidedObject.selectable = false;
                }

                //currentTile = collidedObject;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (isPatrolling && moveTarget != null)
            {
                if (other.gameObject.tag.Equals("PatrolPoint") && other.gameObject.name.Equals(moveTarget.gameObject.name))
                {
                    pointReached = true;
                    if (CandiceConfig.enableDebug)
                        Debug.Log("Patrol Point reached");
                }
            }

            if (other.gameObject.name == "movePoint" && isPlayerControlled)
            {
                CandiceAIManager.getInstance().DestinationReached(this);
            }

        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isPatrolling)
            {
                if (other.gameObject.tag.Equals("PatrolPoint") && other.gameObject.name.Equals(moveTarget.gameObject.name))
                {
                    pointReached = true;
                    if (CandiceConfig.enableDebug)
                        Debug.Log("Patrol Point reached");
                }
            }
            if (other.gameObject.name == "movePoint" && isPlayerControlled)
            {
                CandiceAIManager.getInstance().DestinationReached(this);
            }


        }
        #endregion
    }
    [Serializable]
    public class SpecialAbility
    {
        public string abilityName = "<<Not Set>>";
        public KeyCode abilityKey;
    }


}
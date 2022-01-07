using CandiceAIforGames.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CandiceAIforGames.AI
{
    #region ENUMS
    public enum PatrolType
    {
        PatrolPoints,
        Waypoints,
    }
    public enum AttackType
    {
        Melee,
        Range,
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
    public enum SensorType
    {
        Sphere,
        Box,
    }
    public enum AnimationType
    {
        TransitionBased,
        CodeBased,
    }
    #endregion
    public class CandiceAIController : MonoBehaviour
    {

        #region Member Variables
        /*
         * General Variables
         */
        [SerializeField]
        private int agentID;
        [SerializeField]
        private float maxHitPoints = 100f;
        [SerializeField]
        private float hitPoints = 100f;
        [SerializeField]
        private bool is3D = true;

        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private GameObject rig;

        [SerializeField]
        private bool enableRagdoll = false;

        [SerializeField]
        private List<GameObject> enemies = new List<GameObject>();
        [SerializeField]
        private List<GameObject> allies = new List<GameObject>();
        [SerializeField]
        private List<GameObject> players = new List<GameObject>();
        [SerializeField]
        private GameObject player;
        [SerializeField]
        private GameObject mainTarget;
        [SerializeField]
        private bool hasAnimations;
        [SerializeField]
        private AnimationType animationType = AnimationType.TransitionBased;
        [SerializeField]
        private string idleAnimationName = "idle";
        [SerializeField]
        private string moveAnimationName = "walk";
        [SerializeField]
        private string runAnimationName = "run";
        [SerializeField]
        private string attackAnimationName = "attack";
        [SerializeField]
        private string jumpAnimationName = "jump";
        [SerializeField]
        private string deadAnimationName = "dead";
        [SerializeField]
        private string currentAnimation = "none";

        private string idleTransitionParameter = "idle";
        [SerializeField]
        private string moveTransitionParameter = "walk";
        [SerializeField]
        private string runTransitionParameter = "run";
        [SerializeField]
        private string attackTransitionParameter = "attack";
        [SerializeField]
        private string jumpTransitionParameter = "jump";
        [SerializeField]
        private string deadTransitionParameter = "die";

        /*
         * Detection Variables
         */
        [SerializeField]
        private float detectionRadius = 3f;
        [SerializeField]
        private float lineOfSight = 3f;
        [SerializeField]
        private float detectionHeight = 3f;
        [SerializeField]
        private SensorType sensorType = SensorType.Box;
        [SerializeField]
        private bool objectDetected = false;
        [SerializeField]
        private bool playerDetected = false;
        [SerializeField]
        private bool allyDetected = false;
        [SerializeField]
        private bool enemyDetected = false;
        [SerializeField]
        private List<string> objectTags = new List<string>();
        [SerializeField]
        private List<string> enemyTags = new List<string>();
        [SerializeField]
        private List<string> allyTags = new List<string>();
        [SerializeField]
        private LayerMask perceptionMask;
        [SerializeField]
        private float obstacleAvoidaceDistance = 3f;
        [SerializeField]
        private float obstacleAvoidanceAOE = 3f;

        /*
         * Movement Variables
         */
        [SerializeField]
        private GameObject moveTarget;
        [SerializeField]
        private float moveSpeed = 7f;
        [SerializeField]
        private bool isMoving = false;
        [SerializeField]
        private bool enableHeadLook = false;
        [SerializeField]
        private GameObject headLookTarget;
        [SerializeField]
        private float headLookIntensity = 1f;

        /*
         * Combat Variables
         */
        [SerializeField]
        private GameObject attackTarget;
        [SerializeField]
        private float attackDamage = 3f;
        [SerializeField]
        private float attackSpeed = 1f;
        [SerializeField]
        private float attackRange = 1f;
        [SerializeField]
        private float damageAngle = 45f;
        [SerializeField]
        private AttackType attackType = AttackType.Melee;
        [SerializeField]
        private GameObject projectile;
        [SerializeField]
        private Transform projectileSpawnPos;
        [SerializeField]
        private bool hasAttackAnimation = false;
        [SerializeField]
        private bool isAttacking = false;





        /*
         * Modules
         */
        public CandiceModuleMovement movementModule;
        public CandiceModuleDetection detectionModule;
        public CandiceModuleCombat combatModule;

        /*
         * Properties
         */
        public int AgentID { get => agentID; set => agentID = value; }
        
        public List<GameObject> Enemies { get => enemies; set => enemies = value; }
        public List<GameObject> Allies { get => allies; set => allies = value; }
        public List<GameObject> Players { get => players; set => players = value; }
        public GameObject Player { get => player; set => player = value; }
        public bool ObjectDetected { get => objectDetected; set => objectDetected = value; }
        public bool PlayerDetected { get => playerDetected; set => playerDetected = value; }
        public bool AllyDetected { get => allyDetected; set => allyDetected = value; }
        public bool EnemyDetected { get => enemyDetected; set => enemyDetected = value; }
        public GameObject MainTarget { get => mainTarget; set => mainTarget = value; }
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }
        public SensorType SensorType { get => sensorType; set => sensorType = value; }
        public float DetectionRadius { get => detectionRadius; set => detectionRadius = value; }
        public float LineOfSight { get => lineOfSight; set => lineOfSight = value; }
        public float DetectionHeight { get => detectionHeight; set => detectionHeight = value; }
        public bool Is3D { get => is3D; set => is3D = value; }
        public float AttackDamage { get => attackDamage; set => attackDamage = value; }
        public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
        public LayerMask PerceptionMask { get => perceptionMask; set => perceptionMask = value; }
        public float ObstacleAvoidaceDistance { get => obstacleAvoidaceDistance; set => obstacleAvoidaceDistance = value; }
        public float ObstacleAvoidanceAOE { get => obstacleAvoidanceAOE; set => obstacleAvoidanceAOE = value; }
        public List<string> EnemyTags { get => enemyTags; set => enemyTags = value; }
        public List<string> AllyTags { get => allyTags; set => allyTags = value; }
        public List<string> ObjectTags { get => objectTags; set => objectTags = value; }
        public float AttackRange { get => attackRange; set => attackRange = value; }
        public bool EnableHeadLook { get => enableHeadLook; set => enableHeadLook = value; }
        public GameObject HeadLookTarget { get => headLookTarget; set => headLookTarget = value; }
        public float HeadLookIntensity { get => headLookIntensity; set => headLookIntensity = value; }
        public GameObject AttackTarget { get => attackTarget; set => attackTarget = value; }
        public GameObject MoveTarget { get => moveTarget; set => moveTarget = value; }
        public AttackType AttackType { get => attackType; set => attackType = value; }
        public GameObject Projectile { get => projectile; set => projectile = value; }
        public Transform ProjectileSpawnPos { get => projectileSpawnPos; set => projectileSpawnPos = value; }
        public bool HasAttackAnimation { get => hasAttackAnimation; set => hasAttackAnimation = value; }
        public bool HasAnimations { get => hasAnimations; set => hasAnimations = value; }
        public AnimationType AnimationType { get => animationType; set => animationType = value; }
        public string IdleAnimationName { get => idleAnimationName; set => idleAnimationName = value; }
        public string MoveAnimationName { get => moveAnimationName; set => moveAnimationName = value; }
        public string RunAnimationName { get => runAnimationName; set => runAnimationName = value; }
        public string AttackAnimationName { get => attackAnimationName; set => attackAnimationName = value; }
        public string JumpAnimationName { get => jumpAnimationName; set => jumpAnimationName = value; }
        public string DeadAnimationName { get => deadAnimationName; set => deadAnimationName = value; }
        public string CurrentAnimation { get => currentAnimation; set => currentAnimation = value; }
        public string IdleTransitionParameter { get => idleTransitionParameter; set => idleTransitionParameter = value; }
        public string MoveTransitionParameter { get => moveTransitionParameter; set => moveTransitionParameter = value; }
        public string RunTransitionParameter { get => runTransitionParameter; set => runTransitionParameter = value; }
        public string AttackTransitionParameter { get => attackTransitionParameter; set => attackTransitionParameter = value; }
        public string JumpTransitionParameter { get => jumpTransitionParameter; set => jumpTransitionParameter = value; }
        public string DeadTransitionParameter { get => deadTransitionParameter; set => deadTransitionParameter = value; }
        public float DamageAngle { get => damageAngle; set => damageAngle = value; }
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public Camera MainCamera { get => mainCamera; set => mainCamera = value; }
        public GameObject Rig { get => rig; set => rig = value; }
        public bool EnableRagdoll { get => enableRagdoll; set => enableRagdoll = value; }
        public float MaxHitPoints { get => maxHitPoints; set => maxHitPoints = value; }
        public float HitPoints { get => hitPoints; set => hitPoints = value; }

        #endregion
        // Start is called before the first frame update
        void Start()
        {
            UnityEditor.Undo.RecordObject(this, "Description");
            combatModule = new CandiceModuleCombat(transform,onAttackComplete,"Agent" + AgentID + "-CandiceModuleCombat");
            movementModule = new CandiceModuleMovement("Agent" + AgentID + "-CandiceModuleMovement");
            detectionModule = new CandiceModuleDetection(gameObject.transform,onObjectFound, "Agent" + AgentID + "-CandiceModuleDetection");
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void ScanForObjects()
        {
            CandiceDetectionRequest req = new CandiceDetectionRequest(sensorType, objectTags, DetectionRadius, DetectionHeight, LineOfSight, Is3D);
            detectionModule.ScanForObjects(req);
        }

        public void Attack()
        {
            if (hasAttackAnimation && !IsAttacking)
            {
                //Play attack animation which will call the Damage() function
                IsAttacking = true;
            }
            else if (!IsAttacking)
            {
                IsAttacking = true;
                StartCoroutine(combatModule.DealTimedDamage(1f / AttackSpeed, AttackDamage, AttackRange, DamageAngle, enemyTags));

            }
        }
        public void ReceiveDamage(float damage)
        {
            HitPoints = combatModule.ReceiveDamage(damage, HitPoints);
        }

        void onObjectFound(CandiceDetectionResults results)
        {
            AllyDetected = false;
            EnemyDetected = false;
            PlayerDetected = false;
            foreach (string key in results.objects.Keys)
            {
                if(EnemyTags.Contains(key))
                {
                    EnemyDetected = true;
                    Enemies.AddRange(results.objects[key]);
                    MainTarget = Enemies[0];
                    AttackTarget = Enemies[0];
                }
                if (AllyTags.Contains(key))
                {
                    AllyDetected = true;
                    Allies.AddRange(results.objects[key]);
                }
                if (key == "Player")
                {
                    PlayerDetected = true;
                    Players.AddRange(results.objects[key]);
                    Player = Players[0];
                    
                }
            }
        }
        void onAttackComplete(bool success)
        {
            IsAttacking = false;
        }
    }
}

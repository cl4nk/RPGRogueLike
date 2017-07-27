using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Character
{
    public class PathEnemy : MonoBehaviour
    {
        public enum PATHSTATES
        {
            Waiting = 0,
            Hunting,
            LostTarget,
            Research,
            Repositioning,
            nbStates
        }

        private readonly List<Vector3> listWayPoint = new List<Vector3>();

        [SerializeField] private float angleView = 40f;

        private Enemy enemy; // himself

        private Vector3 lastPosTarget = Vector3.zero;

        private LevelManager levelMgr;
        private float maxRotate;

        private float minRotate;

        private PATHSTATES pathState = PATHSTATES.Waiting;
        private Vector3 posStart;

        public float rangeOfView;

        [SerializeField] private float rotationSpeed = 8f;

        private int sign = -1;

        [SerializeField] private float speed = 6f;

        [SerializeField] private float stoppingDistance;

        public PathEnemy()
        {
            Target = null;
        }

        public PATHSTATES PathState
        {
            get { return pathState; }
        }

        public bool IsMoving { get; private set; }

        public Transform Target { get; private set; }

        public float RangeOfView
        {
            get { return rangeOfView; }
            set { rangeOfView = value; }
        }

        private void Start()
        {
            levelMgr = LevelManager.Instance;
            enemy = GetComponent<Enemy>();

            posStart = transform.position;
            listWayPoint.Add(posStart);
        }

        private void Update()
        {
            IsMoving = false;

            if (pathState == PATHSTATES.Hunting)
                Hunting();
            else if (pathState == PATHSTATES.LostTarget)
                LostTarget();
            else if (pathState == PATHSTATES.Research)
                Research();
            else if (pathState == PATHSTATES.Repositioning)
                Repositioning();
        }

        private void FixedUpdate()
        {
            if (Target)
                if (CheckRayCollideWithEntity(transform.position, Target.position - transform.position, "Wall"))
                {
                    if (CheckRayCollideWithEntity(transform.position,
                        Target.GetComponent<Renderer>().bounds.max - transform.position, "Wall"))
                        lastPosTarget = Target.GetComponent<Renderer>().bounds.min;
                    else if (CheckRayCollideWithEntity(transform.position,
                        Target.GetComponent<Renderer>().bounds.min - transform.position, "Wall"))
                        lastPosTarget = Target.GetComponent<Renderer>().bounds.max;

                    minRotate = CheckRotate(transform.localEulerAngles.y - angleView / 2);
                    maxRotate = CheckRotate(transform.localEulerAngles.y + angleView / 2);

                    listWayPoint.Add(lastPosTarget);

                    Target = null;
                    pathState = PATHSTATES.LostTarget;
                }
        }

        private void Move(Vector3 posTarget)
        {
            IsMoving = true;
            Vector3 lookPos = posTarget - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos),
                rotationSpeed * Time.deltaTime);

            if (pathState == PATHSTATES.Hunting && Vector3.Distance(transform.position, posTarget) > stoppingDistance)
                transform.position += transform.forward * speed * Time.deltaTime;
            else if (pathState != PATHSTATES.Hunting)
                transform.position += transform.forward * speed * Time.deltaTime;
        }

        public void CheckIfPlayerIsSeen(Transform player)
        {
            Vector3 dirPlayer = player.position - transform.position;

            if (!Target && pathState != PATHSTATES.Hunting &&
                Vector3.Angle(transform.TransformDirection(Vector3.forward), dirPlayer) <= angleView / 2)
                if (CheckRayCollideWithEntity(transform.position, player.position - transform.position, "Player"))
                    NewTarget(player);
        }

        public void AttackUndergoByPlayer(Transform charac)
        {
            if (Vector3.Distance(transform.position, charac.position) < rangeOfView &&
                enemy.State != Character.STATES.Dead)
                NewTarget(charac);
        }

        public void NewTarget(Transform player)
        {
            pathState = PATHSTATES.Hunting;
            Target = player;

            levelMgr.EnemyDetectPlayer(enemy.gameObject);
        }

        private bool CheckRayCollideWithEntity(Vector3 origin, Vector3 dir, string tagEntity, float maxDistance = 0f)
        {
            RaycastHit hit;

            if (maxDistance == 0f)
            {
                if (Physics.Raycast(origin, dir, out hit))
                    if (hit.collider.tag == tagEntity)
                        return true;
            }
            else
            {
                if (Physics.Raycast(origin, dir, out hit, maxDistance))
                    if (hit.collider.tag == tagEntity)
                        return true;
            }

            return false;
        }

        private float CheckRotate(float rotate)
        {
            if (rotate <= 0)
                rotate = 360 + rotate;
            else if (rotate > 360)
                rotate = rotate - 360;

            return rotate;
        }

        private void Hunting()
        {
            Move(Target.position);

            listWayPoint.Add(transform.position);

            if (Vector3.Distance(transform.position, Target.position) > rangeOfView)
            {
                levelMgr.EnemyLostPlayer(enemy.gameObject);
                Target = null;
                pathState = PATHSTATES.Repositioning;
            }
        }

        private void LostTarget()
        {
            Move(lastPosTarget);

            if (Vector3.Distance(transform.position, lastPosTarget) < 1)
                pathState = PATHSTATES.Research;

            if (CheckRayCollideWithEntity(transform.position, transform.forward, "Wall", 1f))
            {
                levelMgr.EnemyLostPlayer(enemy.gameObject);
                listWayPoint.Remove(listWayPoint[listWayPoint.Count - 1]);
                pathState = PATHSTATES.Repositioning;
            }
        }

        private void Research()
        {
            transform.RotateAround(transform.position, Vector3.up, 4 * sign * rotationSpeed * Time.deltaTime);

            if (sign == 0 && Target == null)
            {
                sign = -1;
                pathState = PATHSTATES.Repositioning;

                levelMgr.EnemyLostPlayer(enemy.gameObject);
            }

            if (sign < 0 && transform.localEulerAngles.y <= minRotate)
                sign *= -1;
            else if (sign > 0 && transform.localEulerAngles.y >= maxRotate)
                sign = 0;
        }

        private void Repositioning()
        {
            if (!CheckRayCollideWithEntity(transform.position, posStart - transform.position, "Wall"))
                listWayPoint.RemoveRange(1, listWayPoint.Count - 1);

            Vector3 currentWaypoint = listWayPoint[listWayPoint.Count - 1];

            if (Vector3.Distance(transform.position, currentWaypoint) < 1)
            {
                listWayPoint.Remove(currentWaypoint);

                if (listWayPoint.Count == 0)
                    pathState = PATHSTATES.Waiting;
            }

            Move(currentWaypoint);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangeOfView);

            Gizmos.color = Color.green;
            Vector3 resDir = Quaternion.AngleAxis(angleView / 2, Vector3.up) * transform.forward;
            Gizmos.DrawRay(transform.position, resDir * rangeOfView);
            resDir = Quaternion.AngleAxis(-angleView / 2, Vector3.up) * transform.forward;
            Gizmos.DrawRay(transform.position, resDir * rangeOfView);

            if (Target)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, (Target.position - transform.position) * rangeOfView);

                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(transform.position,
                    (Target.GetComponent<Renderer>().bounds.max - transform.position) * rangeOfView);
                Gizmos.DrawRay(transform.position,
                    (Target.GetComponent<Renderer>().bounds.min - transform.position) * rangeOfView);
            }

            if (pathState == PATHSTATES.LostTarget)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(lastPosTarget, new Vector3(1, 1, 1));
            }

            Gizmos.DrawSphere(posStart, 1f);
        }
    }
}
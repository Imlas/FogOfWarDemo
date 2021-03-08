using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public bool isCircle = false;

    [Header("FoW Object Masks")]
    public int visTargetLayer;
    public int invisTargetLayer;
    public LayerMask targetMask;
    public List<GameObject> visibleTargets = new List<GameObject>();

    [Header("LoS Blocker Mask")]
    public LayerMask obstacleMask;


    [Header("FoV Resolution")]
    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;
    public float obstaclePeekDist;

    [Header("FoV Mesh Filters")]
    public MeshFilter viewMeshFilterPrimary;
    public MeshFilter viewMeshFilterSecondary;
    Mesh viewMesh;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "FoV Mesh";
        viewMeshFilterPrimary.mesh = viewMesh;
        viewMeshFilterSecondary.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask); //All targets within the view distance

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                //The target is within the view angle
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    //No obstacle detected between the player and the target
                    visibleTargets.Add(target.gameObject);
                }
            }

        }

        ToggleVisibilityOfTargets();

    }

    void ToggleVisibilityOfTargets()
    {
        //Will first get a list of all targets and switch them to an "invisible" layer
        foreach(GameObject target in TargetManager.Instance.GetTargets())
        {
            target.layer = invisTargetLayer;
        }

        //Then will switch the layer of all visible targets to visible
        foreach(GameObject target in visibleTargets)
        {
            target.layer = visTargetLayer;
        }
    }

    //Makes a Mesh that correctly accounts for the direction/facing/LoS blockers
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);

            if(i > 0) //Can't compare the first time 'round since oldViewCast hasn't done anything yet
            {
                bool edgeDistExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistExceeded)) //If one of the casts hit something and the other didn't
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles;
        if(!isCircle)
        {
            triangles = new int[(vertexCount - 2) * 3];
        }
        else
        {
            triangles = new int[(vertexCount - 2) * 3 + 3];
        }

        vertices[0] = Vector3.zero;

        for(int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i] + Vector3.forward * obstaclePeekDist);

            if( i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        if(isCircle)
        {
            triangles[triangles.Length - 3] = 0;
            triangles[triangles.Length - 2] = vertexCount-1;
            triangles[triangles.Length - 1] = 1;
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    //Takes in two view casts, one of which hit and object, the other didn't
    //Returns two points which are very close to the edge
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = minViewCast.point;
        Vector3 maxPoint = maxViewCast.point;

        for(int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle); //A view cast going at the middle point

            bool edgeDistExceeded = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;

            if (newViewCast.hit == minViewCast.hit && !edgeDistExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA; 
        public Vector3 pointB; 

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}

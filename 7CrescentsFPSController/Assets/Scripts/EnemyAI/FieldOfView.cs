using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Tooltip("Düşmanın maksimum menizlini belirtir.")]
    public float viewRadius;

    [Range(0, 360)]
    [Tooltip("Düşmanın görüş açısını belirtir.")]
    public float viewAngle;

    [Tooltip("Düşman Katmanı")]
    public LayerMask targetMask;

    [Tooltip("Engel Katmanları")]
    public LayerMask obstacleMask;

    [Tooltip("Düşman Obje")]
    public GameObject target;

    [Tooltip("Düşmanın Pozisyonu")]
    public Vector3 targetPosition;

    [Tooltip("Düşman menzil içerisinde")]
    public bool targetInViewRadius;

    [Tooltip("Düşman görüş açısı içerisinde")]
    public bool targetInFieldOfView;

    [Tooltip("Düşman tespit edildi")]
    public bool targetIsDetected;

    public float meshResolution = 3;
    public int edgeResolveIterations = 4;
    public float edgeDstThreshold = 0.5f;

    public MeshFilter viewMeshFilter;
    public Mesh viewMesh;
    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", .2f);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTarget();
        }
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void FindVisibleTarget()
    {
        if (target != null)
        {
            targetPosition = target.transform.position;
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;

            if (Vector3.Distance(transform.position, targetPosition) < viewRadius)
            {
                targetInViewRadius = true;
                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    targetInFieldOfView = true;
                    float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                    {
                        targetIsDetected = true;
                    }
                    else
                    {
                        targetIsDetected = false;
                    }
                }
                else
                {
                    targetInFieldOfView = false;
                    targetIsDetected = false;
                }
            }
            else
            {
                targetInViewRadius = false;
                targetInFieldOfView = false;
                targetIsDetected = false;
            }
        }
    }


    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
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
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
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
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
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

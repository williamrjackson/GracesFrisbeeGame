using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMechanic : MonoBehaviour
{
    [SerializeField]
    Transform shooter;
    [SerializeField]
    Transform target;
    [SerializeField]
    Transform frisbee;
    [SerializeField]
    LaserLine line;
    [SerializeField]
    float angleMultiplier = 1f;

    public Transform testTransform;

    float downX;
    Vector3[] curve;

    // Start is called before the first frame update
    private void Start()
    {
        GameManager.Instance.OnGameOver += GOListener;
    }
    void GOListener()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            downX = Input.mousePosition.x;
            GameManager.Instance.frisbee.eulerAngles = Vector3.zero;
            GameManager.Instance.frisbee.SetParent(transform);
            GameManager.Instance.frisbee.localPosition = Vector3.zero;
        }
        if (Input.GetMouseButton(0))
        {
            float angle = Wrj.Utils.Remap(Input.mousePosition.x - downX, -200f, 200f, -1f, 1f);
            Vector3 influence = Vector3.Lerp(shooter.position, target.position, .5f) + (Vector3.right * (angle * angleMultiplier));
            curve = Wrj.Utils.QuadraticBezierCurve(shooter.position, influence, target.position, 512);
            line.SetPositions(curve);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(MovePath(curve));
            line.SetPositions(new Vector3[0]);
        }
    }

    private IEnumerator MovePath(Vector3[] path)
    { 
        float elapsedTime = 0;
        float duration = .075f;
        duration = duration * GetCurveLength(path);
        while (elapsedTime < duration)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            float scrubPos = Wrj.Utils.Remap(elapsedTime, 0, duration, 0, 1);
            frisbee.position = GetPointOnCurve(path, Mathf.Lerp(0, 1, scrubPos));

            // Project frisbee on line directly between players; measure distance. Based on distance increase/decrease frisbee localEuler z
            Vector3 projection = Vector3.Project(frisbee.position - shooter.position, target.position - shooter.position);
            float frisbeeZAngle = Wrj.Utils.Remap(frisbee.position.x - projection.x, -angleMultiplier, angleMultiplier, -180f, 180f);
            Vector3 rotation = Vector3.zero.With(z: frisbeeZAngle);
            frisbee.localEulerAngles = rotation;
        }
    }
    public Vector3 GetPointOnCurve(Vector3[] curve, float t)
    {
        if (curve.Length < 2)
        {
            return Vector3.zero;
        }

        // Get total length of curve
        float posOnLine = GetCurveLength(curve) * t;

        for (int i = 0; i < curve.Length - 1; i++)
        {
            Vector3 p0 = curve[i];
            Vector3 p1 = curve[i + 1];
            float currentDistance = Vector3.Distance(p1, p0);

            // If the remaining distance is greater than the distance between these vectors, subtract the current distance and proceed
            if (currentDistance < posOnLine)
            {
                posOnLine -= currentDistance;
                continue;
            }
            return Vector3.Lerp(p0, p1, posOnLine / currentDistance);
        }
        // made it to the end
        return curve[curve.Length - 1];
    }
    public float GetCurveLength(Vector3[] curve)
    {
        if (curve.Length < 1)
            return 0;

        float length = 0f;
        for (int i = 0; i < curve.Length - 1; i++)
        {
            length += Vector3.Distance(curve[i + 1], curve[i]);
        }
        return length;
    }


}

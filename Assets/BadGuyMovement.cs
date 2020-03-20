using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadGuyMovement : MonoBehaviour
{
    public float horizRange = 5f;
    public float vertRange = 0f;

    public float duration = 3f;
    // Start is called before the first frame update
    void Start()
    {
        Wrj.Utils.MapToCurve.Ease.Move(transform, transform.localPosition + Vector3.right * horizRange + 
                                                                            Vector3.forward * vertRange, 
                                                                            duration, pingPong: int.MaxValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.gameObject.transform == GameManager.Instance.frisbee)
        {
            Debug.Log("Frisbee Confirmed");
            GameManager.Instance.frisbee.SetParent(transform);
            GameManager.Instance.Hit();
        }

    }
}

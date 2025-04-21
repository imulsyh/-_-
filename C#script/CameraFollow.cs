using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing;

    public Vector2 minPosition;
    public Vector2 maxPosition;
    // Start is called before the first frame update
    void Start()
    {
        GameController.camShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void LateUpdate() 
    {
        if(target != null)
        {
            if(transform.position != target.position)
            {
                Vector3 targetPos = target.position;
                Vector3 Xpos = transform.position;
                targetPos.x = Mathf.Clamp(targetPos.x,minPosition.x, maxPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y,minPosition.y, maxPosition.y);
                transform.position = Vector3.Lerp(Xpos,targetPos,smoothing);
            }
        }
    }
    public void SetCamPosLimit(Vector2 minPos, Vector2 maxPos)
    {
        minPosition = minPos;
        maxPosition = maxPos;
    }
}

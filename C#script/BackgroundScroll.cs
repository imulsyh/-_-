using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Range(-1f,1f)]
    public float scrollSpeed = 0.5f;
    private float offset;
    private Material mat;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        //cameraTransform = Camera.main.transform;
        //lastCameraPosition = cameraTransform.position;
        mat = GetComponent<Renderer>().material;
    }

    private void LateUpdate()
    {
        //Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        //transform.position += deltaMovement;

        offset += (Time.deltaTime * scrollSpeed) / 10f;
        mat.SetTextureOffset("_MainTex",new Vector2(offset,0));
    }
}

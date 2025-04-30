using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;




public class BlockController : MonoBehaviour
{
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 offset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (isDragging)
        {
            DragObject();
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        rb.isKinematic = false;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseUp()
    {
        isDragging = false;
        rb.isKinematic = true;
    }

    void DragObject()
    {
        Vector3 mouseWorldPos = GetMouseWorldPos() + offset;

        float snappedX = Mathf.Round(mouseWorldPos.x);
        float snappedY = transform.position.y;
        float snappedZ = Mathf.Round(mouseWorldPos.z);

        Vector3 snappedPosition = new Vector3(snappedX, snappedY, snappedZ);

       
        Vector3 moveDelta = snappedPosition - transform.position;

        bool canMove = true;

        foreach (Transform child in transform)
        {
            Vector3 childTargetPos = child.position + moveDelta;
            Vector3 halfExtents = child.localScale / 2f;

           
            Collider[] hits = Physics.OverlapBox(childTargetPos, halfExtents * 0.9f, Quaternion.identity);

            foreach (Collider hit in hits)
            {
                if (hit.transform.root != transform) 
                {
                    canMove = false;
                    break;
                }
            }

            if (!canMove) break;
        }

        if (canMove)
        {
            transform.position = snappedPosition;
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}


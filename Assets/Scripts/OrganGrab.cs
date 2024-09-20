using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class OrganGrab : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    GameObject putBackPrompt;

    [SerializeField]
    float distanceToCamera;
    [SerializeField]
    float grabSpeed;
    float grabLerp;
    [SerializeField]
    float rotateSpeed;
    [SerializeField]
    bool grabbed;

    Vector3 startPos;
    Vector3 grabbedPos;

    private void Start()
    {
        cam = Camera.main;        
        startPos = transform.position;
        if (putBackPrompt != null  && putBackPrompt.activeSelf)
        {
            putBackPrompt.SetActive(false);
        }
    }


    private void OnMouseDown()
    {
        if (cam != null && !GameManager.GetGameManager().organGrabbed && !GameManager.GetGameManager().checklistOpen)
        {
            if (GameManager.GetGameManager().inspecting && !grabbed)
            {
                grabbed = true;
                grabbedPos = cam.transform.position + Vector3.down * distanceToCamera;
                grabLerp = 0;
            }
        }
        else
        {
            if (cam == null)
            {
                Debug.Log(name + "could not find camera");
            }
        }
    }


    void InspectingOrgan()
    {
        if (grabLerp < 1)
        {
            grabLerp += grabSpeed * Time.fixedDeltaTime;
        }

        putBackPrompt.SetActive(true);

        transform.localPosition = Vector3.Lerp(startPos, grabbedPos, grabLerp);

        if (Input.GetKeyDown(KeyCode.E) && !GameManager.GetGameManager().checklistOpen)
        {
            transform.position = startPos;
            putBackPrompt.SetActive(false);
            grabbed = false;
            GameManager.GetGameManager().organGrabbed = false;
        }
    }
    private void Update()
    {
        if (grabbed && GameManager.GetGameManager().inspecting)
        {
            InspectingOrgan();
        }
        

        if (!GameManager.GetGameManager().inspecting)
        {
            grabbed = false;
            if (putBackPrompt.activeSelf)
            {
                putBackPrompt.SetActive(false);
            }
        }

        if (grabbed)
        {
            GameManager.GetGameManager().organGrabbed = true;
            if (Input.GetMouseButton(0) && !GameManager.GetGameManager().checklistOpen)
            {
                float xRotation = Input.GetAxis("Mouse X") * rotateSpeed * Time.fixedDeltaTime;
                transform.Rotate(Vector3.up, -xRotation);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(90, transform.rotation.y, transform.rotation.z);
        }
    }
}

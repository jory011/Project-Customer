using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganGrabManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam; // Camera reference
    [SerializeField]
    private List<GameObject> organs = new List<GameObject>(); // List of organ objects
    [SerializeField]
    private GameObject putBackPrompt;

    [SerializeField]
    private float distanceToCamera = 3f; // Distance to move organ towards camera
    [SerializeField]
    private float grabSpeed = 5f;
    [SerializeField]
    private float rotateSpeed = 100f;

    private float grabLerp = 0;
    private bool grabbed = false;
    private GameObject currentOrgan;
    private Vector3 startPos;
    private Quaternion startRotation; // Store the initial rotation
    private Vector3 grabbedPos;

    private GameManager gameManager; // Reference to GameManager

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main; // Assign main camera if not set
        }

        if (putBackPrompt != null)
        {
            putBackPrompt.SetActive(false); // Deactivate the prompt at the start
        }

        // Find and assign GameManager
        gameManager = GameManager.GetGameManager();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found.");
        }
    }

    private void Update()
    {
        if (gameManager != null && gameManager.inspecting && !gameManager.organGrabbed && Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform raycast
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;

                Debug.Log("Raycast hit object: " + hitObject.name);

                // Check if the hit object or any of its parents is an organ in the list
                foreach (GameObject organ in organs)
                {
                    if (hitObject == organ || hitObject.transform.IsChildOf(organ.transform))
                    {
                        Debug.Log("Organ detected: " + organ.name);

                        currentOrgan = organ; // Set the current organ
                        startPos = organ.transform.position;
                        startRotation = organ.transform.rotation; // Store the initial rotation
                        grabbedPos = cam.transform.position + cam.transform.forward * distanceToCamera;

                        grabbed = true;
                        grabLerp = 0;

                        putBackPrompt.SetActive(true); // Show prompt to put the organ back
                        gameManager.organGrabbed = true; // Set organGrabbed to true
                        break;
                    }
                }
            }
            else
            {
                // Debug: Raycast didn't hit anything
                Debug.Log("Raycast did not hit any object.");
            }
        }

        // If organ is grabbed, move and rotate it
        if (grabbed)
        {
            InspectingOrgan();
        }
    }

    private void InspectingOrgan()
    {
        // Move organ towards camera
        if (grabLerp < 1)
        {
            grabLerp += grabSpeed * Time.deltaTime;
            currentOrgan.transform.position = Vector3.Lerp(startPos, grabbedPos, grabLerp);
        }

        // Rotate organ based on mouse movement
        if (Input.GetMouseButton(0))
        {
            float xRotation = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            currentOrgan.transform.Rotate(Vector3.up, -xRotation);
        }

        // Return organ if "E" is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentOrgan.transform.position = startPos;
            currentOrgan.transform.rotation = startRotation; // Return to original rotation
            grabbed = false;
            currentOrgan = null;
            putBackPrompt.SetActive(false);
            gameManager.organGrabbed = false; // Reset organGrabbed
        }
    }
}

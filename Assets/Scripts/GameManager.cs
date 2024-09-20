using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class GameManager : MonoBehaviour
{
    public static GameManager GetGameManager()
    {
        return mainManager;
    }

    static GameManager mainManager = null;

    Camera cam;
    GameObject player;
    GameObject checklistUI;
    GameObject checklistPrompt;

    public bool inspecting;
    public bool organGrabbed;
    public bool checklistOpen;

    private void Awake()
    {
        if (mainManager == null)
        {
            DontDestroyOnLoad(gameObject);
            mainManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = Camera.main;
        checklistUI = GameObject.FindGameObjectWithTag("Checklist");
        checklistPrompt = GameObject.FindGameObjectWithTag("ListPrompt");

        if (checklistUI != null) 
        { 
           checklistUI.SetActive(false);
        } 
        else
        {
            Debug.LogWarning("GameManager did not find Checklist UI");
        }

        if (checklistPrompt != null)
        {
            checklistPrompt.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameManager could not find UI element with tag 'ListPrompt'");
        }
    }

    public void EnterInspection(Vector3 corpsePos, float camDistance)
    {
        if (cam != null && player != null)
        {
            inspecting = true;
            checklistPrompt.SetActive(true);
            cam.transform.position = corpsePos + Vector3.up * camDistance;
            cam.transform.LookAt(corpsePos);
            player.GetComponent<Renderer>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (cam == null) 
        {
            Debug.Log("GameManager did not find a camera");
        }
        if (player == null)
        {
            Debug.Log("GameManager did not find a GameObject with the tag 'Player'");
        }
    }

    public void InspectionInput()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            checklistPrompt.SetActive(false);
            inspecting = false;
            organGrabbed = false;
            player.GetComponent<Renderer>().enabled = true;
            cam.transform.position = player.transform.position + Vector3.up * 0.8f;
            Cursor.lockState = CursorLockMode.Locked;
            checklistUI.SetActive(false);
            checklistOpen = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (checklistUI.activeSelf)
            {
                checklistOpen = false;
                checklistUI.SetActive(false);
            }
            else
            {
                checklistOpen = true;
                checklistUI.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (inspecting) { 
          InspectionInput();
        }
    }
}

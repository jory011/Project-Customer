using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ChecklistText : MonoBehaviour, IPointerClickHandler
{
    TextMeshProUGUI textLine;

    void Start()
    {
        textLine = GetComponent<TextMeshProUGUI>();
        textLine.fontStyle = FontStyles.Normal;
        textLine.enabled = false;

        if (textLine == null)
        {
            Debug.LogWarning("Could not find " + name + "'s text component");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (GameManager.GetGameManager().checklistOpen)
        {
            if (textLine.fontStyle == FontStyles.Normal)
            {
                textLine.fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                textLine.fontStyle = FontStyles.Normal;
            }
        }  
    }

    private void Update()
    {
        if (GameManager.GetGameManager().checklistOpen)
        {
            textLine.enabled = true;
        }
        else
        {
            textLine.fontStyle = FontStyles.Normal;
            textLine.enabled = false;
        }

    }
}

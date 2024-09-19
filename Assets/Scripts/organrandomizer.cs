using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConditionManager : MonoBehaviour
{
    [SerializeField]
    private int numberOfConditionsToApply = 3;

    [SerializeField]
    private OrganCategory[] organCategories; // Array of OrganCategory for different types

    private Dictionary<OrganType, List<GameObject>> organDictionary;

    // List to keep track of applied conditions for later use
    private List<AppliedConditionLog> appliedConditionsLog = new List<AppliedConditionLog>();

    private void Start()
    {
        // Initialize the dictionary
        organDictionary = new Dictionary<OrganType, List<GameObject>>();

        // Populate the dictionary based on the OrganCategory array
        foreach (var category in organCategories)
        {
            organDictionary[category.OrganType] = new List<GameObject>(category.OrganObjects);
        }

        ApplyRandomConditions();
    }

  private void ApplyRandomConditions()
{
    // Load all condition objects from Resources
    ConditionObject[] allConditions = Resources.LoadAll<ConditionObject>("conditions");
    Debug.Log($"Loaded {allConditions.Length} conditions from Resources.");

    // Ensure there are enough conditions
    if (allConditions.Length < numberOfConditionsToApply)
    {
        Debug.LogError("Not enough conditions to apply.");
        return;
    }

    // Shuffle conditions to randomize selection
    allConditions = Shuffle(allConditions);
    
    // Track applied conditions and organ types
    HashSet<OrganType> assignedOrganTypes = new HashSet<OrganType>();
    HashSet<string> appliedConditionNames = new HashSet<string>();

    int conditionsApplied = 0; // Track how many conditions have been applied

    // Iterate over shuffled conditions
    foreach (var condition in allConditions)
    {
        OrganType organType = condition.OrganType;

        // Check if this organ type has already been assigned a condition or if the condition name is already applied
        if (assignedOrganTypes.Contains(organType) || appliedConditionNames.Contains(condition.ConditionName))
        {
            continue; // Skip this condition if already assigned to the organ type or condition name
        }

        List<GameObject> organsOfType = organDictionary.ContainsKey(organType) ? organDictionary[organType] : null;

        if (organsOfType != null && organsOfType.Count > 0)
        {
            // Apply condition to all GameObjects in this organ category
            foreach (var organ in organsOfType)
            {
                ApplyConditionToOrgan(condition, organ);

                // Log the applied condition along with the organ category
                appliedConditionsLog.Add(new AppliedConditionLog(organ.name, organType, condition));
            }

            // Mark this organ type as assigned and condition name as applied
            assignedOrganTypes.Add(organType);
            appliedConditionNames.Add(condition.ConditionName);
            conditionsApplied++;

            // Stop if we have applied the required number of conditions
            if (conditionsApplied >= numberOfConditionsToApply)
            {
                break; // Exit the loop once we have enough conditions
            }
        }
        else
        {
            Debug.LogWarning($"No organs available for condition type {organType}");
        }
    }

    // Log all applied conditions
    Debug.Log("Applied Conditions:");
    foreach (var log in appliedConditionsLog)
    {
        Debug.Log($"Organ: {log.OrganName}, Category: {log.OrganType}, Condition: {log.Condition.ConditionName}");
    }
}



    private void ApplyConditionToOrgan(ConditionObject condition, GameObject organ)
    {
        // Apply color and material to all child GameObjects with MeshRenderer
        ApplyColorAndMaterialToChildren(organ, condition);

        // Log application
        Debug.Log($"Applied condition: {condition.ConditionName} to organ: {organ.name}");
    }

   private void ApplyColorAndMaterialToChildren(GameObject organ, ConditionObject condition)
{
    // Retrieve the MeshRenderer components from the children
    MeshRenderer[] renderers = organ.GetComponentsInChildren<MeshRenderer>();

    foreach (var renderer in renderers)
    {
        // Set material if available
        if (condition.Material != null)
        {
            // Apply the specified material to all meshes
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = condition.Material;
            }
            renderer.materials = materials;
        }
        else
        {
            // Set color only if no material is assigned
            Color color = condition.SpriteColor;
            color.a = 1f; // Set alpha to 100%

            // Apply color to all materials of the MeshRenderer
            foreach (var material in renderer.materials)
            {
                material.color = color;
            }
        }
    }
}


    private T[] Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[rnd];
            array[rnd] = temp;
        }
        return array;
    }

    // Method to retrieve the applied conditions log
    public List<AppliedConditionLog> GetAppliedConditionsLog()
    {
        return appliedConditionsLog;
    }
}

// Class to store log data for applied conditions, including the organ category (OrganType)
[System.Serializable]
public class AppliedConditionLog
{
    public string OrganName;
    public OrganType OrganType; // Log the organ category
    public ConditionObject Condition;

    public AppliedConditionLog(string organName, OrganType organType, ConditionObject condition)
    {
        OrganName = organName;
        OrganType = organType;
        Condition = condition;
    }
}

[System.Serializable]
public class OrganCategory
{
    public OrganType OrganType;
    public GameObject[] OrganObjects;
}

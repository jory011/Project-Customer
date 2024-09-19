using UnityEngine;

public enum OrganType
{
    Lung,
    Heart,
    Stomach,
    Liver,
    Pancreas,
    LargeIntestine,
    AdrenalGland,
    Kidney
}

[CreateAssetMenu(fileName = "NewConditionObject", menuName = "ScriptableObjects/ConditionObject", order = 1)]

public class ConditionObject : ScriptableObject
{
    [SerializeField]
    private Color spriteColor;

    [SerializeField]
    private Material material;

    [SerializeField]
    private string conditionName;

    [SerializeField]
    private OrganType organType;

    [SerializeField]
    private bool isDefaultState;

    [SerializeField]
    private bool hasCancer;

    public Color SpriteColor => spriteColor;
    public string ConditionName => isDefaultState ? "Default" : conditionName;
    public OrganType OrganType => organType;
    public bool IsDefaultState => isDefaultState;
    public bool HasCancer => hasCancer;
    public Material Material => material;
    
}

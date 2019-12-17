using UnityEngine;

[System.Serializable]
public struct VariableMaterialMeshData
{
    public MeshRenderer meshRenderer;
    public Material explorationPhaseMaterial;
    public Material investigationPhaseMaterial;
}

[System.Serializable]
public class ObjectStateController
{
    [SerializeField] VariableMaterialMeshData[] variableMaterialMeshesData = default;

    public ObjectStateController()
    {

    }

    public void SetExplorationVariableMeshesMaterials()
    {
        for (int i = 0; i < variableMaterialMeshesData.Length; i++)
            variableMaterialMeshesData[i].meshRenderer.material = variableMaterialMeshesData[i].explorationPhaseMaterial;
    }

    public void SetInvestigationVariableMeshesMaterials()
    {
        for (int i = 0; i < variableMaterialMeshesData.Length; i++)
            variableMaterialMeshesData[i].meshRenderer.material = variableMaterialMeshesData[i].investigationPhaseMaterial;
    }
}
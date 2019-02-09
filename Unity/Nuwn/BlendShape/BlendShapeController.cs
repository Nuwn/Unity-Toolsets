using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nuwn.Essentials;
using Nuwn.Extensions;
using System;


public class BlendShapeController : MonoBehaviour
{   
    public List<BlendShape> BlendShapes = new List<BlendShape>();

    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    
    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }

    private void Start()
    {
        foreach (var bs in BlendShapes)
        {
            bs.weightChanged += WeightISChanged;
        }

        this.SetInterval(TestWeight, 500);

    }
    void WeightISChanged(int index, float f)
    {
        skinnedMeshRenderer.SetBlendShapeWeight(index, f);
    }
    void TestWeight()
    {
        foreach (var bs in BlendShapes)
        {
            bs.Weight = UnityEngine.Random.Range(0f, 101f);
        }
    }


    public void PopulateListWithBlendShapes()
    {
        var skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        var skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        BlendShapes.Clear();

        for(var i = 0; i < skinnedMesh.blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, 0);
            BlendShapes.Add(new BlendShape() { Name = skinnedMesh.GetBlendShapeName(i), Index = 0, Weight = 0 });
        }
    }
}

public delegate void WeightChanged(int index, float f);
[Serializable]
public class BlendShape
{
    public event WeightChanged weightChanged;

    public string Name;
    public int Index;
    [Header("Read only.")]
    public float _weight;
    public float Weight
    {
        set
        {
            if (_weight == value)
                return;

            _weight = value;
            weightChanged.Invoke(Index, value);
        }
    }
}

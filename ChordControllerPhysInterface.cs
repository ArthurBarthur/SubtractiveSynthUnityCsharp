using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChordControllerPhysInterface : MonoBehaviour
{
    public List<MeshFilter> meshFilters;
    // Start is called before the first frame update
    void Start()
    {
        foreach (ChordSpawner.ChordSelector item in Enum.GetValues(typeof(ChordSpawner.ChordSelector)))
        {
            meshFilters.Add(gameObject.AddComponent<MeshFilter>( ));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

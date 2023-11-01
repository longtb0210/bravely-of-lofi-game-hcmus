using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefabs;
    [SerializeField] private float cloneDuration;


    public void CreateClone(Transform _clonePosition)
    {
        GameObject newClone = Instantiate(clonePrefabs);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration);
    }
}

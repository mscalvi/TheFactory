using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyState
{
    // Constructions
    public Dictionary<string, ConstructionInstance> CompanyConstructions = new Dictionary<string, ConstructionInstance>();

    public List<ConstructionInstance> ActiveConstructions = new List<ConstructionInstance>();

    public Queue<ConstructionInstance> ConstructionsQueue = new Queue<ConstructionInstance>();

    public int MaxConstructionsSlots = 1;
    public int MaxConstructionsQueue = 0;
    public float ConstructionTime = 0;
}

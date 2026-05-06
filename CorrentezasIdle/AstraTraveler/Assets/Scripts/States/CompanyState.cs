using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyState
{
    // Acquisitons
    public Dictionary<string, AcquisitionInstance> CompanyAcquisitions = new Dictionary<string, AcquisitionInstance>();

    public List<AcquisitionInstance> ActiveAcquisitons = new List<AcquisitionInstance>();

    public Queue<AcquisitionInstance> AcquisitionsQueue = new Queue<AcquisitionInstance>();

    public int MaxAcquisitionsSlots = 1;
    public int MaxAcquisitonsQueue = 0;
    public float AcquistionTime = 0;
}

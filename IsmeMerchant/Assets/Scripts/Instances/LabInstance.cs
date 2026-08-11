using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabInstance
{
    public LabModel Model;

    public string Id;
    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public bool Note;

    public AlchemyHelper.LabType Type;

    public int Level;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public LabInstance(LabModel model)
    {
        Id = model.Id;

        NameEN = model.NameEN;
        NamePT = model.NamePT;
        DescriptionPT = model.DescriptionPT;
        DescriptionEN = model.DescriptionEN;

        Note = false;

        Type = model.Type;

        Level = model.Level;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }
    public LabInstance()
    {

    }
}

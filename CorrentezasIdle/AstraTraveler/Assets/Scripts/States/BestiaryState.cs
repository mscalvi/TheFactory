using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestiaryEntry
{
    public int KilledExpedition;
    public int KilledLastExpedition;
    public int KilledTotal;

    public BestiaryEntry()
    {
        KilledExpedition = 0;
        KilledLastExpedition = 0;
        KilledTotal = 0;
    }
}

public class BestiaryState
{
    public Dictionary<string, BestiaryEntry> Bestiary = new Dictionary<string, BestiaryEntry>();
}

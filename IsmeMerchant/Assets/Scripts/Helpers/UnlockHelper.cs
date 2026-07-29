using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockHelper
{
    public enum UnlockStatus
    {
        Unknow,     // Não visto ainda
        Blocked,    // Visto, mas indisponível
        Available,  // Visto e disponível
        Unlocked,   // Completamente Comprado - Uso Único
        Deleted,    // Dados obsoletos
        Finished,   // Completamente Comprado
    }

    public enum UnlockTrigger
    {
        Start,
        Empty,
        FirstWeapon,
        FirstShip,
        FirstAmmo,
    }
}

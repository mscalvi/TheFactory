using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public int Width = 20;
    public int Height = 12;

    public IReadOnlyList<string> Layout => layout;

    private readonly string[] layout =
    {
        "GGGGGGGGGGGGGGGGGGGG",
        "GGGGPPPPPPPPPPPPGGGG",
        "GGGGPGGGGGGGGGGPGGGG",
        "SPPPPGGGGGGGGGGPPPPG",
        "GGGGGGGGGGGGGGGGGGPG",
        "GGGGGGGGGGGGGGGGGGPG",
        "GGGPPPPPPPPPPPPPPPPG",
        "GGGPGGGGGGGGGGGGGGGG",
        "GGGPGGGPPPPPPPPPPPPG",
        "GGGPGGGPGGGGGGGGGGPG",
        "GGGPPPPPGGGGGGGGGGPG",
        "GGGGGGGGGGGGGGGGGGTG"
    };
}

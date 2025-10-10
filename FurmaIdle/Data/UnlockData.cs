public static class UnlockData
{
    public static readonly List<UnlockRule> Rules = new()
    {
        // DESTINOS
        new UnlockRule {
            TargetType = "dest", TargetId = "d01",
            When = ctx => ctx.ResourceTotal("r001") >= 1000   // total de moedas
        },
        new UnlockRule {
            TargetType = "dest", TargetId = "d02",
            When = ctx => ctx.StageUnlocked("s00") && ctx.ResourceTotal("r001") >= 5000
        },

        // TECNOLOGIAS
        new UnlockRule {
            TargetType = "tech", TargetId = "t01",
            When = ctx => ctx.StageUnlocked("s00") && ctx.ResourceTotal("r002") >= 50 // ex.: conhecimento
        },

        // UPGRADES (apenas “aparecer” na loja; compra continua na BuyUpgrade)
        new UnlockRule {
            TargetType = "upgrade", TargetId = "u001",
            When = ctx => ctx.TechUnlocked("t01") // gating por tecnologia
        },
        new UnlockRule {
            TargetType = "upgrade", TargetId = "mx00", // permanente (MX)
            When = ctx => ctx.ResourceTotal("r001") >= 2000
        },

        // STAGES
        new UnlockRule {
            TargetType = "stage", TargetId = "s01",
            When = ctx => ctx.StageUnlocked("s00") && ctx.UpgradeBuys("u001") >= 3
        },
    };
}

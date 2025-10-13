namespace FurmaIdle.Helpers
{
    public class UpgradeCostEnum
    {
        public enum UpgradeCostCode
        {
            Geracao1R1,
            Geracao1T1,
            LimiteContrato1T1,
            Quantidade1C1,
            Quantidade1C2,
            Quantidade1C3,
            Quantidade1C4,
            Quantidade1C5,
            Quantidade1T1,
            Tempo1C1,
            Tempo1C2,
            Tempo1C3,
            Tempo1C4,
            Clicks1T1,
            Teste,
            UnlockResource,
            UnlockStage,
            PartyCap
        }

        public static class UpgradeCostMap
        {
            // (resourceId, base, growth)
            private static readonly Dictionary<UpgradeCostCode, (string res, double @base, double growth)> _map = new()
            {
                [UpgradeCostCode.Quantidade1C1] = ("r001", 1000, 1.7),
                [UpgradeCostCode.Quantidade1C2] = ("r001", 3000, 1.7),
                [UpgradeCostCode.Quantidade1C3] = ("r001", 15000, 1.7),
                [UpgradeCostCode.Quantidade1C4] = ("r001", 110000, 2.5),
                [UpgradeCostCode.Quantidade1C5] = ("r001", 720000, 2.5),
                [UpgradeCostCode.Quantidade1T1] = ("r001", 13000, 2.2),
                [UpgradeCostCode.Tempo1C1] = ("r001", 2000, 1.7),
                [UpgradeCostCode.Tempo1C2] = ("r001", 3000, 1.7),
                [UpgradeCostCode.Tempo1C3] = ("r001", 30000, 1.7),
                [UpgradeCostCode.Tempo1C4] = ("r001", 220000, 2.5),
                [UpgradeCostCode.Geracao1T1] = ("r001", 25000, 2.2),
                [UpgradeCostCode.Geracao1R1] = ("r001", 8000, 1.5),
                [UpgradeCostCode.LimiteContrato1T1] = ("r001", 100, 1.2),
                [UpgradeCostCode.Clicks1T1] = ("r001", 150, 3.3),
                [UpgradeCostCode.Teste] = ("r001", 1, 1),
                [UpgradeCostCode.UnlockResource] = ("r001", 2000, 1),
                [UpgradeCostCode.UnlockStage] = ("r001", 20000000, 1),
                [UpgradeCostCode.PartyCap] = ("r001", 1, 1)
            };

            public static (string res, double @base, double growth) Get(UpgradeCostCode code) => _map[code];

            public static bool TryGet(UpgradeCostCode code, out (string res, double @base, double growth) tuple)
                => _map.TryGetValue(code, out tuple);
        }
    }
}

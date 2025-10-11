using System.Collections.Generic;
using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class CharacterData
    {
        public static int SchemaVersion => 1;

        // ordem de exibição/seed
        public static readonly List<string> Order = new() { "p00", "p01", "p02", "p03", "p04", "p05" };

        // catálogo IMUTÁVEL (não use em runtime diretamente)
        internal static readonly Dictionary<string, CharacterModel> All = new()
        {
            #region s00
            ["p00"] = new CharacterModel
            {
                Id = "p00",
                Name = "Ferri Karu",
                MainKnowId = "k11",
                SecondKnowId = "k10",
                KnowContractsIds = new() { "c10", "c20", "c30" },
                UnknowContractsIds = new() { "c40" },
                SpecialtyId = "e01",
                TraitId = "tr00",
                Sort = 1,
                Unlocked = true,
                CharState = CharStateEnum.CharState.InBase,
                CharDestId = "d00",
                Avaliable = false,
                Image = "images/icons/characters/p00.jpg",
                BigImage = "images/characters/p00.jpg",
                FullImage = "images/pictures/p00.jpg",
                MaxContracts = 3
            },
            ["p01"] = new CharacterModel
            {
                Id = "p01",
                Name = "Maik Monhang",
                MainKnowId = "k10",
                SecondKnowId = "k12",
                KnowContractsIds = new() { "c10", "c20", "c31" },
                UnknowContractsIds = new() { "c41" },
                SpecialtyId = "e02",
                Sort = 2,
                Unlocked = true,
                CharState = CharStateEnum.CharState.InBase,
                CharDestId = "d00",
                Avaliable = false,
                TraitId = "tr01",
                Image = "images/icons/characters/p01.jpg",
                BigImage = "images/characters/p01.jpg",
                FullImage = "images/pictures/p01.jpg",
                MaxContracts = 3
            },
            ["p02"] = new CharacterModel
            {
                Id = "p02",
                Name = "Claimi Eky",
                MainKnowId = "k12",
                SecondKnowId = "k11",
                KnowContractsIds = new() { "c11", "c21", "c30" },
                UnknowContractsIds = new() { "c42" },
                SpecialtyId = "e00",
                Sort = 3,
                Unlocked = true,
                CharState = CharStateEnum.CharState.InBase,
                CharDestId = "d00",
                Avaliable = false,
                TraitId = "tr04",
                Image = "images/icons/characters/p02.jpg",
                BigImage = "images/characters/p02.jpg",
                FullImage = "images/pictures/p02.jpg",
                MaxContracts = 3
            },
            ["p03"] = new CharacterModel
            {
                Id = "p03",
                Name = "Alan Nhengar",
                MainKnowId = "k10",
                SecondKnowId = "k10",
                KnowContractsIds = new() { "c10", "c22", "c33" },
                UnknowContractsIds = new() { "c43" },
                SpecialtyId = "e02",
                Sort = 4,
                Unlocked = false,
                CharState = CharStateEnum.CharState.Locked,
                CharDestId = "d00",
                Avaliable = true,
                TraitId = "tr03",
                Image = "images/icons/characters/p03.jpg",
                BigImage = "images/characters/p03.jpg",
                FullImage = "images/pictures/p03.jpg",
                MaxContracts = 3,
                Cost = 5000,
                CostResourceId = "r001"
            },
            ["p04"] = new CharacterModel
            {
                Id = "p04",
                Name = "Jaime Boor",
                MainKnowId = "k11",
                SecondKnowId = "k20",
                KnowContractsIds = new() { "c11", "c21", "c31" },
                UnknowContractsIds = new() { "c44" },
                SpecialtyId = "e03",
                Sort = 5,
                Unlocked = false,
                CharState = CharStateEnum.CharState.Locked,
                CharDestId = "d01",
                Avaliable = false,
                TraitId = "tr02",
                Image = "images/icons/characters/p04.jpg",
                BigImage = "images/characters/p04.jpg",
                FullImage = "images/pictures/p04.jpg",
                MaxContracts = 3,
                Cost = 8000,
                CostResourceId = "r001"
            },
            ["p05"] = new CharacterModel
            {
                Id = "p05",
                Name = "Yg Iepora",
                MainKnowId = "k12",
                SecondKnowId = "k12",
                KnowContractsIds = new() { "c10", "c21", "c32" },
                UnknowContractsIds = new() { "c44" },
                SpecialtyId = "e00",
                Sort = 6,
                Unlocked = false,
                CharState = CharStateEnum.CharState.Locked,
                CharDestId = "d02",
                Avaliable = false,
                TraitId = "tr04",
                Image = "images/icons/characters/p05.jpg",
                BigImage = "images/characters/p05.jpg",
                FullImage = "images/pictures/p05.jpg",
                MaxContracts = 3,
                Cost = 20000,
                CostResourceId = "r001"
            }
            #endregion
        };

        public static CharacterModel GetDef(string id)
        {
            var chara = All[id];
            return new CharacterModel
            {
                Id = chara.Id,
                Name = chara.Name,
                MainKnowId = chara.MainKnowId,
                SecondKnowId = chara.SecondKnowId,
                KnowContractsIds = new List<string>(chara.KnowContractsIds),
                UnknowContractsIds = new List<string>(chara.UnknowContractsIds),
                SpecialtyId = chara.SpecialtyId,
                Sort = chara.Sort,
                Unlocked = chara.Unlocked,
                CharState = chara.CharState,
                CharDestId = chara.CharDestId,
                Avaliable = chara.Avaliable,
                TraitId = chara.TraitId,
                Image = chara.Image,
                BigImage = chara.BigImage,
                FullImage = chara.FullImage,
                MaxContracts = chara.MaxContracts,
                Cost = chara.Cost,
                CostResourceId = chara.CostResourceId,
            };
        }

        public static Dictionary<string, CharacterModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, CharacterModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var chara)) continue;

                dict[id] = new CharacterModel
                {
                    Id = chara.Id,
                    Name = chara.Name,
                    MainKnowId = chara.MainKnowId,
                    SecondKnowId = chara.SecondKnowId,
                    KnowContractsIds = new List<string>(chara.KnowContractsIds),
                    UnknowContractsIds = new List<string>(chara.UnknowContractsIds),
                    SpecialtyId = chara.SpecialtyId,
                    Sort = chara.Sort,
                    Unlocked = chara.Unlocked,
                    CharState = chara.Unlocked
                        ? CharStateEnum.CharState.InBase
                        : CharStateEnum.CharState.Locked,
                    CharDestId = chara.CharDestId,
                    Avaliable = chara.Avaliable,
                    TraitId = chara.TraitId,
                    Image = chara.Image,
                    BigImage = chara.BigImage,
                    FullImage = chara.FullImage,
                    MaxContracts = chara.MaxContracts,
                    Cost = chara.Cost,
                    CostResourceId = chara.CostResourceId,
                };
            }
            return dict;
        }
    }
}

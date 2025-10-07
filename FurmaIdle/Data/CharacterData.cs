// Data/CharacterData.cs
using System.Collections.Generic;
using FurmaIdle.Enums;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class CharacterData
    {
        // ordem de exibição/seed
        public static readonly List<string> Order = new() { "p00", "p01", "p02", "p03", "p04", "p05" };

        // catálogo IMUTÁVEL (não use em runtime diretamente)
        internal static readonly Dictionary<string, CharacterModel> All = new()
        {
            ["p00"] = new CharacterModel
            {
                Id = "p00",
                Name = "Ferri Karu",
                MainKnowId = "k11",
                SecondKnowId = "k10",
                KnowContractsIds = new() { "c10", "c20", "c30" },
                UnknowContractsIds = new() { "c40" },
                SpecialtyId = "e01",
                Sort = 1,
                StartUnlocked = true,
                CharState = CharStateEnum.CharState.InBase,
                CharStageId = null,
                Image = "images/icons/characters/p00.jpg",
                BigImage = "images/characters/p00.jpg",
                FullImage = "images/pictures/p00.jpg"
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
                StartUnlocked = true,
                CharState = CharStateEnum.CharState.InBase,
                CharStageId = null,
                Image = "images/icons/characters/p01.jpg",
                BigImage = "images/characters/p01.jpg",
                FullImage = "images/pictures/p01.jpg"
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
                StartUnlocked = true,
                CharState = CharStateEnum.CharState.InBase,
                CharStageId = null,
                Image = "images/icons/characters/p02.jpg",
                BigImage = "images/characters/p02.jpg",
                FullImage = "images/pictures/p02.jpg"
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
                StartUnlocked = false,
                CharState = CharStateEnum.CharState.Locked,
                CharStageId = null,
                Image = "images/icons/characters/p03.jpg",
                BigImage = "images/characters/p03.jpg",
                FullImage = "images/pictures/p03.jpg"
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
                StartUnlocked = false,
                CharState = CharStateEnum.CharState.Locked,
                CharStageId = null,
                Image = "images/icons/characters/p04.jpg",
                BigImage = "images/characters/p04.jpg",
                FullImage = "images/pictures/p04.jpg"
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
                StartUnlocked = false,
                CharState = CharStateEnum.CharState.Locked,
                CharStageId = null,
                Image = "images/icons/characters/p05.jpg",
                BigImage = "images/characters/p05.jpg",
                FullImage = "images/pictures/p05.jpg"
            }
        };

        public static CharacterModel GetDef(string id)
        {
            var d = All[id];
            return new CharacterModel
            {
                Id = d.Id,
                Name = d.Name,
                MainKnowId = d.MainKnowId,
                SecondKnowId = d.SecondKnowId,
                KnowContractsIds = new List<string>(d.KnowContractsIds),
                UnknowContractsIds = new List<string>(d.UnknowContractsIds),
                SpecialtyId = d.SpecialtyId,
                Sort = d.Sort,
                StartUnlocked = d.StartUnlocked,
                CharState = d.CharState,
                CharStageId = d.CharStageId,
                Image = d.Image,
                BigImage = d.BigImage,
                FullImage = d.FullImage
            };
        }

        public static Dictionary<string, CharacterModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, CharacterModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var def)) continue;

                dict[id] = new CharacterModel
                {
                    Id = def.Id,
                    Name = def.Name,
                    MainKnowId = def.MainKnowId,
                    SecondKnowId = def.SecondKnowId,
                    KnowContractsIds = new List<string>(def.KnowContractsIds),
                    UnknowContractsIds = new List<string>(def.UnknowContractsIds),
                    SpecialtyId = def.SpecialtyId,
                    Sort = def.Sort,
                    StartUnlocked = def.StartUnlocked,
                    CharState = def.StartUnlocked
                        ? CharStateEnum.CharState.InBase
                        : CharStateEnum.CharState.Locked,
                    CharStageId = def.CharStageId,
                    Image = def.Image,
                    BigImage = def.BigImage,
                    FullImage = def.FullImage
                };
            }
            return dict;
        }
    }
}

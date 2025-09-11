using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorCartas___Guildas.Models
{
    internal class CharacterModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Order {  get; set; }
        public string Class { get; set; }
        public string Trait { get; set; }
        public string Faction { get; set; }
        public int Health { get; set; }
        public int Atack { get; set; }
        public string Resistence { get; set; }
        public string Damage { get; set; }
        public int Bravery { get; set; }
        public string Hab1 { get; set; }
        public string Hab2 { get; set; }
        public bool HasPrep { get; set; }
        public int Prep { get; set; }
        public string Art {  get; set; }
        public string Lore { get; set; }
        public string Credits { get; set; }
        public string Info { get; set; }
        public string Edition { get; set; }

        //Auxiliares
        public int Body { get; set; }
        public int Strength { get; set; }
        public string Description { get; set; }
    }
}

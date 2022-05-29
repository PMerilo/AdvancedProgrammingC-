using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace PokemonPocket
{   public class PokePocket: DbContext
    {
        public DbSet<Pokemon> Pokemons {get; set;}
        public DbSet<Pokeball> Pokeballs {get; set;}
        public DbSet<Pity> Pity {get; set;}

        public string dbpath {get; set;}

        public PokePocket()
        {
            var path = Directory.GetCurrentDirectory();
            dbpath = System.IO.Path.Join(path, "PokePocket.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) 
            => options.UseSqlite($"Data Source={dbpath}");


    }
    public class PokemonMaster
    {
        public string Name { get; set; }
        public int NoToEvolve { get; set; }
        public string EvolveTo { get; set; }

        public PokemonMaster(string name, int noToEvolve, string evolveTo)
        {
            this.Name = name;
            this.NoToEvolve = noToEvolve;
            this.EvolveTo = evolveTo;
        }
    }

    public class Pokemon
    {
        public int Id {get; set;}
        public string Name { get; set; }
        public int Hp {get; set;}
        public int Exp {get; set;}
        public string Skill {get; set;}
        public bool Shiny {get; set;}

        public Pokemon(string name, int hp, int exp, bool shiny = false)
        {
            this.Name = name;
            this.Hp = hp;
            this.Exp = exp;
            this.Skill = "";
            this.Shiny = shiny;
        }
    }

    public class Charmander : Pokemon
    {
        public Charmander(string name, int hp, int exp, bool shiny = false) : base (name, hp, exp, shiny)
        {
            this.Skill = "Solar Power";
        }



    }

    public class Eevee : Pokemon
    {
        public Eevee(string name, int hp, int exp, bool shiny = false) : base (name, hp, exp, shiny)
        {
            this.Skill = "Run Away";
        }
        
    }

    public class Pikachu : Pokemon
    {
        public Pikachu(string name, int hp, int exp, bool shiny = false) : base (name, hp, exp, shiny)
        {
            this.Skill = "Lightning Bolt";
        }
    }

    public class WildPokemon : Pokemon
    {
        public int Tries {get; set;}
        public WildPokemon(string name, int hp, int exp) : base (name, hp, exp)
        {
            
            this.Tries = new Random().Next(5);
        }
    }
}


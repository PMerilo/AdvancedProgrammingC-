using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace PokemonPocket
{
    
    public class Pokeball
    {
        public int Id {get; set;}
        public string Name {get; set;}

        public int Count {get; set;}

        public void addBall(int number = 1)
        {
            this.Count += number;
        }
        
        public virtual bool useBall(WildPokemon pokemon)
        {
            this.Count--;
            var pokeballsMaster = new Dictionary<string, double>()
            {
                {"Poke Ball", 0.4},
                {"Great Ball", 0.6},
                {"Ultra Ball", 0.8},
                {"Master Ball", 1},

            };

            var rand = new Random();
            if (rand.NextDouble() <= pokeballsMaster[this.Name])
            {
                return true;
            } else {
                Console.WriteLine($"The {pokemon.Name} evades capture!");
                pokemon.Tries--;
                if (pokemon.Tries == 0)
                {
                    Console.WriteLine($"The {pokemon.Name} runs away!");
                }
                return false;
            }
        }

        public Pokeball(string name)
        {
            this.Name = name;
            this.Count = 0;
        }
    }

    public class Pity
    {
        public int Id {get; set;}
        public double Value {get; set;}

        public Pity()
        {
            this.Value = 0.1;
        }
        public void Reset()
        {
            this.Value = 0.1;
        }

        public void Add()
        {
            this.Value += 0.15;
        }
    }
}
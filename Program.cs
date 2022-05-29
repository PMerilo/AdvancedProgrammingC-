using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace PokemonPocket
{
    class Program
    {
        static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (input is String && input.Length > 0)
                {
                    return input;
                } else
                {
                    Console.WriteLine("Invalid Input: Input must be a string");
                    
                    
                }
            }
        }
        static int ReadInt(string prompt)
        {
            while (true)
            {
                var input = ReadString(prompt);
                int output;
                if (int.TryParse(input, out output))
                {
                    return output;
                } else
                {
                    Console.WriteLine("Invalid Input: Input must be a number");
                    
                }
            }
        }

        static string ReadOptions(string prompt, List<string> options)
        {
            while (true)
            {
                string input = ReadString(prompt);
                
                options = options
                        .Select(p => p.ToLower())
                        .ToList();

                if (options.Contains(input.ToLower())) {
                    return input;
                } else {
                    Console.WriteLine($"Invalid Option: Please only input one of the following options ({String.Join(',', options)})");
                }
            }
        }
        static int ReadOptionsInt(string prompt, List<int> options)
        {
            while (true)
            {
                int input = ReadInt(prompt);
                

                if (options.Contains(input)) {
                    return input;
                } else {
                    Console.WriteLine($"Invalid Option: Please only input one of the following options ({String.Join(',', options)})");
                }
            }
        }
        static string UserInput()
        {
            Console.WriteLine("*****************************");
            Console.WriteLine("Welcome to Pokemon Pocket App");
            Console.WriteLine("*****************************");
            Console.WriteLine("(1). Add Pokemon to Pocket");
            Console.WriteLine("(2). List Pokemon(s) in Pocket");
            Console.WriteLine("(3). Check if I can evolve Pokemon");
            Console.WriteLine("(4). Evolve Pokemon");
            Console.WriteLine("(5). List Pokeballs");
            Console.WriteLine("(6). Find & Capture a Pokemon");
            var options = new List<string>(){"1","2","3","4","5","6","q"};
            string userInput = ReadOptions($"Please only enter [1, 2, 3, 4, 5, 6] or Q to quit: ", options);
            return userInput.ToLower();
        }

        
        static bool CheckInMaster(string input, List<PokemonMaster> pokemonMasters)
        {
            var valid = pokemonMasters
                        .Select(p => p.Name.ToLower())
                        .ToList();
            
            return valid.Contains(input.ToLower());
        }
        static void AddPokemon(PokePocket db, List<PokemonMaster> pokemonMasters)
        {   
            
            string name;
            while (true)
            {
                name = ReadString("Enter Pokemons's Name: ");
                if (CheckInMaster(name, pokemonMasters))
                {
                    break;
                } else
                {
                    Console.WriteLine("Not a valid Pokemon in PokeDex. Try Again!");
                    
                }
            }

            int hp = ReadInt("Enter Pokemons's Hp: ");
            int exp = ReadInt("Enter Pokemons's Exp: ");

            if (name.ToLower() == "charmander")
            {
                var newPoke = new Charmander(name, hp, exp);
                db.Add(newPoke);
            } else if (name.ToLower() == "eevee")
            {
                var newPoke = new Eevee(name, hp, exp);
                db.Add(newPoke);
            } else if (name.ToLower() == "pikachu")
            {
                var newPoke = new Pikachu(name, hp, exp);
                db.Add(newPoke);
            }
            RewardBalls(db, 1, 11);
            db.SaveChanges();
            
        } 


        static void ListPokes(List<Pokemon> pokemons, bool id = true)
        {
            pokemons = pokemons
                    .OrderBy(obj => obj.Hp)
                    .ToList();
            foreach (var obj in pokemons)
            {
                if (obj.Shiny)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                if (id)
                {
                    Console.WriteLine($"---------ID: {obj.Id}-------------");
                } else {

                    Console.WriteLine($"--------------------------");
                }
                Console.WriteLine($"Name: {obj.Name}");
                Console.WriteLine($"Hp: {obj.Hp}");
                Console.WriteLine($"Exp: {obj.Exp}");
                Console.WriteLine($"Skill: {obj.Skill}");
                Console.WriteLine("--------------------------");
                Console.ResetColor();
            }
        }
        static void ListPokeballs(DbSet<Pokeball> pokeballs)
        {
            
            foreach (var ball in pokeballs)
            {
                Console.WriteLine($"{ball.Id}. {ball.Name}: {ball.Count}");
            }
        }

        static List<Pokemon> CheckEvolve(PokePocket db, List<PokemonMaster> pokemonMasters)
        {
            var i = db.Pokemons
                    .OrderBy(obj => obj.Name);
            
            var evolvables = new List<Pokemon>();
            var PokeCount = new Dictionary<string, int>();

            foreach (var obj in pokemonMasters)
            {
                PokeCount.Add(obj.Name.ToLower(), 0);
            }

            foreach (var obj in i)
            {   
                if (PokeCount.Keys.Contains(obj.Name.ToLower()))
                {
                    PokeCount[obj.Name.ToLower()] += 1;
                }
            }

            

            foreach (var obj in pokemonMasters)
            {
                if (PokeCount[obj.Name.ToLower()] >= obj.NoToEvolve)
                {
                    foreach (var objP in i)
                    {
                        if (objP.Name.ToLower() == obj.Name.ToLower())
                        {
                            evolvables.Add(objP);
                        }
                    }

                    Console.WriteLine($"{obj.Name} -------> {obj.EvolveTo}");

                } 
            }

            if (evolvables.Count() == 0)
            {
                Console.WriteLine("No Pokemon can be evolved!");
            }
            return evolvables;

        }

        static void EvolvePoke(PokePocket db, List<PokemonMaster> pokemonMasters)
        {

            var evolvables = CheckEvolve(db, pokemonMasters);
            var evos = evolvables
                    .Select(obj => obj.Name.ToLower())
                    .Distinct()
                    .ToList();

            if (evos.Count == 0){return;}

            var i = db.Pokemons
                    .OrderBy(obj => obj.Id);            
            
            string evo = ReadOptions($"Select an Evolution ({String.Join(',', evos)}): ", evos).ToLower();

            evolvables = evolvables
                        .Where(obj => obj.Name.ToLower() == evo)
                        .ToList();

            ListPokes(evolvables);
            int evolverID = ReadOptionsInt("Which pokemon do u want to evolve? (Select ID): ", evolvables.Select(p => p.Id).ToList());

            
            var evolver = evolvables
                        .Where(obj => obj.Id == evolverID)
                        .First();

            evolvables.Remove(evolver);

            var evoRequirement = pokemonMasters
                                .Where(obj => obj.Name.ToLower() == evo)
                                .First()
                                .NoToEvolve-1;

            var evolvees = new List<Pokemon>();
            if (evolvables.Count() > evoRequirement)
            {
                ListPokes(evolvables);

                for (int x = 0; x < evoRequirement; x++)
                {
                    
                    int evolveeID = ReadOptionsInt($"Select No.{x} Pokemon to Consume (Select ID): ", evolvables.Select(p => p.Id).ToList());
                    evolvees.Add(evolvables.Single(obj => obj.Id == evolveeID));
                }

            } else
            {
                evolvees = evolvables;
            }

            evolver.Name = pokemonMasters
                        .Single(obj => obj.Name.ToLower() == evolver.Name.ToLower())
                        .EvolveTo;
            evolver.Hp = 0;
            evolver.Exp = 0;

            for (int x = 0; x < evoRequirement; x++)
            {

                db.Remove(i.ToList()[x]);

            }
            
            Console.WriteLine($"{evo} has evolved to {evolver.Name}");
            ListPokes(new List<Pokemon>() {evolver});
            RewardBalls(db, 5, 16);
            db.SaveChanges();

        }
        static WildPokemon CreateWildPokemon(List<PokemonMaster> pokemonMasters)
        {
            var rand = new Random();

            var quality = rand.NextDouble();
            int hp;
            int exp;

            if (quality < 0.7)
            {
                hp = rand.Next(10, 51);
                exp = rand.Next(10, 51);
            } else if (quality < 0.9) {
                hp = rand.Next(60, 101);
                exp = rand.Next(60, 101);
            } else {
                hp = rand.Next(120, 201);
                exp = rand.Next(120, 201);
            }

            return new WildPokemon(pokemonMasters[rand.Next(3)].Name, hp, exp);
            
            
        }
        static WildPokemon CapturePokemon(PokePocket db, List<PokemonMaster> pokemonMasters)
        {
            var pokeballs = db.Pokeballs;
            var pokemons = db.Pokemons;
            

            if (pokeballs.Where(p => p.Count > 0).Count() == 0)
            {
                Console.WriteLine("You do not have any pokeballs to capture a pokemon");
                return null;
            }

            while (true)
            {
                Console.WriteLine("Finding a Pokemon...");
                
                var wildpoke = CreateWildPokemon(pokemonMasters);

                Console.WriteLine($"A wild {wildpoke.Name} has appeared!");
                Console.WriteLine("-------------------------------------");
                Console.WriteLine($"Name: {wildpoke.Name}");
                Console.WriteLine($"HP: {wildpoke.Hp}");
                Console.WriteLine($"EXP: {wildpoke.Exp}");
                
                
                bool exit = true;
                while (exit)
                {
                    Console.WriteLine("-------------------------------------"); 
                    Console.WriteLine("What do you want to do?");
                    Console.WriteLine("1. View Pokeballs");
                    Console.WriteLine("2. Use Pokeball");
                    Console.WriteLine("0. Run Away");
                    var input = ReadOptions("Enter option (1, 2 or 0): ", new List<string>(){"1", "2", "0"});
                    Console.WriteLine("-------------------------------------");
                    
                    
                    switch (input)
                    {
                        case "1":
                            ListPokeballs(pokeballs);
                            break;
                        
                        case "2":
                            do
                            {
                                ListPokeballs(pokeballs);
                                var ballinput = ReadOptionsInt($"Which ball will you use? ({String.Join(", ", pokeballs.Where(b => b.Count > 0).Select(b => b.Id).ToList())}): ", pokeballs.Where(b => b.Count > 0).Select(b => b.Id).ToList());
                                if (pokeballs.Single(p => p.Id == ballinput).useBall(wildpoke))
                                {
                                    var pity = db.Pity.First();
                                    if (new Random().NextDouble() < pity.Value)
                                    {
                                        pity.Reset();
                                        wildpoke.Shiny = true;
                                        Console.WriteLine($"SUCCESS! You caught the {wildpoke.Name}. It has been added to your pocket.");
                                        Console.WriteLine("NO WAY! ITS SHINY");
                                        RewardBalls(db, 20, 31);
                                        Console.WriteLine(wildpoke.Shiny);
                                        
                                        
                                    } else {
                                        pity.Add();
                                        db.SaveChanges();
                                        Console.WriteLine($"SUCCESS! You caught the {wildpoke.Name}. It has been added to your pocket.");
                                    }
                                    return wildpoke;
                                } else if (wildpoke.Tries > 0) {
                                    if (ReadOptions("Try Again? (Y/N): ", new List<string>() {"Y", "N"}).ToLower() == "n")  
                                    {
                                        break;
                                    }
                                }
                            } while (wildpoke.Tries > 0 && pokeballs.Where(p => p.Count > 0).Count() > 0);
                            if (pokeballs.Where(b => b.Count > 0).Count() == 0)
                            {
                                Console.WriteLine("You have no available Pokeballs to use. Running away and returning home!");
                                return null;
                            }
                            Console.WriteLine("Running away!...");
                            exit = false;
                            break;
                        case "0":
                            exit = false;
                            break;
                    }

                }
                if (ReadOptions("Find another Pokemon? (Y/N): ", new List<string>() {"Y", "N"}).ToLower() == "n")
                {
                    return null;
                }
                
            }
        }

        static void RewardBalls(PokePocket db, int min, int max)
        {
            var rand = new Random();
            
            var pokeball = db.Pokeballs.ToList()[rand.Next(4)];
            var amt = rand.Next(min, max);
            pokeball.addBall(amt);
            Console.WriteLine($"You gained {amt} {pokeball.Name}s as a reward!");
            db.SaveChanges();
        }

        
        static void Main(string[] args )
        {
            var db = new PokePocket();
            //PokemonMaster list for checking pokemon evolution availability.    
            List<PokemonMaster> pokemonMasters = new List<PokemonMaster>() {
                new PokemonMaster("Pikachu", 2, "Raichu"),
                new PokemonMaster("Eevee", 3, "Flareon"),
                new PokemonMaster("Charmander", 1, "Charmeleon")
            };

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "restartall":
                        db.RemoveRange(db.Pokemons);
                        db.RemoveRange(db.Pokeballs);
                        db.RemoveRange(db.Pity);
                        break;
                    case "restartpokeballs":
                        db.RemoveRange(db.Pokeballs);
                        break;
                    case "restartpity":
                        db.RemoveRange(db.Pity);
                        break;

                    case "restartpokemons":
                        db.RemoveRange(db.Pokemons);
                        break;
                }
            }

            if (db.Pokeballs.Count() == 0) {
                Console.WriteLine("Creating Pokeball objects");
                db.Add(new Pokeball("Poke Ball"));
                db.Add(new Pokeball("Great Ball"));
                db.Add(new Pokeball("Ultra Ball"));
                db.Add(new Pokeball("Master Ball"));
                db.SaveChanges();
            }

            if (db.Pity.Count() == 0) {
                Console.WriteLine("Creating Pity object");
                db.Add(new Pity());
                db.SaveChanges();
            }
            var rand = new Random();

            //Use "Environment.Exit(0);" if you want to implement an exit of the console program
            // Start your assignment 1 requirements below.
            do
            {
                string userInput = UserInput();
                Console.WriteLine("-----------------------------------------------------------");
                switch(userInput)
                {
                    case "1":
                        AddPokemon(db, pokemonMasters);
                        break;
                    case "2":
                        ListPokes(db.Pokemons.ToList(), false);
                        break;

                    case "3":
                        CheckEvolve(db, pokemonMasters);
                        break;

                    case "4":
                        EvolvePoke(db, pokemonMasters);
                        break;
                    case "5":
                        ListPokeballs(db.Pokeballs);
                        Console.WriteLine("Get PokeBalls by adding a Pokemon to your pocket (1-10) or by Evolving one (5-15)");
                        
                        break;
                    case "6":
                        var wildpoke = CapturePokemon(db, pokemonMasters);
                        if (wildpoke is null)
                        {                            
                            break;
                        }
                        var pokemons = db.Pokemons;
                        if (wildpoke.Name == "Charmander")
                            {
                                var newPoke = new Charmander(wildpoke.Name, wildpoke.Hp, wildpoke.Exp, wildpoke.Shiny);
                                pokemons.Add(newPoke);
                                ListPokes(new List<Pokemon>(){newPoke}, false);
                            } else if (wildpoke.Name == "Eevee")
                            {
                                var newPoke = new Eevee(wildpoke.Name, wildpoke.Hp, wildpoke.Exp, wildpoke.Shiny);
                                pokemons.Add(newPoke);
                                ListPokes(new List<Pokemon>(){newPoke}, false);
                            } else if (wildpoke.Name == "Pikachu")
                            {
                                var newPoke = new Pikachu(wildpoke.Name, wildpoke.Hp, wildpoke.Exp, wildpoke.Shiny);
                                pokemons.Add(newPoke);
                                ListPokes(new List<Pokemon>(){newPoke}, false);
                            }
                            db.SaveChanges();

                        break;
                        
                    case "q":
                        Environment.Exit(0);
                        break;
                }

            } while (true);

            
        }
    }
}

using System;
using System.Collections.Generic;

namespace War
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Platoon platoon1 = new Platoon();
            Platoon platoon2 = new Platoon();
            War war = new War(platoon1, platoon2);

            war.Start();
        }

        class War
        {
            private Platoon _firstPlatoon;
            private Platoon _secondPlatoon;

            public War(Platoon platoon1, Platoon platoon2)
            {
                _firstPlatoon = platoon1;
                _secondPlatoon = platoon2;
            }

            public void Start()
            {
                _firstPlatoon.ShowInfo();
                Console.ReadKey(true);
                Console.Clear();

                _secondPlatoon.ShowInfo();
                Console.ReadKey(true);
                Console.Clear();

                Fight();
                AnnounceWinner();

                Console.ReadKey(true);
                Console.Clear();
            }

            private void Fight()
            {
                while (_firstPlatoon.IsAlive && _secondPlatoon.IsAlive)
                {
                    Console.WriteLine("Атака первого взвода:");
                    _firstPlatoon.Attack(_secondPlatoon);
                    Console.WriteLine("\nИнофрмация про второй взвод\n");
                    _secondPlatoon.ShowInfo();
                    Console.ReadKey(true);
                    Console.Clear();

                    if (_firstPlatoon.IsAlive && _secondPlatoon.IsAlive)
                    {
                        Console.WriteLine("Атака второго взвода:");
                        _secondPlatoon.Attack(_firstPlatoon);
                        Console.WriteLine("\nИнофрмация про первый взвод\n");
                        _firstPlatoon.ShowInfo();
                        Console.ReadKey(true);
                        Console.Clear();
                    }
                }
            }

            private void AnnounceWinner()
            {
                if (_firstPlatoon.IsAlive)
                {
                    Console.WriteLine("Победа за первым взводом!");
                    _firstPlatoon.ShowInfo();
                }
                else
                {
                    Console.WriteLine("Победа за вторым взводом!");
                    _secondPlatoon.ShowInfo();
                }
            }
        }

        class Platoon
        {
            private static int _ids;
            private List<Soldier> _soldiers = new List<Soldier>();

            public int Id { get => _ids; private set { } }
            public bool IsAlive => _soldiers.Count > 0;

            public Platoon(List<Soldier> soldiers = null)
            {
                Id = ++_ids;
                if (soldiers == null)
                {
                    SetDefaultListSoldiers();
                }
            }

            public void Attack(Platoon platoon)
            {
                int numberAttack = _soldiers.Count > platoon._soldiers.Count ? platoon._soldiers.Count : _soldiers.Count;

                for (int i = 0; i < numberAttack; i++)
                {
                    platoon._soldiers[i].TakeDamage(_soldiers[i]);
                    platoon._soldiers[i].ShowInfo();

                    if (platoon._soldiers[i].NumberHealth <= 0)
                    {
                        Console.WriteLine($"{platoon._soldiers[i].Name} был убит ...");
                        platoon._soldiers.Remove(platoon._soldiers[i]);

                        if (platoon._soldiers.Count < numberAttack)
                        {
                            --numberAttack;
                            --i;
                        }
                    }
                }
            }

            public void ShowInfo()
            {
                Console.WriteLine($"Кол-во солдат в взводе: {_soldiers.Count}" +
                                  $"\nСолдаты:");

                for (int i = 0; i < _soldiers.Count; i++)
                {
                    _soldiers[i].ShowInfo();
                }
            }

            private void SetDefaultListSoldiers()
            {
                Random random = new Random();
                int maximumSoldiers = 15;
                int minimumSoldiers = 10;

                for (int i = 0; i < random.Next(minimumSoldiers, maximumSoldiers); i++)
                {
                    SoldierType numberSoldier = (SoldierType)random.Next(1, 4);

                    switch (numberSoldier)
                    {
                        case SoldierType.Recruit:
                            _soldiers.Add(new Recruit("Рекрут", 100, 50, 30));
                            break;
                        case SoldierType.Sergeant:
                            _soldiers.Add(new Sergeant("Сержант", 150, 75, 50));
                            break;
                        case SoldierType.Sniper:
                            _soldiers.Add(new Sniper("Снайпер", 50, 130));
                            break;
                    }
                }
            }
        }

        enum SoldierType
        {
            Recruit = 1,
            Sergeant,
            Sniper
        }

        abstract class Soldier
        {
            private static int _ids;
            protected Random Random = new Random();
            protected int Health;

            public int NumberHealth { get { return Health; } }
            public string Name { get; private set; }
            protected int Damage { get; private set; }
            protected int Armor { get; private set; }

            public Soldier(string name, int health, int damage, int armor)
            {
                Name = $"{name}_{++_ids}";
                Health = health;
                Damage = damage;
                Armor = armor;
            }

            abstract public int Attack();

            public void TakeDamage(Soldier soldier)
            {
                if (Health > 0)
                {
                    int damage = soldier.Attack() - Armor;

                    if (damage < 0)
                    {
                        damage = 0;
                    }

                    Health -= damage;

                    if (Health < 0)
                    {
                        Health = 0;
                    }

                    Console.WriteLine($"\nБоец {soldier.Name} нанес {damage} урона Бойцу {Name}");
                }
            }

            public void ShowInfo()
            {
                Console.WriteLine($"Имя: {Name} | Здоровье: {Health} | Урон: {Damage} | Броня: {Armor}");
            }
        }

        class Recruit : Soldier
        {
            public Recruit(string name, int health, int damage, int armor) :
                base(name, health, damage, armor) { }

            public override int Attack()
            {
                int chanceMiss = 2;
                return Damage * Random.Next(0, chanceMiss);
            }
        }

        class Sergeant : Soldier
        {
            public Sergeant(string name, int health, int damage, int armor) :
                base(name, health, damage, armor) { }

            public override int Attack()
            {
                int half = 2;
                int damageMultiplier = Armor / half;
                return Damage + damageMultiplier;
            }
        }

        class Sniper : Soldier
        {
            public Sniper(string name, int health, int damage) :
                base(name, health, damage, 0) { }

            public override int Attack()
            {
                int chanceSuperAttack = 5;
                return Random.Next(0, chanceSuperAttack) == 3 ? SuperAttack() : Damage;
            }

            private int SuperAttack()
            {
                int damageMultiplier = 2;
                return Damage * damageMultiplier;
            }
        }
    }
}
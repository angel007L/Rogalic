using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Добро пожаловать, воин!");
        Console.Write("Назови себя: ");
        string playerName = Console.ReadLine();

        Player player = new Player(playerName);
        Console.WriteLine($"Ваше имя: {player.Name}");
        Console.WriteLine($"Вам был ниспослан меч {player.Weapon.Name} ({player.Weapon.Damage}), а также {player.HealthAid.Name} ({player.HealthAid.HealingPower}hp).");
        Console.WriteLine($"У вас {player.CurrentHealth}hp.");

        while (player.IsAlive)
        {
            Enemy enemy = Enemy.GenerateRandomEnemy();
            Console.WriteLine($"\n{player.Name} встречает врага {enemy.Name} ({enemy.CurrentHealth}hp), у врага на поясе сияет оружие {enemy.Weapon.Name} ({enemy.Weapon.Damage})");

            while (player.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine("\nЧто вы будете делать?");
                Console.WriteLine("1. Ударить");
                Console.WriteLine("2. Пропустить ход");
                Console.WriteLine("3. Использовать аптечку");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        player.Attack(enemy);
                        if (enemy.IsAlive) 
                        {
                            enemy.Attack(player);
                        }
                        break;
                    case "2":
                        Console.WriteLine($"{player.Name} пропускает ход.");
                        if (enemy.CurrentHealth < 50)
                        {
                            Console.WriteLine($"Здоровье врага меньше 50. Выбирает действие:");
                            Random random = new Random();
                            int randomNumber = random.Next(2);
                            if (randomNumber == 0)
                            {
                                Console.WriteLine($"{enemy.Name} выбирает атаковать!");
                                enemy.Attack(player);
                            }
                            else
                            {
                                Console.WriteLine($"{enemy.Name} выбирает использовать аптечку!");
                                enemy.UseHealthAid();
                            }
                        }
                        else
                        {
                            enemy.Attack(player);
                        }
                        break;
                    case "3":
                        player.UseHealthAid();
                        enemy.Attack(player); 
                        break;
                    default:
                        Console.WriteLine("Некорректный ввод. Пожалуйста, выберите снова.");
                        break;
                }
            }

            if (!player.IsAlive)
            {
                Console.WriteLine($"Игрок {player.Name} повержен. Игра окончена.");
                break;
            }

            Console.WriteLine($"Игрок {player.Name} победил врага {enemy.Name} и получает {enemy.Score} очков!");
            player.Score += enemy.Score;
            Console.WriteLine($"Текущий счет: {player.Score}");

            Console.WriteLine("Продолжить игру? (да(1)/нет(0))");
            string continueGame = Console.ReadLine();

            if (continueGame.ToLower() != "1")
            {
                break;
            }
        }

        Console.WriteLine("Спасибо за игру!");
    }
}

class Player
{
    public string Name { get; }
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; }
    public int Score { get; set; }
    public Weapon Weapon { get; private set; }
    public Aid HealthAid { get; }

    public bool IsAlive => CurrentHealth > 0;

    public Player(string name)
    {
        Name = name;
        MaxHealth = 100;
        CurrentHealth = MaxHealth;
        Score = 0;
        HealthAid = new Aid("Средняя аптечка", 10);
        GenerateRandomEquipment();
    }

    private void GenerateRandomEquipment()
    {
        string[] weaponNames = { "Меч", "Лук", "Кинжал", "Топор", "Посох" };
        string randomWeaponName = weaponNames[new Random().Next(weaponNames.Length)];

        int weaponDamage = new Random().Next(10, 20);

        Weapon = new Weapon(randomWeaponName, weaponDamage);
    }

    public void Attack(Enemy enemy)
    {
        Console.WriteLine($"{Name} ударил противника {enemy.Name} с использованием {Weapon.Name}!");
        int damage = Weapon.Damage;
        enemy.TakeDamage(damage);

    }

    public void UseHealthAid()
    {
        Console.WriteLine($"{Name} использовал аптечку и восстановил {HealthAid.HealingPower} hp.");
        CurrentHealth = Math.Min(CurrentHealth + HealthAid.HealingPower, MaxHealth);
        
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (!IsAlive)
        {
            Console.WriteLine($"{Name} повержен!");
        }
        else
        {
            Console.WriteLine($"{Name} получает {damage} урона. Осталось {CurrentHealth} hp.");
        }
    }
}

class Enemy
{
    public string Name { get; }
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; }
    public Weapon Weapon { get; }
    public int Score { get; }
    private static Random random = new Random();
    public Aid HealthAid { get; }
    

    public bool IsAlive => CurrentHealth > 0;

    public Enemy(string name, int maxHealth, Weapon weapon, int score)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        Weapon = weapon;
        Score = score;
        HealthAid = new Aid("Средняя аптечка", 5);
    }

    public void Attack(Player player)
    {
        Console.WriteLine($"{Name} атакует {player.Name} с использованием {Weapon.Name}!");
        int damage = Weapon.Damage;
        player.TakeDamage(damage);

    }
    public void UseHealthAid()
    {
        Console.WriteLine($"{Name} использовал аптечку и восстановил {HealthAid.HealingPower} здоровья.");
        CurrentHealth = Math.Min(CurrentHealth + HealthAid.HealingPower, MaxHealth);
        Console.WriteLine($"{Name} теперь имеет {CurrentHealth}hp.");
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (!IsAlive)
        {
            Console.WriteLine($"{Name} повержен!");
        }
        else
        {
            Console.WriteLine($"{Name} получает {damage} урона. Осталось {CurrentHealth} hp.");
        }
    }

    public static Enemy GenerateRandomEnemy()
    {
        string[] names = { "Гоблин", "Орк", "Скелет", "Демон", "Вампир" };
        string randomName = names[random.Next(names.Length)];

        int maxHealth = random.Next(50, 100);
        int score = random.Next(1, 10);

        string[] weaponNames = { "Кинжал", "Меч", "Топор", "Лук", "Посох" };
        string randomWeaponName = weaponNames[random.Next(weaponNames.Length)];

        int weaponDamage = random.Next(5, 15);

        Weapon enemyWeapon = new Weapon(randomWeaponName, weaponDamage);
        return new Enemy(randomName, maxHealth, enemyWeapon, score);
    }
}

class Aid
{
    public string Name { get; }
    public int HealingPower { get; }

    public Aid(string name, int healingPower)
    {
        Name = name;
        HealingPower = healingPower;
    }
}

class Weapon
{
    public string Name { get; }
    public int Damage { get; }

    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }
}

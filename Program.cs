using Newtonsoft.Json;
using Spectre.Console;
using System.Runtime.InteropServices;

public class Program
{

    public static void Main()
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        IntPtr consoleWindowHandle = GetForegroundWindow();
        ShowWindow(consoleWindowHandle, 3);

        KnoxSim knoxSim = new KnoxSim();
        Upgrades upgrades = new Upgrades();
        Talent talent = new Talent();
        Attributes attributes = new Attributes();

        AnsiConsole.WriteLine(" Select Mode : ");
        AnsiConsole.WriteLine(" 1. Normal Mode");
        AnsiConsole.WriteLine(" 2. Best Upgrade Mode");
        string input = AnsiConsole.Prompt(
             new TextPrompt<string>(""));


        string json = File.ReadAllText("config.json");
        ConfigFile config = JsonConvert.DeserializeObject<ConfigFile>(json);

        void ClearResultsFile()
        {
            using (StreamWriter writer = new StreamWriter("Results.md", false)) ;// 'false' to overwrite the file
        }
        ClearResultsFile();

        if (input == "1")
        {
            foreach (Config simulation in config.simulations)
            {
                StartSimulation(simulation);
            }

        }
        else if (input == "2")
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(" ");
            double initialGlacium = AnsiConsole.Prompt(
            new TextPrompt<double>(" Glacium: ")
                .Validate(input => double.TryParse(input.ToString(), out _) ? ValidationResult.Success() : ValidationResult.Error("[red]Please enter a valid number.[/]")));
            AnsiConsole.Write(" ");
            double initialQuartz = AnsiConsole.Prompt(
             new TextPrompt<double>("Quartz: ")
                 .Validate(input => double.TryParse(input.ToString(), out _) ? ValidationResult.Success() : ValidationResult.Error("[red]Please enter a valid number.[/]")));
            AnsiConsole.Write(" ");
            double initialTesseract = AnsiConsole.Prompt(
                new TextPrompt<double>("Tesseract: ")
                    .Validate(input => double.TryParse(input.ToString(), out _) ? ValidationResult.Success() : ValidationResult.Error("[red]Please enter a valid number.[/]")));


            config.simulations = new List<Config> { config.simulations[0] };
            Config sim = config.simulations[0];
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[bold deepskyblue3_1] Original Build : [/]");
            StartSimulation(sim);

            double CalculateHpCost(int level)
            {
                return Math.Ceiling(1 * Math.Pow(1.054 + 0.00027 * MathF.Min(level, 110), level));
            }
            double CalculateAtkCost(int level)
            {
                return Math.Ceiling(2 * Math.Pow(1.068 + 0.00027 * MathF.Min(level, 100), level));
            }
            double CalculateRegenCost(int level)
            {
                return Math.Ceiling(4 * Math.Pow(1.09 + 0.00027 * MathF.Min(level, 70), level));
            }
            double CalculateDRCost(int level)
            {
                double baseValue = 2 * Math.Pow((0.008 * level + 1.12), level);
                double multiplier = 0.9 * Math.Pow(1.2, Math.Max(level - 9, 0)) *
                        Math.Pow(1.5, Math.Max(level - 19, 0)) *
                        Math.Pow(2, Math.Max(level - 29, 0)) *
                        Math.Pow(3, Math.Max(level - 34, 0)) *
                        Math.Pow(4, Math.Max(level - 39, 0)) *
                        Math.Pow(5, Math.Max(level - 44, 0));
                return Math.Ceiling(Math.Ceiling(baseValue) * multiplier);
            }
            double CalculateBlockCost(int level)
            {
                double baseValue = 3 * Math.Pow((0.028 * level + 1.18), level);
                double multiplier = 0.9 * Math.Pow(1.2, Math.Max(level - 9, 0)) *
                        Math.Pow(1.5, Math.Max(level - 19, 0)) *
                        Math.Pow(2, Math.Max(level - 29, 0)) *
                        Math.Pow(3, Math.Max(level - 34, 0)) *
                        Math.Pow(4, Math.Max(level - 39, 0)) *
                        Math.Pow(5, Math.Max(level - 44, 0));
                return Math.Ceiling(Math.Ceiling(baseValue) * multiplier);
            }
            double CalculateEffectCost(int level)
            {
                double baseValue = 50 * Math.Pow((0.018 * level + 1.2), level);
                double multiplier = 0.9 * Math.Pow(1.2, Math.Max(level - 9, 0)) *
                        Math.Pow(1.5, Math.Max(level - 19, 0)) *
                        Math.Pow(2, Math.Max(level - 29, 0)) *
                        Math.Pow(3, Math.Max(level - 34, 0)) *
                        Math.Pow(4, Math.Max(level - 39, 0)) *
                        Math.Pow(5, Math.Max(level - 44, 0));
                return Math.Ceiling(Math.Ceiling(baseValue) * multiplier);
            }
            double CalculateChargeChanceCost(int level)
            {
                double baseValue = Math.Pow(0.016 * level + 1.18, level);
                double multiplier = 0.9 *
                                     Math.Pow(1.05, Math.Max(level - 9, 0)) *
                                     Math.Pow(1.05, Math.Max(level - 19, 0)) *
                                     Math.Pow(1.2, Math.Max(level - 29, 0)) *
                                     Math.Pow(1.3, Math.Max(level - 39, 0)) *
                                     Math.Pow(1.4, Math.Max(level - 49, 0)) *
                                     Math.Pow(1.5, Math.Max(level - 59, 0));

                return (int)Math.Ceiling(Math.Ceiling(baseValue * multiplier));
            }
            double CalculateChargeAmountCost(int level)
            {
                double baseValue = Math.Pow(0.025 * level + 1.35, level);
                double multiplier = 0.9 *
                                     Math.Pow(1.05, Math.Max(level - 9, 0)) *
                                     Math.Pow(1.05, Math.Max(level - 19, 0)) *
                                     Math.Pow(1.2, Math.Max(level - 29, 0)) *
                                     Math.Pow(1.3, Math.Max(level - 39, 0)) *
                                     Math.Pow(1.4, Math.Max(level - 49, 0)) *
                                     Math.Pow(1.5, Math.Max(level - 59, 0));

                return (int)Math.Ceiling(Math.Ceiling(baseValue * multiplier));
            }
            double CalculateASCost(int level)
            {
                double baseValue = 2 * Math.Pow((0.035 * level + 1.24), level);
                double multiplier = 0.9 *
                                     Math.Pow(1.02, Math.Max(level - 9, 0)) *
                                     Math.Pow(1.05, Math.Max(level - 19, 0)) *
                                     Math.Pow(1.2, Math.Max(level - 29, 0)) *
                                     Math.Pow(1.3, Math.Max(level - 39, 0)) *
                                     Math.Pow(1.4, Math.Max(level - 49, 0)) *
                                     Math.Pow(1.5, Math.Max(level - 59, 0)) *
                                     Math.Pow(1.6, Math.Max(level - 69, 0)) *
                                     Math.Pow(1.7, Math.Max(level - 79, 0)) *
                                     Math.Pow(1.8, Math.Max(level - 89, 0));

                return (int)Math.Ceiling(Math.Ceiling(baseValue * multiplier));
            }
            (double, double, double) CalculateSalvoCost(int level)
            {
                double glaciumCost = 100 * Math.Pow(1000, level) * 0.8;
                double quartzCost = 100 * Math.Pow(1000, level) * 1.2;
                double tesseractCost = 100 * Math.Pow(1000, level) * 0.9;

                return (glaciumCost, quartzCost, tesseractCost);
            }
            int CalculateMaxLevel(int currentLevel, double resource, Func<int, double> costFunction)
            {
                while (resource > costFunction(currentLevel))
                {
                    resource -= costFunction(currentLevel);
                    currentLevel++;
                }
                return currentLevel;
            }

            // Define groups of upgrades for each resource type with their corresponding cost functions            
            var resourcesUpgrades = new[]
               {
                    (Resource: "Glacium", Amount: initialGlacium, Stats: new Dictionary<int, Func<int, double>>
                    {
                        [0] = CalculateHpCost,
                        [1] = CalculateAtkCost,
                        [2] = CalculateRegenCost
                    }),
                    (Resource: "Quartz", Amount: initialQuartz, Stats: new Dictionary<int, Func<int, double>>
                    {
                        [3] = CalculateDRCost,
                        [4] = CalculateBlockCost,
                        [5] = CalculateEffectCost
                    }),
                    (Resource: "Tesseract", Amount: initialTesseract, Stats: new Dictionary<int, Func<int, double>>
                    {
                        [6] = CalculateChargeChanceCost,
                        [7] = CalculateChargeAmountCost,
                        [8] = CalculateASCost
                    })
                };

            // Filter to only include resources that have an amount > 0
            var availableUpgrades = resourcesUpgrades.Where(u => u.Amount > 0).ToList();

            // Generate all possible combinations of upgrades from available resources
            // If a resource isn't available, use empty dictionary to skip those combinations
            var combinations = new List<((double, KeyValuePair<int, Func<int, double>>) first,
                (double, KeyValuePair<int, Func<int, double>>) second,
                (double, KeyValuePair<int, Func<int, double>>) third)>();

            foreach (var firstUpgrade in availableUpgrades[0].Stats)
            {
                // If we have a second resource
                if (availableUpgrades.Count > 1)
                {
                    foreach (var secondUpgrade in availableUpgrades[1].Stats)
                    {
                        // If we have a third resource
                        if (availableUpgrades.Count > 2)
                        {
                            foreach (var thirdUpgrade in availableUpgrades[2].Stats)
                            {
                                combinations.Add(((availableUpgrades[0].Amount, firstUpgrade), (availableUpgrades[1].Amount, secondUpgrade), (availableUpgrades[2].Amount, thirdUpgrade)));
                            }
                        }
                        else
                        {
                            // No third resource, use empty upgrade
                            combinations.Add(((availableUpgrades[0].Amount, firstUpgrade), (availableUpgrades[1].Amount, secondUpgrade), (0, new KeyValuePair<int, Func<int, double>>(100, new Func<int, double>((_) => 0.0)))));
                        }
                    }
                }
                else
                {
                    // No second resource, use empty upgrades
                    combinations.Add(((availableUpgrades[0].Amount, firstUpgrade), (0, new KeyValuePair<int, Func<int, double>>(100, new Func<int, double>((_) => 0.0))), (0, new KeyValuePair<int, Func<int, double>>(100, new Func<int, double>((_) => 0.0)))));
                }
            }

            foreach (var (first, second, third) in combinations)
            {
                Config newConfig = new Config
                {
                    iterations = sim.iterations,
                    knoxLevel = sim.knoxLevel,
                    upgrades = new List<int>(sim.upgrades),
                    talents = new List<int>(sim.talents),
                    attributes = new List<int>(sim.attributes),
                };



                newConfig.upgrades[first.Item2.Key] = CalculateMaxLevel(newConfig.upgrades[first.Item2.Key], first.Item1, first.Item2.Value);
                if (second.Item2.Key < 100)
                {
                    newConfig.upgrades[second.Item2.Key] = CalculateMaxLevel(newConfig.upgrades[second.Item2.Key], second.Item1, second.Item2.Value);
                }
                if (third.Item2.Key < 100)
                {
                    newConfig.upgrades[third.Item2.Key] = CalculateMaxLevel(newConfig.upgrades[third.Item2.Key], third.Item1, third.Item2.Value);
                }


                StartSimulation(newConfig, (first.Item2.Key, second.Item2.Key, third.Item2.Key));
            }

            (double, double, double) salvoCost = CalculateSalvoCost(sim.upgrades[9]);
            //check is Salvo upgrade affordable
            if (initialGlacium >= salvoCost.Item1 && initialQuartz >= salvoCost.Item2 && initialTesseract >= salvoCost.Item3)
            {
                Config newConfig = new Config
                {
                    iterations = sim.iterations,
                    knoxLevel = sim.knoxLevel,
                    upgrades = new List<int>(sim.upgrades),
                    talents = new List<int>(sim.talents),
                    attributes = new List<int>(sim.attributes),
                };
                newConfig.upgrades[9] += 1;
                StartSimulation(newConfig, (9, 100, 100));
            }

            using (StreamWriter writer = new StreamWriter("Results.md", true))
            {
                writer.WriteLine();
                writer.WriteLine(" Glacium : " + initialGlacium + ", Quartz : " + initialQuartz + " Tesseract : " + initialTesseract);
                writer.WriteLine(" First build is original Build : ");
            }


        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        void StartSimulation(Config simulation, (int first, int second, int third)? modifiedUpgrades = null)
        {
            knoxSim = new KnoxSim();
            knoxSim.SetKnoxLevel(simulation.knoxLevel);
            upgrades.SetUpgradesList(simulation.upgrades);
            talent.SetTalentList(simulation.talents);
            attributes.SetAttributeList(simulation.attributes);
            knoxSim.InitializeKnox(talent, attributes, upgrades);
            knoxSim.Fight(simulation.iterations);
            knoxSim.Output(modifiedUpgrades);
        }


    }
}

public class KnoxSim
{
    float health, maxHealth, attack, healthRegen, dr, block, effect, chargeChance, chargeAmount, chargeSalvo, chargeTorpedo, attackSpeed, knoxLevel;
    int salvo, stage = 0, maxStage = 50;
    float ennemyHealth, ennemyMaxHealth, ennemyAttack, ennemyRegen, ennemyAttackSpeed, ennemyCritChance, ennemyCritPower;
    int elixirRegenTicks = 0;
    TimeSpan reviveTime = TimeSpan.FromSeconds(40);

    Talent currentTalents = new Talent();
    Attributes currentAttributes = new Attributes();
    Upgrades currentUpgrades = new Upgrades();

    List<float> lootList = new List<float>();
    List<float> stageList = new List<float>();
    List<TimeSpan> timeList = new List<TimeSpan>();
    List<float> ghostList = new List<float>();
    List<float> lpmList = new List<float>();
    List<int> stageMaxGhostsList = new List<int>();

    public void SetKnoxLevel(int level)
    {
        knoxLevel = level;
    }
    public float GetStageScaling(int currentStage)
    {
        if (currentStage > 49)
        {
            return (1f + (currentStage - 49) * 0.006f);
        }
        else
        {
            return 1f;
        }
    }
    public float GetLootBonus()
    {
        return 1.074f;
    }
    public void InitializeKnox(Talent talent, Attributes attribute, Upgrades upgrade)
    {
        currentTalents = talent;
        currentAttributes = attribute;
        currentUpgrades = upgrade;

        health = maxHealth = (20f + (2f + (int)(upgrade.health / 5) * 0.1f) * upgrade.health) * (1 + attribute.kraken * 0.01f);
        attack = (1.2f + (0.06f + (int)(upgrade.attack / 10) * 0.01f) * upgrade.attack) * (1 + attribute.kraken * 0.005f);
        healthRegen = (0.05f + (0.03f + (int)(upgrade.regen / 30) * 0.02f) * upgrade.regen) * (1 + attribute.kraken * 0.008f);
        dr = upgrade.dr * 0.32f + attribute.pirate * 0.9f;
        block = 8f + upgrade.block * 0.55f + attribute.pirate * 0.8f + attribute.elixir * 1f;
        effect = 5f + upgrade.effect * 0.36f + attribute.searious * 2f + attribute.pirate * 0.7f;
        chargeChance = 7f + upgrade.chargechance * 0.25f + attribute.pirate * 0.6f + attribute.searious * 1f;
        chargeAmount = 0.25f + upgrade.chargeamount * 0.01f;
        attackSpeed = 7f - 0.03f * upgrade.attackspeed;
        salvo = 3 + upgrade.salvo;
    }
    public void InitializeEnnemy()
    {
        ennemyHealth = ennemyMaxHealth = (7 + stage * 9) * GetStageScaling(stage);
        ennemyAttack = (2.4f + stage * 1.4f) * GetStageScaling(stage) * (1 - currentTalents.presence * 0.03f); ;
        ennemyAttackSpeed = 6.005f - stage * 0.005f;
        ennemyRegen = (0 + stage * 0.04f) * GetStageScaling(stage) * (1 - 0.08f * currentTalents.omen);
        ennemyCritChance = 9.94f + stage * 0.06f;
        ennemyCritPower = 1.03f + stage * 0.008f;
    }
    public void Fight(int iterations)
    {
        float elixirBonusFactor = currentAttributes.elixir * 0.1f;
        int ghostBulletChance = currentTalents.ghostBullet * 5;
        int pirateBonusChance = currentAttributes.pirate * 2;
        int maxTorpedoCharge = (100 - currentAttributes.kingTorpedo * 10);
        float passiveChargeAmount = currentAttributes.passiveCharge * 0.02f;
        float lootMultiplier = 1f + (currentAttributes.timeless * 0.13f);
        float soulAmpMult = 0.005f * (1 + currentAttributes.soulAmp * 0.01f);
        int maxGhostCharge = 100 + currentAttributes.deadMan * 10;
        float shieldEffect = 0.2f * currentAttributes.shield;
        float unfairEffect = 0.02f * currentTalents.unfair;
        float finishingEffect = 0.1f * currentTalents.finishing;
        for (int i = 0; i < iterations; i++)
        {
            int ennemyCounter = 0;
            InitializeEnnemy();
            float totalLoot = 0f;
            int currentSalvo = 0;
            int currentTorpedo = 0;
            int reviveCounter = currentTalents.revive;
            int ghostCharges = 0;
            int stageMaxGhost = 0;


            // Define intervals for each event
            TimeSpan regenInterval = TimeSpan.FromSeconds(1);
            TimeSpan enemyAttackInterval = TimeSpan.FromSeconds(ennemyAttackSpeed);
            TimeSpan playerReloadInterval = TimeSpan.FromSeconds(attackSpeed);
            TimeSpan bulletInterval = TimeSpan.FromSeconds(0.1);

            // Initialize last event times
            TimeSpan virtualTime = TimeSpan.Zero;
            TimeSpan lastRegenTime = TimeSpan.Zero;
            TimeSpan lastEnemyAttackTime = TimeSpan.Zero;
            TimeSpan lastReloadTime = TimeSpan.Zero;

            Random random = new Random(); ;

            float GetMaxHealth()
            {
                return maxHealth * (1 + GetGhostCharges() * soulAmpMult);
            }
            float GetAttack()
            {
                return attack * (1 + GetGhostCharges() * soulAmpMult);
            }
            float GetRegen()
            {
                return healthRegen * (1 + GetGhostCharges() * soulAmpMult);
            }
            void Loot()
            {
                float llmult = 1f;
                if (random.NextDouble() * 100 < effect)
                {
                    llmult = (1 + (0.2f * currentTalents.luckyLoot));
                }

                totalLoot += (float)Math.Pow(GetLootBonus(), stage) * lootMultiplier * llmult;
            }
            float GetGhostChance()
            {
                return 2f + (effect / 3) + (maxStage * 0.08f) + (knoxLevel * 0.14f);
            }
            int GetGhostCharges()
            {
                return Math.Min(maxGhostCharge, ghostCharges);
            }
            void AddGhostCharges()
            {
                if (random.NextDouble() * 100 < GetGhostChance() && ghostCharges <= maxGhostCharge)
                {
                    ghostCharges++;
                    health += maxHealth * 0.005f;

                    ghostCharges = Math.Min(ghostCharges, 100 + currentAttributes.deadMan * 10);
                }
                if (ghostCharges == maxGhostCharge && stageMaxGhost == 0)
                {
                    stageMaxGhost = stage;
                }
            }
            void Calypso(int oldStage)
            {
                if (oldStage < stage)
                {
                    if (random.NextDouble() * 100 < effect * 2.5)
                    {
                        ghostCharges += currentTalents.calypso;
                        health += maxHealth * 0.005f * currentTalents.calypso;

                        ghostCharges = Math.Min(ghostCharges, maxGhostCharge);
                    }
                }
                if (ghostCharges == maxGhostCharge && stageMaxGhost == 0)
                {
                    stageMaxGhost = stage;
                }
            }


            while (health > 0 || reviveCounter > 0)
            {
                // Calculate time until each event occurs next
                TimeSpan timeUntilRegen = regenInterval - (virtualTime - lastRegenTime);
                TimeSpan timeUntilEnemyAttack = enemyAttackInterval - (virtualTime - lastEnemyAttackTime);
                TimeSpan timeUntilReload = playerReloadInterval - (virtualTime - lastReloadTime);
                TimeSpan timeUntilBullet = currentSalvo > 0 ? bulletInterval : TimeSpan.MaxValue;
                TimeSpan timeUntilTorpedo = currentTorpedo > 0 && currentSalvo == 0 ? bulletInterval : TimeSpan.MaxValue;
                // Find the smallest time increment to jump to the next event
                TimeSpan nextEventTime = timeUntilRegen;

                if (timeUntilEnemyAttack < nextEventTime)
                    nextEventTime = timeUntilEnemyAttack;
                if (timeUntilReload < nextEventTime)
                    nextEventTime = timeUntilReload;
                if (timeUntilBullet < nextEventTime)
                    nextEventTime = timeUntilBullet;

                // Advance virtual time by the time to the next event
                virtualTime += nextEventTime;

                if (timeUntilRegen == nextEventTime)
                {
                    TickEverySecond();
                    lastRegenTime = virtualTime;
                }
                if (timeUntilEnemyAttack == nextEventTime)
                {
                    EnnemyAttack();
                    lastEnemyAttackTime = virtualTime;
                }
                if (timeUntilReload == nextEventTime && currentSalvo == 0)
                {
                    Recharge();
                    lastReloadTime = virtualTime;
                }
                if (timeUntilBullet == nextEventTime && currentSalvo > 0)
                {
                    currentSalvo--;
                    Attack();
                }
                if (timeUntilTorpedo == nextEventTime && currentTorpedo > 0)
                {
                    chargeTorpedo--;
                    Torpedo();
                }

                void TickEverySecond()
                {
                    float elixirBonus = 1f;
                    if (elixirRegenTicks > 0)
                    {
                        elixirBonus = 1f + elixirBonusFactor;
                        elixirRegenTicks -= 1;
                    }
                    health = MathF.Min(health + GetRegen() * elixirBonus, GetMaxHealth());
                    ennemyHealth = MathF.Min(ennemyHealth + ennemyRegen, ennemyMaxHealth);
                    chargeSalvo += passiveChargeAmount;
                    chargeTorpedo += passiveChargeAmount;
                }
                void EnnemyAttack()
                {
                    float effectiveDamage = ennemyAttack * (1 - dr / 100);

                    if (random.NextDouble() * 100 < ennemyCritChance)
                    {
                        effectiveDamage *= ennemyCritPower;
                    }
                    if (random.NextDouble() * 100 < block)
                    {
                        effectiveDamage /= 2;
                        //elixir stacks
                        elixirRegenTicks = 5;

                        //shield attribute effect
                        ennemyHealth -= effectiveDamage * shieldEffect;
                        chargeSalvo += shieldEffect / 2;
                        chargeTorpedo += shieldEffect / 2;

                        if (ennemyHealth < 0)
                        {
                            KillEnnemy();
                        }
                    }
                    health -= effectiveDamage;
                }
                void Recharge()
                {
                    currentSalvo = salvo;
                    //Salvo and Torpedo mechanism;
                    if (random.NextDouble() * 100 < chargeChance) // Assuming chargeChance is a percentage
                    {
                        chargeSalvo += chargeAmount;
                        chargeTorpedo += chargeAmount;
                    }
                    if (chargeSalvo >= 10)
                    {
                        chargeSalvo -= 10;
                        currentSalvo += 3;
                    }
                    if (chargeTorpedo >= maxTorpedoCharge)
                    {
                        chargeTorpedo -= maxTorpedoCharge;
                        currentTorpedo = 5 + currentAttributes.kingTorpedo;
                    }
                    //Ghost Bullet talent
                    if (random.NextDouble() * 100 < ghostBulletChance)
                    {
                        currentSalvo += 1;
                    }
                    //SpacePirate Attribute
                    if (random.NextDouble() * 100 < pirateBonusChance)
                    {
                        currentSalvo += 3;
                    }
                }
                void Attack()
                {
                    //Finishing Move
                    if (currentSalvo == 0 && random.NextDouble() * 100 < 2 * effect)
                    {
                        ennemyHealth -= GetAttack() * (1 + finishingEffect);
                    }
                    else
                    {
                        ennemyHealth -= GetAttack();
                    }
                    if (ennemyHealth < 0)
                    {
                        KillEnnemy();
                        lastEnemyAttackTime = virtualTime;
                    }
                }
                void Torpedo()
                {
                    ennemyHealth -= GetAttack() * 30 * (1f + 0.08f * currentAttributes.passiveCharge + 0.2f * currentAttributes.kingTorpedo);
                    if (ennemyHealth < 0)
                    {
                        KillEnnemy();
                        lastEnemyAttackTime = virtualTime;
                    }
                }

                if (health <= 0 && reviveCounter > 0)
                {
                    reviveCounter--;
                    health = maxHealth * 0.8f;
                }

                void KillEnnemy()
                {
                    ennemyCounter += 1;
                    Loot();
                    AddGhostCharges();
                    int oldStage = stage;
                    stage = ennemyCounter / 10;
                    Calypso(oldStage);
                    InitializeEnnemy();

                    //Unfair Advantage
                    if (random.NextDouble() * 100 < effect && currentTalents.unfair > 0)
                    {
                        health = MathF.Min(health + (GetMaxHealth() * unfairEffect), GetMaxHealth());
                    }
                }
            }

            virtualTime += reviveTime;
            lootList.Add(totalLoot);
            stageList.Add((float)ennemyCounter / 10);
            ghostList.Add(GetGhostCharges());
            timeList.Add(virtualTime);
            stageMaxGhostsList.Add(stageMaxGhost);
            lpmList.Add(totalLoot / (float)virtualTime.TotalMinutes);
            maxStage = Math.Max(maxStage, stage);
            stage = 0;
            health = maxHealth;
        }
    }

    public void Output((int first, int second, int third)? modifiedUpgrades = null)
    {
        int zeroCount = stageMaxGhostsList.Count(value => value == 0); // Compte les 0 dans la liste
        float percentage = (float)zeroCount / stageMaxGhostsList.Count * 100; // Calcule le pourcentage

        var nonZeroValues = stageMaxGhostsList.Where(value => value != 0).ToList(); // Filtre les valeurs différentes de 0       

        TimeSpan timeMin = timeList.Min();
        string formattedTimeMin = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                         timeMin.Hours,
                                         timeMin.Minutes,
                                         timeMin.Seconds);

        TimeSpan timeAvg = new TimeSpan((long)timeList.Average(t => t.Ticks));
        string formattedTimeAvg = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                         timeAvg.Hours,
                                         timeAvg.Minutes,
                                         timeAvg.Seconds);

        TimeSpan timeMax = timeList.Max();
        string formattedTimeMax = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                         timeMax.Hours,
                                         timeMax.Minutes,
                                         timeMax.Seconds);

        void WriteResultsToFile()
        {
            using (StreamWriter writer = new StreamWriter("Results.md", true)) // 'true' appends to the file if it exists
            {
                writer.WriteLine(" ");
                writer.WriteLine(" Level: " + knoxLevel);
                writer.WriteLine(" Health : " + currentUpgrades.upgradeList[0] +
                                 ", Attack : " + currentUpgrades.upgradeList[1] +
                                 ", Regen : " + currentUpgrades.upgradeList[2] +
                                 ", DR : " + currentUpgrades.upgradeList[3] +
                                 ", Block : " + currentUpgrades.upgradeList[4] +
                                 ", Effect : " + currentUpgrades.upgradeList[5] +
                                 ", ChargeChance : " + currentUpgrades.upgradeList[6] +
                                 ", ChargeAmount : " + currentUpgrades.upgradeList[7] +
                                 ", AttackSpeed : " + currentUpgrades.upgradeList[8] +
                                 ", Salvo:" + currentUpgrades.upgradeList[9]);

                writer.WriteLine(" Revive : " + currentTalents.talentList[0] +
                                 ", Calypso : " + currentTalents.talentList[1] +
                                 ", Unfair : " + currentTalents.talentList[2] +
                                 ", GhostBullet : " + currentTalents.talentList[3] +
                                 ", Omen : " + currentTalents.talentList[4] +
                                 ", LL : " + currentTalents.talentList[5] +
                                 ", POG : " + currentTalents.talentList[6] +
                                 ", Finishing : " + currentTalents.talentList[7]);


                writer.WriteLine(" Kraken : " + currentAttributes.attributeList[0] +
                                 ", Armory : " + currentAttributes.attributeList[1] +
                                 ", Pirate : " + currentAttributes.attributeList[2] +
                                 ", Timeless : " + currentAttributes.attributeList[3] +
                                 ", SoulA : " + currentAttributes.attributeList[4] +
                                 ", DeadM : " + currentAttributes.attributeList[5] +
                                 ", SeaR : " + currentAttributes.attributeList[6] +
                                 ", Charge : " + currentAttributes.attributeList[7] +
                                 ", King : " + currentAttributes.attributeList[8] +
                                 ", Elixir : " + currentAttributes.attributeList[9] +
                                 ", Shield : " + currentAttributes.attributeList[10]);

                writer.WriteLine();
                writer.WriteLine(" Time //     Min : " + formattedTimeMin + " // Avg : " + formattedTimeAvg + " // Max : " + formattedTimeMax);
                writer.WriteLine(" Stage //    Min : " + stageList.Min() + " // Avg : " + stageList.Average().ToString("F2") + " // Max : " + stageList.Max());
                writer.WriteLine(" LPM //      Min : " + lpmList.Min().ToString("F4") + " // **Avg : " + lpmList.Average().ToString("F4") + "** // Max : " + lpmList.Max().ToString("F4"));
                writer.WriteLine(" Ghosts //   Min : " + ghostList.Min() + " // Avg : " + ghostList.Average().ToString("F2") + " // Max : " + ghostList.Max());
                writer.WriteLine(" Max Ghosts at stage //   Never : " + percentage.ToString("F2") + " % // " + "Min : " + nonZeroValues.Min() + " // Avg : " + nonZeroValues.Average().ToString("F2") + " // Max : " + nonZeroValues.Max());
                writer.WriteLine();
                writer.WriteLine("------------------------------------------------------------------------------------------");
            }
        }
        void ConsoleOutput()
        {
            string[] upgradeNames = { "HP", "Atk", "Regen", "DR", "Block", "Effect", "ChargeChance", "ChargeAmount", "Reload", "Salvo" };
            if (modifiedUpgrades.HasValue)
            {
                var selectedUpgrades = new[] { modifiedUpgrades.Value.first, modifiedUpgrades.Value.second, modifiedUpgrades.Value.third }
                    .Where(index => index >= 0 && index < upgradeNames.Length)
                    .Select(index => upgradeNames[index]);
                AnsiConsole.Write(" ");
                AnsiConsole.Markup("[deepskyblue3_1] Modified Upgrades : " + string.Join(", ", selectedUpgrades) + "[/]");
            }

            AnsiConsole.WriteLine(" ");
           
            Style goldstyle = new Style(foreground: Color.Gold3_1);
            Style blueStyle = new Style(foreground: Color.DeepSkyBlue3_1);
            Style redStyle = new Style(foreground: Color.Red);


            var table = new Table();
            table.AddColumn(new TableColumn(" Upgrades : ").LeftAligned());
            table.AddColumn(new TableColumn(" Talents : ").LeftAligned());
            table.AddColumn(new TableColumn(" Attributes : ").LeftAligned());
            table.AddRow(
                new Text($"Level : {knoxLevel}"),
                new Text($"Revive : {currentTalents.revive}", currentTalents.revive > 0 ? goldstyle : Style.Plain),
                new Text($"Kraken : {currentAttributes.kraken}", currentAttributes.kraken > 1 ? goldstyle : Style.Plain)
            );
            table.AddRow(
                new Text($"HP : {currentUpgrades.health}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 0 || modifiedUpgrades.Value.second == 0 || modifiedUpgrades.Value.third == 0) ? blueStyle : Style.Plain),
                new Text($"Calypso : {currentTalents.calypso}", currentTalents.calypso > 0 ? goldstyle : Style.Plain),
                new Text($"Armory : {currentAttributes.armory}", currentAttributes.armory > 1 ? goldstyle : Style.Plain)
            );
            table.AddRow(
                new Text($"Atk : {currentUpgrades.attack}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 1 || modifiedUpgrades.Value.second == 1 || modifiedUpgrades.Value.third == 1) ? blueStyle : Style.Plain),
                new Text($"Unfair : {currentTalents.unfair}", currentTalents.unfair > 0 ? goldstyle : Style.Plain),
                new Text($"Pirate : {currentAttributes.pirate}", currentAttributes.pirate > 1 ? goldstyle : Style.Plain)
            );
            table.AddRow(
                new Text($"Regen : {currentUpgrades.regen}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 2 || modifiedUpgrades.Value.second == 2 || modifiedUpgrades.Value.third == 2) ? blueStyle : Style.Plain),
                new Text($"Bullet : {currentTalents.ghostBullet}", currentTalents.ghostBullet > 0 ? goldstyle : Style.Plain),
                new Text($"TimeLess : {currentAttributes.timeless}", currentAttributes.timeless > 0 ? goldstyle : Style.Plain)
            );
            table.AddRow(
                new Text($"DR : {currentUpgrades.dr}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 3 || modifiedUpgrades.Value.second == 3 || modifiedUpgrades.Value.third == 3) ? blueStyle : Style.Plain),
                new Text($"Omen : {currentTalents.omen}", currentTalents.omen > 0 ? goldstyle : Style.Plain),
                new Text($"SoulAmp : {currentAttributes.soulAmp}", currentAttributes.soulAmp > 1 ? goldstyle : Style.Plain)
           );
            table.AddRow(
                new Text($"Block : {currentUpgrades.block}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 4 || modifiedUpgrades.Value.second == 4 || modifiedUpgrades.Value.third == 4) ? blueStyle : Style.Plain),
                new Text($"LL : {currentTalents.luckyLoot}", currentTalents.luckyLoot > 0 ? goldstyle : Style.Plain),
                new Text($"DeadMan : {currentAttributes.deadMan}", currentAttributes.deadMan > 0 ? goldstyle : Style.Plain)
          );
            table.AddRow(
                new Text($"Effect : {currentUpgrades.effect}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 5 || modifiedUpgrades.Value.second == 5 || modifiedUpgrades.Value.third == 5) ? blueStyle : Style.Plain),
                new Text($"POG : {currentTalents.presence}", currentTalents.presence > 0 ? goldstyle : Style.Plain),
                new Text($"SeaR : {currentAttributes.searious}", currentAttributes.searious > 1 ? goldstyle : Style.Plain)
          );
            table.AddRow(
               new Text($"CC : {currentUpgrades.chargechance}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 6 || modifiedUpgrades.Value.second == 6 || modifiedUpgrades.Value.third == 6) ? blueStyle : Style.Plain),
                new Text($"Finish : {currentTalents.finishing}", currentTalents.finishing > 0 ? goldstyle : Style.Plain),
                new Text($"Charge : {currentAttributes.passiveCharge}", currentAttributes.passiveCharge > 1 ? goldstyle : Style.Plain)
         );
            table.AddRow(
                new Text($"CA : {currentUpgrades.chargeamount}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 7 || modifiedUpgrades.Value.second == 7 || modifiedUpgrades.Value.third == 7) ? blueStyle : Style.Plain),
                new Text(" "),
                new Text($"King : {currentAttributes.kingTorpedo}", currentAttributes.kingTorpedo > 0 ? goldstyle : Style.Plain)
         );
            table.AddRow(
                new Text($"Reload : {currentUpgrades.attackspeed}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 8 || modifiedUpgrades.Value.second == 8 || modifiedUpgrades.Value.third == 8) ? blueStyle : Style.Plain),
                new Text(" "),
                new Text($"Elixir : {currentAttributes.elixir}", currentAttributes.elixir > 1 ? goldstyle : Style.Plain)
         );
            table.AddRow(
                new Text($"Salvo : {currentUpgrades.salvo}", modifiedUpgrades.HasValue && (modifiedUpgrades.Value.first == 9 || modifiedUpgrades.Value.second == 9 || modifiedUpgrades.Value.third == 9) ? blueStyle : Style.Plain),
                new Text(" "),
                new Text($"Shield : {currentAttributes.shield}", currentAttributes.shield > 0 ? goldstyle : Style.Plain)
         );
            
            
            table.Border = TableBorder.Minimal;
            AnsiConsole.Write(table);

            var table2 = new Table();
            table2.AddColumn(new TableColumn(" ").LeftAligned());
            table2.AddColumn(new TableColumn("Minimum").Centered());
            table2.AddColumn(new TableColumn("Average").Centered());
            table2.AddColumn(new TableColumn("Maximum").Centered());

            table2.AddRow(
                new Text("Time : "),
                new Text(formattedTimeMin),
                new Text(formattedTimeAvg),
                new Text(formattedTimeMax)
                );
            table2.AddRow(
               new Text("Stage : "),
               new Text(stageList.Min().ToString()),
               new Text(stageList.Average().ToString("F2")),
               new Text(stageList.Max().ToString())
               );
            table2.AddRow(
               new Text("LPM : "),
               new Text(lpmList.Min().ToString("F4")),
               new Markup("[bold green]" + lpmList.Average().ToString("F4") + "[/]"),
               new Text(lpmList.Max().ToString("F4"))
               );
            table2.AddRow(
               new Text("MaxGhosts at : "),
               new Text(nonZeroValues.Min().ToString()),
               new Text(nonZeroValues.Average().ToString("F2")),
               new Text(nonZeroValues.Max().ToString())
               );
            foreach (var column in table2.Columns)
            {
                column.Width(15);
            }
            table2.Border = TableBorder.Minimal;

            AnsiConsole.Write(table2);
            AnsiConsole.Write(" ");
            AnsiConsole.Write(new Text("Never Max Ghosts : " + percentage.ToString("F2") + " % ", percentage > 25 ? redStyle : Style.Plain));
            AnsiConsole.Write("\n");

            AnsiConsole.WriteLine("------------------------------------------------------------------------------------------");

        }

        ConsoleOutput();
        WriteResultsToFile();
    }
}

public class Talent
{
    // Variables representing each talent and their levels
    public int revive { get; set; }
    public int calypso { get; set; }
    public int unfair { get; set; }
    public int ghostBullet { get; set; }
    public int omen { get; set; }
    public int luckyLoot { get; set; }
    public int presence { get; set; }
    public int finishing { get; set; }

    public List<int> talentList = new List<int>();

    // Method to assign levels and calculate bonuses
    public void SetTalentList(List<int> talentLevels)
    {
        talentList = talentLevels;

        revive = talentLevels[0];
        calypso = talentLevels[1];
        unfair = talentLevels[2];
        ghostBullet = talentLevels[3];
        omen = talentLevels[4];
        luckyLoot = talentLevels[5];
        presence = talentLevels[6];
        finishing = talentLevels[7];
    }
}

public class Attributes
{
    // Variables representing each talent and their levels
    public int kraken { get; set; }
    public int armory { get; set; }
    public int pirate { get; set; }
    public int timeless { get; set; }
    public int soulAmp { get; set; }
    public int deadMan { get; set; }
    public int searious { get; set; }
    public int passiveCharge { get; set; }
    public int kingTorpedo { get; set; }
    public int elixir { get; set; }
    public int shield { get; set; }

    public List<int> attributeList = new List<int>();

    // Method to assign levels and calculate bonuses
    public void SetAttributeList(List<int> attributeLevels)
    {
        attributeList = attributeLevels;

        kraken = attributeLevels[0];
        armory = attributeLevels[1];
        pirate = attributeLevels[2];
        timeless = attributeLevels[3];
        soulAmp = attributeLevels[4];
        deadMan = attributeLevels[5];
        searious = attributeLevels[6];
        passiveCharge = attributeLevels[7];
        kingTorpedo = attributeLevels[8];
        elixir = attributeLevels[9];
        shield = attributeLevels[10];
    }
}

public class Upgrades
{
    public int health { get; set; }
    public int attack { get; set; }
    public int regen { get; set; }
    public int dr { get; set; }
    public int block { get; set; }
    public int effect { get; set; }
    public int chargechance { get; set; }
    public int chargeamount { get; set; }
    public int attackspeed { get; set; }
    public int salvo { get; set; }

    public List<int> upgradeList = new List<int>();

    public void SetUpgradesList(List<int> upgradeLevels)
    {
        upgradeList = upgradeLevels;

        health = upgradeLevels[0];
        attack = upgradeLevels[1];
        regen = upgradeLevels[2];
        dr = upgradeLevels[3];
        block = upgradeLevels[4];
        effect = upgradeLevels[5];
        chargechance = upgradeLevels[6];
        chargeamount = upgradeLevels[7];
        attackspeed = upgradeLevels[8];
        salvo = upgradeLevels[9];
    }
}

public class Config
{
    public int iterations { get; set; }
    public int knoxLevel { get; set; }
    public required List<int> upgrades { get; set; }
    public required List<int> talents { get; set; }
    public required List<int> attributes { get; set; }
}

public class ConfigFile
{
    public required List<Config> simulations { get; set; }
}


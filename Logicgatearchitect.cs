using System;
using System.Collections.Generic;
using Spectre.Console;

// --- Supporting Classes at the Top ---

abstract class LogicGate
{
    private string gateName = "";
    private int NumberOfInputs;
    private bool[] inputs = new bool[2];
    private bool output;

    public void SetInput(int index, bool value)
    {
        if (index >= 0 && index < inputs.Length)
            inputs[index] = value;
    }

    public bool GetOutput()
    {
        return output;
    }

    public string GetName()
    {
        return gateName;
    }

    public abstract bool Evaluate(bool inputA, bool inputB);
}

// Starting of inheritance 

class AndGate : LogicGate
{
    public override bool Evaluate(bool inputA, bool inputB)
    {
        return inputA && inputB;
    }
    public override string ToString()
    {
        return "AND";
    }
}

class OrGate : LogicGate
{
    public override bool Evaluate(bool inputA, bool inputB)
    {
        return inputA || inputB;
    }
    public override string ToString()
    {
        return "OR";
    }
}

class XorGate : LogicGate
{
    public override bool Evaluate(bool inputA, bool inputB)
    {
        return inputA ^ inputB;
    }
    public override string ToString()
    {
        return "XOR";
    }
}

class NandGate : LogicGate
{
    public override bool Evaluate(bool inputA, bool inputB)
    {
        return !(inputA && inputB);
    }
    public override string ToString()
    {
        return "NAND";
    }
}

class NorGate : LogicGate
{
    public override bool Evaluate(bool inputA, bool inputB)
    {
        return !(inputA || inputB);
    }
    public override string ToString()
    {
        return "NOR";
    }
}

// End of inheritance, start of class level

class Level
{
    public int LevelNumber { get; set; }
    public string GateName { get; set; }
    public LogicGate Gate { get; set; }
    public string Hint { get; set; }
    public List<(bool A, bool B)> TestCases { get; set; }

    public Level(int number, string name, LogicGate gate, string hint, List<(bool A, bool B)> testCases)
    {
        LevelNumber = number;
        GateName = name;
        Gate = gate;
        Hint = hint;
        TestCases = testCases;
    }

    public void DisplayTruthTable()
    {
        var table = new Table();
        table.AddColumn("[blue]Input A[/]");
        table.AddColumn("[blue]Input B[/]");
        table.AddColumn("[yellow]Expected Output[/]");

        foreach (var test in TestCases)
        {
            bool result = Gate.Evaluate(test.A, test.B);
            table.AddRow(
                test.A ? "[green]True[/]" : "[red]False[/]",
                test.B ? "[green]True[/]" : "[red]False[/]",
                result ? "[green]True[/]" : "[red]False[/]"
            );
        }
        AnsiConsole.Write(table);
    }
}

//end of class level, start of user's class

class User
{
    public string Name { get; set; }
    private int score;
    public int Score
    {
        get { return score; }
        set { score = value; }
    }
    public int CurrentLevelIndex { get; set; }
    public int RemainingAttempts { get; set; }
    public List<Item> Inventory { get; set; }

    public bool IsFiftyFiftyActive { get; set; }
    public bool IsDoublePointsActive { get; set; }
    public bool IsHintActive { get; set; }

    public User(string name)
    {
        Name = name;
        Score = 0;
        CurrentLevelIndex = 0;
        RemainingAttempts = 3;
        Inventory = new List<Item>();
    }

    public void AddScore(int points)
    {
        if (IsDoublePointsActive)
        {
            points *= 2;
            IsDoublePointsActive = false;
        }
        Score += points;
    }

    public void DecreaseAttempt()
    {
        RemainingAttempts--;
    }

    public void NextLevel()
    {
        CurrentLevelIndex++;
    }

    public bool IsGameOver()
    {
        return RemainingAttempts <= 0;
    }

    public void AddItem(Item item)
    {
        Inventory.Add(item);
    }

    public void UseItem(Item item)
    {
        item.Use(this);
        Inventory.Remove(item);
    }
}

//end of user class, start of item class

abstract class Item
{
    protected string itemName = "";
    protected string description = "";

    public string ItemName { get => itemName; set => itemName = value; }
    public string Description { get => description; set => description = value; }

    public abstract void Use(User player);
}

class FiftyFiftyItem : Item
{
    public FiftyFiftyItem()
    {
        ItemName = "50/50 Item";
        Description = "Removes 2 incorrect options randomly.";
    }

    public override void Use(User player)
    {
        player.IsFiftyFiftyActive = true;
    }
}

class DoublePointsItem : Item
{
    public DoublePointsItem()
    {
        ItemName = "Double Points";
        Description = "Doubles the score won in this round.";
    }

    public override void Use(User player)
    {
        player.IsDoublePointsActive = true;
    }
}

class HintItem : Item
{
    public HintItem()
    {
        ItemName = "Hint Item";
        Description = "Provides a small hint about the logic gate.";
    }

    public override void Use(User player)
    {
        player.IsHintActive = true;
    }
}

//end of item class, start of gamecontroller class

class GameController
{
    private User player;
    private List<Level> levels;
    private Level currentLevel;

    public void StartGame()
    {
        Console.Clear();
        RenderHeader();

        // EDIT: Removed the local variable 'User player;' here to avoid shadowing the main GameController 'player' field
        try
        {
            string name = AnsiConsole.Ask<string>("Enter your [green]Architect Name[/]:");

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Architect Name cannot be empty!");
            }

            player = new User(name);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Initialization Error:[/] {ex.Message}");
            AnsiConsole.MarkupLine("[yellow]Defaulting name to 'Guest Architect'...[/]");
            player = new User("Guest Architect");
        }

        // Give player one of each item to start
        player.AddItem(new FiftyFiftyItem());
        player.AddItem(new DoublePointsItem());
        player.AddItem(new HintItem());

        levels = InitializeLevels();

        ShowMenu();
    }

    public void ShowMenu()
    {
        while (!player.IsGameOver() && player.CurrentLevelIndex < levels.Count)
        {
            LoadLevel(player.CurrentLevelIndex);
        }
        EndGame();
    }

    public void LoadLevel(int levelNumber)
    {
        currentLevel = levels[levelNumber];
        bool levelCleared = PlayLevel(player, currentLevel);

        if (levelCleared)
        {
            player.AddScore(100);
            player.NextLevel();
            AnsiConsole.MarkupLine($"[bold green]Success![/] Level {currentLevel.LevelNumber} complete. Score: {player.Score}");
            if (player.CurrentLevelIndex < levels.Count)
            {
                AnsiConsole.MarkupLine("Press any key for the next level...");
                Console.ReadKey();
            }
        }
        else
        {
            player.DecreaseAttempt();
            AnsiConsole.MarkupLine($"[bold red]Incorrect![/] Attempts remaining: {player.RemainingAttempts}");
            AnsiConsole.MarkupLine($"[yellow]Hint: {currentLevel.Hint}[/]");

            if (player.IsGameOver())
            {
                AnsiConsole.MarkupLine("[bold darkred]GAME OVER.[/] Your logic failed you tonight.");
            }
            else
            {
                AnsiConsole.MarkupLine("Press any key to retry...");
                Console.ReadKey();
            }
        }
    }

    public void EndGame()
    {
        if (player.CurrentLevelIndex >= levels.Count)
        {
            AnsiConsole.Write(new FigletText("Congratulation!!").Color(Color.Gold1));
            AnsiConsole.MarkupLine($"[bold yellow]Congratulations, {player.Name}![/] You are a Master Logic Architect.");
            AnsiConsole.MarkupLine($"Final Score: [green]{player.Score}[/]");
        }
    }

    private bool PlayLevel(User player, Level level)
    {
        AnsiConsole.Clear();
        RenderHeader();
        AnsiConsole.Write(new Rule($"[yellow]Level {level.LevelNumber}: Identify the Gate[/]").Justify(Justify.Left));
        AnsiConsole.MarkupLine($"[bold white]Target:[/][blue] Fill in the truth table for this unknown gate.[/]");

        level.DisplayTruthTable();

        var availableGates = new List<string> { "AND", "OR", "XOR", "NAND", "NOR" };

        while (true)
        {
            var menuChoices = new List<string>(availableGates);
            if (player.Inventory.Count > 0)
            {
                menuChoices.Add("[yellow]Use Item[/]");
            }

            string selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which logic gate produces this behavior?")
                    .PageSize(10)
                    .AddChoices(menuChoices));

            if (selection == "[yellow]Use Item[/]")
            {
                var itemChoices = new List<string>();
                foreach (var itm in player.Inventory)
                {
                    itemChoices.Add(itm.ItemName);
                }
                itemChoices.Add("Cancel");

                string itemChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Choose an item to use:")
                    .AddChoices(itemChoices));

                if (itemChoice != "Cancel")
                {
                    Item selectedItem = player.Inventory.Find(i => i.ItemName == itemChoice);
                    if (selectedItem != null)
                    {
                        player.UseItem(selectedItem);
                        AnsiConsole.MarkupLine($"[green]Used {selectedItem.ItemName}![/]");

                        if (player.IsFiftyFiftyActive)
                        {
                            var wrongGates = new List<string>(availableGates);
                            wrongGates.Remove(level.GateName);

                            Random rnd = new Random();
                            int toRemove = Math.Min(2, wrongGates.Count - 1);
                            for (int i = 0; i < toRemove; i++)
                            {
                                int idx = rnd.Next(wrongGates.Count);
                                string wrongGate = wrongGates[idx];
                                wrongGates.RemoveAt(idx);
                                availableGates.Remove(wrongGate);
                            }
                            player.IsFiftyFiftyActive = false;
                        }
                        else if (player.IsHintActive)
                        {
                            AnsiConsole.MarkupLine($"[yellow]Hint:[/] {level.Hint}");
                            player.IsHintActive = false;
                        }
                    }
                }
                continue;
            }
            else
            {
                return selection.Equals(level.GateName, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    private void RenderHeader()
    {
        AnsiConsole.Write(new FigletText("Logic Gate").Color(Color.Blue));
        AnsiConsole.Write(new FigletText("Architect").Color(Color.Cyan1));
        AnsiConsole.Write(new Rule("[grey]OOP Puzzle Challenge[/]").Justify(Justify.Left));
        Console.WriteLine();
    }

    private List<Level> InitializeLevels()
    {
        return new List<Level>
        {
            new Level(1, "AND", new AndGate(), "Only outputs True if BOTH inputs are True.",
                new List<(bool, bool)> { (true, true), (true, false), (false, true), (false, false) }),

            new Level(2, "OR", new OrGate(), "Outputs True if AT LEAST ONE input is True.",
                new List<(bool, bool)> { (true, true), (true, false), (false, true), (false, false) }),

            new Level(3, "XOR", new XorGate(), "Outputs True if inputs are DIFFERENT.",
                new List<(bool, bool)> { (true, true), (true, false), (false, true), (false, false) }),

            new Level(4, "NAND", new NandGate(), "The inverse of AND. Only False when both are True.",
                new List<(bool, bool)> { (true, true), (true, false), (false, true), (false, false) }),

            new Level(5, "NOR", new NorGate(), "The inverse of OR. Only True when both are False",
                new List<(bool, bool)> { (true, true), (true, false), (false, true), (false, false) })
        };
    }
}

//end of gamecontroller class, start of program class
// --- Program Class at the Bottom ---
// This is where the main executed, like what we're learning in lecture

class Program
{
    static void Main(string[] args)
    {
        GameController controller = new GameController();
        controller.StartGame();
    }
}

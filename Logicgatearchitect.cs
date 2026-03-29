using System;
using System.Collections.Generic;
using System.Security;
using Spectre.Console;

// --- Supporting Classes at the Top ---

public abstract class LogicGate
{
    public abstract bool Evaluate(bool inputA, bool inputB);
}

// Starting of inheritance 

public class AndGate : LogicGate
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

public class OrGate : LogicGate
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

public class XorGate : LogicGate
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

public class NandGate : LogicGate
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

public class NorGate : LogicGate
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
// Interface requirement
public interface IUsable
{
    void Use(User player);
}

// Abstract Class requirement
public abstract class Item : IUsable
{
    public string Name { get; protected set; }
    public abstract void Use(User player); // Polymorphism: Overridden behavior
    public override string ToString()
    {
        return Name;
    }
}

// Item 1
public class DoublePointsItem : Item 
{
    public DoublePointsItem()
    {
        Name = "Double Points";
    }
    public override void Use(User player) 
    {
        AnsiConsole.MarkupLine("[bold gold1]Double Points Activated![/] Your next success will be worth 200 points.");
    }
}

// Item 2
public class LifeItem : Item
{
    public LifeItem()
    {
        Name = "Extra Life";
    }
    public override void Use(User player)
    {
        player.RemainingAttempts++;
        AnsiConsole.MarkupLine("[green]Extra Life added![/]");
    }
}

public class Level
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

// End of class level, start of user class

public class User
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

    public List<Item> Inventory { get; set; } = new List<Item>();

    public User(string name)
    {
        Name = name;
        Score = 0;
        CurrentLevelIndex = 0;
        RemainingAttempts = 3;
    }
}

// --- Program Class at the Bottom ---
// This is where the main executed, like what we're learning in lecture

public class GameEngine
{
    public List<Level> Levels { get; private set; }

    public GameEngine()
    {
        Levels = InitializeLevels();
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

    public bool PlayLevel(User player, Level level, ref bool doublePointsActive)
    {
        AnsiConsole.Clear();
        RenderHeader();
        AnsiConsole.Write(new Rule($"[yellow]Level {level.LevelNumber}[/]").Justify(Justify.Left));
        
        level.DisplayTruthTable();

        if (player.Inventory.Count > 0)
        {
            var useItem = AnsiConsole.Confirm("Would you like to use an item from your inventory?");
            if (useItem)
            {
                var itemToUse = AnsiConsole.Prompt(
                    new SelectionPrompt<Item>()
                        .Title("Select an item:")
                        .AddChoices(player.Inventory));

                if (itemToUse is DoublePointsItem)
                {
                    doublePointsActive = true;
                }
                
                itemToUse.Use(player);
                player.Inventory.Remove(itemToUse);
            }
        }

        string guess = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which logic gate is this?")
                .AddChoices(new[] { "AND", "OR", "XOR", "NAND", "NOR" }));

        return guess.Equals(level.GateName, StringComparison.OrdinalIgnoreCase);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        RenderHeader();

        User player;
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

        GameEngine engine = new GameEngine();

        player.Inventory.Add(new DoublePointsItem());
        player.Inventory.Add(new LifeItem());
        bool doublePointsActive = false;

        while (player.CurrentLevelIndex < engine.Levels.Count && player.RemainingAttempts > 0)
        {
            Level currentLevel = engine.Levels[player.CurrentLevelIndex];
            
            bool levelCleared = engine.PlayLevel(player, currentLevel, ref doublePointsActive);

            if (levelCleared)
            {
                int pointsEarned = doublePointsActive ? 200 : 100;
                player.Score += pointsEarned;

                doublePointsActive = false; 
                player.CurrentLevelIndex++;

                AnsiConsole.MarkupLine($"[bold green]Success![/] Level {currentLevel.LevelNumber} complete. Score: {player.Score}");
                if (player.CurrentLevelIndex < engine.Levels.Count)
                {
                    AnsiConsole.MarkupLine("Press any key for the next level...");
                    Console.ReadKey();
                }
            }
            else
            {
                doublePointsActive = false;
                player.RemainingAttempts--;
                AnsiConsole.MarkupLine($"[bold red]Incorrect![/] Attempts remaining: {player.RemainingAttempts}");
                AnsiConsole.MarkupLine($"[yellow]Hint: {currentLevel.Hint}[/]");
                
                if (player.RemainingAttempts <= 0)
                {
                    AnsiConsole.MarkupLine("[bold darkred]GAME OVER.[/] Your logic failed you tonight.");
                    break;
                }
                else 
                {
                    AnsiConsole.MarkupLine("Press any key to retry...");
                    Console.ReadKey();
                }
            }
        }

        if (player.CurrentLevelIndex == engine.Levels.Count)
        {
            AnsiConsole.Write(new FigletText("Congratulations!!").Color(Color.Gold1));
            AnsiConsole.MarkupLine($"[bold yellow]Congratulations, {player.Name}![/] You are a Master Logic Architect.");
            AnsiConsole.MarkupLine($"Final Score: [green]{player.Score}[/]");
        }
    }

    static void RenderHeader()
    {
        AnsiConsole.Write(new FigletText("Logic Gate").Color(Color.Blue));
        AnsiConsole.Write(new FigletText("Architect").Color(Color.Cyan1));
        AnsiConsole.Write(new Rule("[grey]OOP Puzzle Challenge[/]").Justify(Justify.Left));
        Console.WriteLine();
    }
}
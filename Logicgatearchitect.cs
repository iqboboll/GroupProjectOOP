using System;
using System.Collections.Generic;
using Spectre.Console;

// INTERFACE

public interface IUsable
{
    void Use(User player);
}

// ABSTRACT BASE CLASSES

public abstract class LogicGate
{
    public abstract bool Evaluate(bool inputA, bool inputB);
}

public abstract class Item : IUsable
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public abstract void Use(User player); // POLYMORPHISM
    
    public override string ToString() 
    { 
        return Name; 
    }
}

// LOGIC GATE INHERITANCE

class AndGate : LogicGate 
{ 
    public override bool Evaluate(bool a, bool b) 
    {
        return a && b;
    } 
    public override string ToString()
    {
        return "AND";
    } 
}

class OrGate : LogicGate 
{ 
    public override bool Evaluate(bool a, bool b)
    {
        return a || b;
    } 
    public override string ToString() 
    {
        return "OR";
    } 
}

class XorGate : LogicGate 
{ 
    public override bool Evaluate(bool a, bool b)
    {
        return a ^ b;
    } 
    public override string ToString()
    {
        return "XOR";
    } 
}

class NandGate : LogicGate 
{ 
    public override bool Evaluate(bool a, bool b)
    {
        return !(a && b);
    } 
    public override string ToString()
    {
        return "NAND";
    } 
}

class NorGate : LogicGate 
{ 
    public override bool Evaluate(bool a, bool b)
    {
        return !(a || b);
    } 
    public override string ToString() 
    {
        return "NOR";
    } 
}

// ITEM INHERITANCE 

class FiftyFiftyItem : Item
{
    public FiftyFiftyItem() 
    { 
        Name = "50/50 Item"; 
        Description = "Removes 2 incorrect options randomly."; 
    }
    public override void Use(User player) // POLYMORPHISM
    { 
        player.IsFiftyFiftyActive = true; 
    }
}

class DoublePointsItem : Item
{
    public DoublePointsItem() 
    { 
        Name = "Double Points"; 
        Description = "Doubles points for this round."; 
    }
    public override void Use(User player) // POLYMORPHISM
    { 
        player.IsDoublePointsActive = true; 
    }
}

class HintItem : Item
{
    public HintItem() 
    { 
        Name = "Hint Item"; 
        Description = "Provides a small hint about the logic gate."; 
    }
    public override void Use(User player) // POLYMORPHISM
    { 
        player.IsHintActive = true; 
    }
}

// CORE CLASSES

public class User
{
    public string Name { get; set; }
    private int score;
    public int Score // ENCAPSULATION
    { 
        get { return score; }
        set { score = value; }
    }
    public int CurrentLevelIndex { get; set; }
    public int RemainingAttempts { get; set; }
    public List<Item> Inventory { get; set; } = new List<Item>();

    public bool IsFiftyFiftyActive { get; set; }
    public bool IsDoublePointsActive { get; set; }
    public bool IsHintActive { get; set; }

    public User(string name)
    {
        Name = name;
        Score = 0;
        CurrentLevelIndex = 0;
        RemainingAttempts = 3;
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
        LevelNumber = number; GateName = name; Gate = gate; Hint = hint; TestCases = testCases;
    }

    public void DisplayTruthTable()
    {
        var table = new Table();
        table.AddColumn("[blue]Input A[/]");
        table.AddColumn("[blue]Input B[/]");
        table.AddColumn("[yellow]Expected Output[/]");

        foreach (var test in TestCases)
        {
            bool res = Gate.Evaluate(test.A, test.B);
            table.AddRow(
                test.A ? "[green]True[/]" : "[red]False[/]", 
                test.B ? "[green]True[/]" : "[red]False[/]", 
                res ? "[green]True[/]" : "[red]False[/]"
            );
        }
        AnsiConsole.Write(table);
    }
}

// GAME CONTROLLER

class GameController
{
    private User player;
    private List<Level> levels;

    public void StartGame()
    {
        Console.Clear();
        RenderHeader();

        try
        {
            string name = AnsiConsole.Ask<string>("Enter your [green]Architect Name[/]:");

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty!");
            }
        
            player = new User(name);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}. Defaulting to 'Guest'.");
            player = new User("Guest Architect");
        }

        levels = InitializeLevels();
        player.Inventory.Add(new FiftyFiftyItem());
        player.Inventory.Add(new DoublePointsItem());
        player.Inventory.Add(new HintItem());

        while (player.RemainingAttempts > 0 && player.CurrentLevelIndex < levels.Count)
        {
            PlayLevel(levels[player.CurrentLevelIndex]);
        }
        
        EndGame();
    }

    private void PlayLevel(Level level)
    {
        AnsiConsole.Clear();
        RenderHeader();
        AnsiConsole.Write(new Rule($"[yellow]Level {level.LevelNumber}[/]").Justify(Justify.Left));
        level.DisplayTruthTable();

        var choices = new List<string> { "AND", "OR", "XOR", "NAND", "NOR" };

        if (player.Inventory.Count > 0 && AnsiConsole.Confirm("Would you like to use an item from your inventory?"))
        {
            var item = AnsiConsole.Prompt(new SelectionPrompt<Item>().Title("Pick an item:").AddChoices(player.Inventory));
            item.Use(player);
            player.Inventory.Remove(item);

            if (player.IsFiftyFiftyActive)
            {
                AnsiConsole.MarkupLine("[bold yellow]50/50 Activated![/] Two incorrect gates have been removed.");
                var wrong = choices.FindAll(c => c != level.GateName);
                if (wrong.Count >= 2) { choices.Remove(wrong[0]); choices.Remove(wrong[1]); }
                player.IsFiftyFiftyActive = false;
            }
            
            if (player.IsDoublePointsActive)
            {
                AnsiConsole.MarkupLine("[bold gold1]Double Points Activated![/] Your success reward is now [green]200 points[/].");
            }

            if (player.IsHintActive)
            {
                AnsiConsole.MarkupLine($"[bold cyan]Hint Activated![/] Secret: [yellow]{level.Hint}[/]");
                player.IsHintActive = false;
            }
            
            Console.WriteLine("\nPress any key to continue to your choice...");
            Console.ReadKey();
        }

        string guess = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Which logic gate is this?").AddChoices(choices));

        if (guess.Equals(level.GateName, StringComparison.OrdinalIgnoreCase))
        {
            int gain = player.IsDoublePointsActive ? 200 : 100;
            player.Score += gain;
            player.IsDoublePointsActive = false;
            player.CurrentLevelIndex++;
            AnsiConsole.MarkupLine($"[green]Correct![/] Score: {player.Score}");
            Console.ReadKey();
        }
        else
        {
            player.RemainingAttempts--;
            player.IsDoublePointsActive = false;
            AnsiConsole.MarkupLine($"[red]Wrong![/] Attempts left: {player.RemainingAttempts}");
            Console.ReadKey();
        }
    }

    private void EndGame()
    {
        if (player.RemainingAttempts > 0)
            AnsiConsole.Write(new FigletText("Winner!").Color(Color.Gold1));
        else
            AnsiConsole.MarkupLine("[red]GAME OVER.[/]");
    }

    private void RenderHeader()
    {
        AnsiConsole.Write(new FigletText("Logic Gate").Color(Color.Blue));
        AnsiConsole.Write(new FigletText("Architect").Color(Color.Cyan1));
        AnsiConsole.Write(new Rule("[grey]OOP Puzzle Challenge[/]").Justify(Justify.Left));
    }

    private List<Level> InitializeLevels()
    {
        var fullCases = new List<(bool, bool)> { (true, true), (true, false), (false, true), (false, false) };

        return new List<Level> 
        {
            new Level(1, "AND", new AndGate(), "Only outputs True if BOTH inputs are True.", new List<(bool, bool)>(fullCases)),
            new Level(2, "OR", new OrGate(), "Outputs True if AT LEAST ONE input is True.", new List<(bool, bool)>(fullCases)),
            new Level(3, "XOR", new XorGate(), "Outputs True if inputs are DIFFERENT.", new List<(bool, bool)>(fullCases)),
            new Level(4, "NAND", new NandGate(), "The inverse of AND. Only False when both are True.", new List<(bool, bool)>(fullCases)),
            new Level(5, "NOR", new NorGate(), "The inverse of OR. Only True when both are False.", new List<(bool, bool)>(fullCases))
        };
    }
}

class Program
{
    static void Main() 
    { 
        GameController controller = new GameController();
        controller.StartGame();
    }
}
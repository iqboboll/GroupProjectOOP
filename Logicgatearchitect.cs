using System;
using System.Collections.Generic;
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

//end of class level, start of user's class

public class User
{
    public string Name { get; set; }
    public int Score { get; set; }
    public int CurrentLevelIndex { get; set; }
    public int RemainingAttempts { get; set; }

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

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        RenderHeader();

        string name = AnsiConsole.Ask<string>("Enter your [green]Architect Name[/]:");
        User player = new User(name);

        List<Level> levels = InitializeLevels();

        while (player.CurrentLevelIndex < levels.Count && player.RemainingAttempts > 0)
        {
            Level currentLevel = levels[player.CurrentLevelIndex];
            bool levelCleared = PlayLevel(player, currentLevel);

            if (levelCleared)
            {
                player.Score += 100;
                player.CurrentLevelIndex++;
                AnsiConsole.MarkupLine($"[bold green]Success![/] Level {currentLevel.LevelNumber} complete. Score: {player.Score}");
                if (player.CurrentLevelIndex < levels.Count)
                {
                    AnsiConsole.MarkupLine("Press any key for the next level...");
                    Console.ReadKey();
                }
            }
            else
            {
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

        if (player.CurrentLevelIndex == levels.Count)
        {
            AnsiConsole.Write(new FigletText("Congratulation!!").Color(Color.Gold1));
            AnsiConsole.MarkupLine($"[bold yellow]Congratulations, {player.Name}![/] You are a Master Logic Architect.");
            AnsiConsole.MarkupLine($"Final Score: [green]{player.Score}[/]");
        }
    }

    static bool PlayLevel(User player, Level level)
    {
        AnsiConsole.Clear();
        RenderHeader();
        AnsiConsole.Write(new Rule($"[yellow]Level {level.LevelNumber}: Identify the Gate[/]").Justify(Justify.Left));
        AnsiConsole.MarkupLine($"[bold white]Target:[/][blue] Fill in the truth table for this unknown gate.[/]");
        
        level.DisplayTruthTable();

        string guess = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which logic gate produces this behavior?")
                .PageSize(5)
                .AddChoices(new[] { "AND", "OR", "XOR", "NAND", "NOR" }));

        return guess.Equals(level.GateName, StringComparison.OrdinalIgnoreCase);
    }

    static void RenderHeader()
    {
        AnsiConsole.Write(new FigletText("Logic Gate").Color(Color.Blue));
        AnsiConsole.Write(new FigletText("Architect").Color(Color.Cyan1));
        AnsiConsole.Write(new Rule("[grey]OOP Puzzle Challenge[/]").Justify(Justify.Left));
        Console.WriteLine();
    }

    static List<Level> InitializeLevels()
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

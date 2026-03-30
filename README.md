# Logic Gate Architect

> An interactive OOP puzzle game that tests your knowledge of digital logic gates through truth table analysis.

---

## Group Members

| No. |                   Name                     | Student ID |
|-----|--------------------------------------------|------------|
| 1   | Muhammad Airil Asyraff Bin Zulkifli        |  25009515  |
| 2   | Muhammad Iqbal bin Abdul Razak             |  25006561  |
| 3   | Isma Yasmin Binti Ramli                    |  24005367  |
| 4   | Nursyazwani Binti Zulkufli                 |  24005408  |
| 5   | Zarifah Izzati Binti Zaini                 |  24005368  |
| 6   | Nurul Ieman Binti Abdullah                 |  24005565  |
| 7   | Argya Muhammad Raifan                      |  25005723  |
| 8   | Sharifah Syazwahida Binti Syd Roselan      |  24005605  |

---

## Project Description

The **Logic Gate Architect** is an interactive logic puzzle that tests a player's knowledge of digital circuitry through a series of increasingly difficult levels. Upon starting, the player takes on the role of an "Architect" and is presented with a truth table, showing how a gate reacts to different combinations of `True` and `False` inputs. The player's goal is to analyze these patterns and correctly identify the gate from a list of options, such as AND, OR, XOR, NAND, or NOR. Each correct identification earns the player **100 points** and advances them to the next stage of the challenge.

To win the game, the player must successfully navigate through all five levels of the architectural challenge. However, the system enforces a strict penalty for incorrect deductions: players start with a limited pool of **three attempts**. For each level, a truth table is displayed, and the player is given the opportunity to use an item from their inventory to gain a strategic advantage. After the optional item phase, the player must select a logic gate guess. A correct identification earns the player points and advances them to the next level. Conversely, an incorrect guess results in the deduction of one attempt. The game concludes in one of two states: a **"Winner"** message is displayed if all levels are successfully cleared, or a **"Game Over"** message appears if the player exhausts all three attempts.

---

## System Features

| Feature | Description |
|---------|-------------|
| Truth Table Display | Displays a formatted truth table using Spectre.Console for each level |
| 5 Logic Gate Levels | Covers AND, OR, XOR, NAND, and NOR gates with progressive difficulty |
| Scoring System | Awards 100 points per correct answer (200 with Double Points item) |
| Attempt System | Players have 3 attempts; wrong answers reduce remaining attempts |
| Inventory & Items | Players can use items (50/50, Double Points, Hint) to gain advantages |
| 50/50 Item | Removes 2 incorrect options from the answer choices |
| Double Points Item | Doubles the point reward for the current round |
| Hint Item | Reveals a contextual clue about the current logic gate |
| Win/Lose Conditions | Completing all levels grants the "Winner" title; losing all attempts triggers Game Over |

---

## OOP Concepts Used

| OOP Concept | Implementation | Code Example |
|-------------|----------------|--------------|
| **Abstraction** | `LogicGate` and `Item` are abstract base classes that define a common interface without full implementation | `public abstract class LogicGate` with `abstract bool Evaluate(bool, bool)` |
| **Inheritance** | Gate classes (`AndGate`, `OrGate`, `XorGate`, `NandGate`, `NorGate`) extend `LogicGate`; Item classes (`FiftyFiftyItem`, `DoublePointsItem`, `HintItem`) extend `Item` | `class AndGate : LogicGate` |
| **Polymorphism** | Each gate subclass overrides `Evaluate()` with its own logic; each item subclass overrides `Use()` with unique behaviour | `public override bool Evaluate(bool a, bool b) => a && b;` |
| **Encapsulation** | `User.Score` uses a private backing field (`score`) exposed through a public property | `private int score;` with `public int Score { get; set; }` |
| **Interface** | `IUsable` interface defines a `Use(User)` contract, implemented by the `Item` class hierarchy | `public interface IUsable { void Use(User player); }` |

---

## How to Run the Program

### Prerequisites

| Requirement | Details |
|-------------|---------|
| IDE | Visual Studio 2022 or later |
| Framework | .NET 6.0 or later |
| NuGet Package | `Spectre.Console` |

### Steps

1. Open `LogicGateArchitect.sln` in Visual Studio
2. Restore NuGet packages (Visual Studio does this automatically on build)
3. Build the project (`Ctrl + Shift + B`)
4. Run using **Start Debugging** (`F5`)

---

## Project Structure

| File | Description |
|------|-------------|
| `Logicgatearchitect.cs` | Main source file containing all game classes (gates, items, user, levels, game controller) |
| `Logicgatearchitect.csproj` | C# project configuration file with dependencies |
| `LogicGateArchitect.sln` | Visual Studio solution file |
| `Program.cs` *(within Logicgatearchitect.cs)* | Entry point — instantiates `GameController` and starts the game |
| `README.md` | Project documentation |
| `.gitignore` | Specifies files excluded from version control |

### Class Overview

| Class / Interface | Type | Purpose |
|-------------------|------|---------|
| `IUsable` | Interface | Defines `Use(User)` contract for usable items |
| `LogicGate` | Abstract Class | Base class for all logic gate implementations |
| `AndGate`, `OrGate`, `XorGate`, `NandGate`, `NorGate` | Concrete Class | Individual logic gate evaluators |
| `Item` | Abstract Class | Base class for all inventory items |
| `FiftyFiftyItem`, `DoublePointsItem`, `HintItem` | Concrete Class | Usable power-up items with unique effects |
| `User` | Class | Stores player data (name, score, attempts, inventory) |
| `Level` | Class | Represents a game level with gate, hint, and truth table |
| `GameController` | Class | Orchestrates game flow, input handling, and state management |
| `Program` | Class | Application entry point |

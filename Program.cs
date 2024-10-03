using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Quic;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Text;

Console.InputEncoding = UnicodeEncoding.UTF8;
string jsonQuizDataString = File.ReadAllText(GetQuizDataFilePath());

Dictionary<string, string> atomDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonQuizDataString);

List<Atom> atoms = new List<Atom>();
foreach (var kvp in atomDictionary)
{
    atoms.Add(new Atom { name = kvp.Key, symbol = kvp.Value });
}

Console.Clear();

int score = 0;

void CaseCorrectAnswer()
{
    score++;
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Correct!");
    Console.ForegroundColor = ConsoleColor.Gray;
}

void CaseWrongAnswer(string correctAnswer)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Wrong!");
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine($"Correct answer is: {correctAnswer}");
    Console.ForegroundColor = ConsoleColor.Gray;
}

Console.CursorVisible = false;
int currentCursorTop = 4;
string navigationCharacter = "=>";

while (true)
{
    Console.Clear();
    Console.WriteLine("Welcome to Chemistry Quiz!");
    Console.WriteLine("What would you like to do?\n\n");
    Console.WriteLine("  1. Start Quiz");
    Console.WriteLine("  2. Create Quiz From Unformatted List");
    Console.WriteLine("  3. Edit/See Quiz Manually");
    Console.WriteLine("  4. Quit");
    Console.WriteLine("\n\n\n");
    Console.WriteLine("Navigate with UP & DOWN Arrows and Enter to Confirm.");

    Console.CursorTop = currentCursorTop;
    Console.Write(navigationCharacter);

    ConsoleKey inputCharacter = Console.ReadKey().Key;

    if (inputCharacter == ConsoleKey.UpArrow && currentCursorTop > 4)
    {
        Console.SetCursorPosition(0, currentCursorTop);
        Console.Write("  ");
        currentCursorTop--;
    }
    else if (inputCharacter == ConsoleKey.DownArrow && currentCursorTop < 7)
    {
        Console.SetCursorPosition(0, currentCursorTop);
        Console.Write("  ");
        currentCursorTop++;
    }

    Console.SetCursorPosition(0, currentCursorTop);
    Console.Write(navigationCharacter);

    if (inputCharacter == ConsoleKey.Enter)
    {
        Console.Clear();
        if (currentCursorTop == 4)
        {
            StartQuiz();
        }
        else if (currentCursorTop == 5)
        {
            Console.WriteLine("(Warning: the content of the file will be overwritten) Paste the unformatted text here:");
            string unformattedQuiz = Console.ReadLine();

            string formattedQuiz = FormatQuiz(unformattedQuiz);
            Console.WriteLine("The quiz will be formatted as follows:");
            Console.WriteLine(formattedQuiz);
            Console.WriteLine("Do you want to apply changes? (y)/(n)");
            string answer = Console.ReadLine();
            if (answer == "y")
                File.WriteAllText(GetQuizDataFilePath(), formattedQuiz);
        }
        else if (currentCursorTop == 6)
        {
            EditQuiz();
        }
        else if (currentCursorTop == 7)
        {
            break;
        }
    }
}

void EditQuiz()
{
    Process.Start("Notepad.exe", GetQuizDataFilePath());
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
}

void AskQuestions(List<Atom> atoms, string type)
{
    Random random = new Random();
    while (atoms.Count > 0)
    {
        int randomIndex = random.Next(atoms.Count);
        Atom atom = atoms[randomIndex];

        Console.WriteLine($"What is the {type} (type \"Exit\" to quit):\n");

        if (type == "symbol")
        {
            // Show the symbol, expect the name as input
            Console.WriteLine(atom.symbol);
            Console.Write("Your answer (name): ");
            string answer = Console.ReadLine()?.Trim(); // Trim whitespace

            if (answer.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                return;

            // Normalize both the input and correct answer to account for encoding issues
            if (Normalize(answer).Equals(Normalize(atom.name), StringComparison.OrdinalIgnoreCase))
                CaseCorrectAnswer();
            else
                CaseWrongAnswer(atom.name); // Correct answer is the name
        }
        else if (type == "name")
        {
            // Show the name, expect the symbol as input
            Console.WriteLine(atom.name);
            Console.Write("Your answer (symbol): ");
            string answer = Console.ReadLine()?.Trim(); // Trim whitespace

            if (answer.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                return;

            // Normalize both the input and correct answer to account for encoding issues
            if (Normalize(answer).Equals(Normalize(atom.symbol), StringComparison.OrdinalIgnoreCase))
                CaseCorrectAnswer();
            else
                CaseWrongAnswer(atom.symbol); // Correct answer is the symbol
        }

        atoms.RemoveAt(randomIndex);
    }
}

// Helper function to normalize strings for consistent Unicode handling
string Normalize(string input)
{
    return input.Normalize(NormalizationForm.FormC).Trim(); // Normalize Unicode characters
}



void StartQuiz()
{
    List<Atom> atomsCopy = new List<Atom>(atoms);
    AskQuestions(atomsCopy, "symbol");
    Console.Clear();

    atomsCopy = new List<Atom>(atoms); // Reset the list for the second round
    AskQuestions(atomsCopy, "name");

    Console.WriteLine($"Your score is: {score} / {atoms.Count * 2}");
    Console.ReadKey();
}

static string GetQuizDataFilePath()
{
    string path = Path.Combine(Directory.GetCurrentDirectory(), "QuizData.json");

    if (!File.Exists(path))
    {
        File.Create(path).Close();
    }

    return path;
}

static string FormatQuiz(string quiz)
{
    quiz = quiz.Trim();

    // Replace newline breaks with commas and ensure proper JSON format
    quiz = Regex.Replace(quiz, @"\s*=\s*", ":");
    quiz = Regex.Replace(quiz, @"\s+", ""); // Remove excess whitespace
    quiz = quiz.ReplaceLineEndings(",");
    quiz = Regex.Replace(quiz, @"(\w+)", "\"$1\"");

    return quiz;
}

public class Atom
{
    public string name { get; set; }
    public string symbol { get; set; }
}

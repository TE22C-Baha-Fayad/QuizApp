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
static void CaseWrongAnswer(string atomeNameOrSymbol)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Wrong!");

    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("correct Answer is " + atomeNameOrSymbol);
    Console.ForegroundColor = ConsoleColor.Gray;
}


Console.CursorVisible = false;
Console.CursorLeft = 0;
int currentCursorTop = 4;
while (true)
{

    Console.Clear();
    System.Console.WriteLine("Welcom to Chemistry Quiz!");
    System.Console.WriteLine("what are you willing to do?\n\n");
    System.Console.WriteLine("  1.Start Quiz");
    System.Console.WriteLine("  2.Create Quiz From Unformatted List");
    System.Console.WriteLine("  2.Edit/See Quiz Manually");
    System.Console.WriteLine("  3.Quit");
    System.Console.WriteLine("\n\n\n");
    System.Console.WriteLine("Navigate with UP & DOWN Arrows and Enter to Confirm.");
    string navigationCharacter = "=>";
    Console.CursorTop = currentCursorTop;
    Console.Write(navigationCharacter);
    ConsoleKey inputCharacter = Console.ReadKey().Key;

    if (inputCharacter == ConsoleKey.UpArrow && Console.CursorTop != 4)
    {
        Console.CursorTop--;
        currentCursorTop = Console.CursorTop;
        Console.Write(navigationCharacter);
    }
    else if (inputCharacter == ConsoleKey.DownArrow && Console.CursorTop != 7)
    {
        Console.CursorTop++;
        currentCursorTop = Console.CursorTop;
        Console.Write(navigationCharacter);
    }
    if (inputCharacter == ConsoleKey.Enter)
    {
        Console.Clear();
        if (currentCursorTop == 4)
        {
            StartQuiz();
        }
        else if (currentCursorTop == 5)
        {
            System.Console.WriteLine("(Warning the content of the File will be overwritten) Paste the Unformatted text here:");
            string unformattedQuiz = Console.ReadLine();

            string formattedQuiz = FormatQuiz(unformattedQuiz);
            Console.WriteLine("the quiz will be formatted as following:");
            System.Console.WriteLine(formattedQuiz);
            System.Console.WriteLine("Do you want to apply changes? (y)/(n)");
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
    System.Console.WriteLine("press anything to continue...");
    Console.ReadKey();
}

void AskQuestions(List<Atom> atomerna, string symbolOrName)
{
    while (true)
    {
        
        if (atomerna.Count == 0)
            break;
        int randomInt = new Random().Next(0, atomerna.Count);
        Atom atom = atomerna[randomInt];

        Console.WriteLine($"What is the name of the following {symbolOrName}(to quit Type \"Exit\"):\n");
        if (symbolOrName == "symbol")
        {
            Console.WriteLine(atom.symbol);
            Console.Write("Your Answer: ");
            string answer = Console.ReadLine();
            if (answer == "Exit")
            {
                return;
            }
            else if (answer == atom.name)
            {
                CaseCorrectAnswer();
            }
            else
            {
                CaseWrongAnswer(atom.name);
            }
        }
        else if (symbolOrName == "name")
        {
            Console.WriteLine(atom.name);
            Console.Write("Your Answer: ");
            string answer = Console.ReadLine();
            if (answer == "Exit")
            {
                return;
            }
            else if (answer == atom.symbol)
            {
                CaseCorrectAnswer();
            }
            else
            {
                CaseWrongAnswer(atom.symbol);
            }
        }
        atomerna.Remove(atom);
    }


}




void StartQuiz()
{

List<Atom> atomsCopy = atoms;
List<Atom> atomsCopy1 = atoms;
    AskQuestions(atomsCopy, "symbol");
    Console.ReadKey();
    Console.Clear();
    AskQuestions(atomsCopy1, "name");
    Console.ReadKey();
    Console.WriteLine("your score is:" + score + "/" + atoms.Count * 2);
}



static string GetQuizDataFilePath()
{
    string[] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory());
    foreach (string filepath in filePaths)
    {
        Console.WriteLine(filepath);
        if (filepath.Contains("QuizData.json", StringComparison.OrdinalIgnoreCase))
        {
            return filepath;
        }
    }
    const string fileName = "QuizData.Json";
    string path = Directory.GetCurrentDirectory()+"\\"+fileName;
    File.Create(path);
    return path;
}
static string FormatQuiz(string quiz)
{


    quiz = quiz.Trim();
    foreach (char character in quiz)
    {
        quiz = Regex.Replace(quiz, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
        quiz = quiz.ReplaceLineEndings(",");
        quiz = Regex.Replace(quiz, "\"+", "");
        quiz = Regex.Replace(quiz, @"\w+", "\"$0\"");


        if (character == '=')
        {
            quiz = quiz.Replace(character, ':');
        }
    }
    return quiz;
}
public class Atom
{
    public string name;
    public string symbol;
}
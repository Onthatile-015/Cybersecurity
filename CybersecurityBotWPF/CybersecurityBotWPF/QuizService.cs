using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityBotWPF
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public string Type { get; set; } // "mcq" or "truefalse"
    }

    internal class QuizService
    {
        //Quiz questions
        private readonly List<QuizQuestion> _questions = new List<QuizQuestion>()
        {
            new QuizQuestion
            {
                Type = "mcq",
                Question = "What should you do if you receive an email asking for your password?",
                Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
                CorrectAnswer = "c",
                Explanation = "Reporting phishing emails helps prevent scams and protects others."
            },
            new QuizQuestion
            {
                Type = "truefalse",
                Question = "True or False: Using the same password for all accounts is safe.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "b",
                Explanation = "Using the same password everywhere is dangerous. If one account is hacked, all accounts are at risk."
            },
            new QuizQuestion
            {
                Type = "mcq",
                Question = "What does MFA stand for?",
                Options = new List<string> { "A) Multiple File Access", "B) Multi-Factor Authentication", "C) Managed Firewall Application", "D) Mobile Feature Activation" },
                CorrectAnswer = "b",
                Explanation = "MFA adds an extra layer of security beyond just a password."
            },
            new QuizQuestion
            {
                Type = "truefalse",
                Question = "True or False: Public Wi-Fi is always safe to use for banking.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "b",
                Explanation = "Public Wi-Fi is unsecured. Always use a VPN for sensitive activities."
            },
            new QuizQuestion
            {
                Type = "mcq",
                Question = "What is ransomware?",
                Options = new List<string> { "A) Software that speeds up your PC", "B) A type of antivirus", "C) Malware that encrypts files and demands payment", "D) A firewall tool" },
                CorrectAnswer = "c",
                Explanation = "Ransomware locks your files and demands payment. Regular backups are your best defence."
            },
            new QuizQuestion
            {
                Type = "mcq",
                Question = "Which of these is the strongest password?",
                Options = new List<string> { "A) password123", "B) John1990", "C) qwerty", "D) T#9kL!mZ2@pQ" },
                CorrectAnswer = "d",
                Explanation = "Strong passwords use uppercase, lowercase, numbers and special characters."
            },
            new QuizQuestion
            {
                Type = "truefalse",
                Question = "True or False: Clicking links in emails from unknown senders is safe.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "b",
                Explanation = "Never click links from unknown senders — they may lead to phishing sites."
            },
            new QuizQuestion
            {
                Type = "mcq",
                Question = "What is a VPN used for?",
                Options = new List<string> { "A) Making your PC faster", "B) Encrypting your internet connection", "C) Blocking all websites", "D) Installing antivirus" },
                CorrectAnswer = "b",
                Explanation = "A VPN encrypts your internet traffic keeping your data safe."
            },
            new QuizQuestion
            {
                Type = "truefalse",
                Question = "True or False: Antivirus software alone is enough to protect your device.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "b",
                Explanation = "Antivirus is important but you also need updates, strong passwords, and safe browsing habits."
            },
            new QuizQuestion
            {
                Type = "mcq",
                Question = "What is the 3-2-1 backup rule?",
                Options = new List<string> { "A) 3 passwords, 2 devices, 1 cloud", "B) 3 copies, 2 media types, 1 offsite", "C) 3 backups per day, 2 per week, 1 per month", "D) None of the above" },
                CorrectAnswer = "b",
                Explanation = "The 3-2-1 rule means 3 copies of data, on 2 different media, with 1 stored offsite."
            },
            new QuizQuestion
            {
                Type = "truefalse",
                Question = "True or False: Social engineering attacks target software vulnerabilities.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "b",
                Explanation = "Social engineering targets people, not software. It manipulates humans into giving up information."
            },
            new QuizQuestion
            {
                Type = "mcq",
                Question = "Which of these is a sign of a phishing email?",
                Options = new List<string> { "A) It comes from your boss", "B) It has urgent language and asks for personal info", "C) It has your full name", "D) It comes from a known website" },
                CorrectAnswer = "b",
                Explanation = "Phishing emails often create urgency and ask for sensitive information."
            }
        };

        private int _currentIndex = 0;
        private int _score = 0;
        private bool _isActive = false;
        private readonly Random _rng = new Random();
        private List<QuizQuestion> _shuffled;

        public bool IsActive => _isActive;
        public int Score => _score;
        public int Total => _shuffled?.Count ?? 0;
        public int CurrentIndex => _currentIndex;

        public string StartQuiz()
        {
            _shuffled = new List<QuizQuestion>(_questions);
            // Shuffle questions
            for (int i = _shuffled.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                var temp = _shuffled[i];
                _shuffled[i] = _shuffled[j];
                _shuffled[j] = temp;
            }
            _currentIndex = 0;
            _score = 0;
            _isActive = true;
            return "Cybersecurity Quiz Started!\n\n" +
                   $"You will answer {_shuffled.Count} questions.\n" +
                   "Type the letter of your answer (A, B, C or D)\n\n" +
                   GetCurrentQuestion();
        }

        public string GetCurrentQuestion()
        {
            if (_currentIndex >= _shuffled.Count)
                return EndQuiz();

            var q = _shuffled[_currentIndex];
            string options = string.Join("\n", q.Options);
            return $"Question {_currentIndex + 1} of {_shuffled.Count}:\n\n" +
                   $"{q.Question}\n\n{options}";
        }

        public string AnswerQuestion(string answer)
        {
            if (!_isActive) return "";

            var q = _shuffled[_currentIndex];
            string userAnswer = answer.ToLower().Trim();
            bool correct = userAnswer == q.CorrectAnswer;

            if (correct) _score++;
            _currentIndex++;

            string feedback = correct
                ? $"Correct! {q.Explanation}\n\n"
                : $"Wrong! The correct answer was {q.CorrectAnswer.ToUpper()}.\n{q.Explanation}\n\n";

            if (_currentIndex >= _shuffled.Count)
                return feedback + EndQuiz();

            return feedback + GetCurrentQuestion();
        }
        //Ending quiz
        private string EndQuiz()
        {
            _isActive = false;
            string grade = _score >= 10 ? " Cybersecurity Pro!" :
                           _score >= 7 ? " Great job!" :
                           _score >= 5 ? " Good effort!" :
                                          " Keep learning!";

            return $"Quiz Complete!\n\n" +
                   $"Your Score: {_score}/{_shuffled.Count}\n" +
                   $"{grade}\n\n" +
                   "Type 'start quiz' to try again!";
        }


    }
}

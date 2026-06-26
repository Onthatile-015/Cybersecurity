using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityBotWPF
{
    class ChatBot
    {
        private readonly List<string> _activityLog = new List<string>();
        private readonly ResponseHandler _responseHandler = new ResponseHandler();
        private readonly MemoryService _memory = new MemoryService();
        private readonly SentimentDetector _sentiment = new SentimentDetector();
        private readonly DatabaseService _db = new DatabaseService();
        private readonly QuizService _quiz = new QuizService();

        //Conversation Flow: track last topic
        private string _lastTopic = "";
        private bool _awaitingName = true;

        //Follow up triggers
        private readonly List<string> _followUpTriggers = new List<string>

        {
            "more", "another", "again", "explain",
            "tell me more", "give me another",
            "continue", "elaborate", "details", "further"
        };

        public string GetResponse(string input)
        {
            string lower = input.ToLower().Trim();

            //It asks for name first
            if (_awaitingName)
            {
                string name = ExtractName(input);
                _memory.Remember("name", name);
                _awaitingName = false;
                _activityLog.Add($"[{DateTime.Now:HH:mm}] User identified as: {name}");
                return $"Nice to meet you, {name}! \n\n" +
                       "What cybersecurity topic can I help you with today?.\n\n"
                       ;
            }

            // Follow-up / continuation
            if (_followUpTriggers.Any(t => lower.Contains(t))
                && !string.IsNullOrEmpty(_lastTopic))
            {
                string followUp = _responseHandler.GetResponse(_lastTopic);
                return $"Here's more on {_lastTopic}:\n\n{followUp}";
            }
            //SQL Adding task
            if (lower.Contains("add task") || lower.Contains("new task")
       || lower.Contains("create task"))
            {
                string title = input.Length > 9 ? input.Substring(9).Trim() : "Cybersecurity Task";
                bool added = _db.AddTask(title, "Added via chatbot", "");
                _lastTopic = "task";
                if (added)
                    _activityLog.Add($"[{DateTime.Now:HH:mm}] Task added: '{title}'");
                return added
                    ? $"Task added: '{title}'\n\nType 'remind me in X days' to set a reminder!"
                    : "Could not add task. Check your database connection.";
            }
            //Adding tasks
            if (lower.Contains("show tasks") || lower.Contains("my tasks")
                || lower.Contains("view tasks") || lower.Contains("list tasks"))
            {
                var tasks = _db.GetAllTasks();
                if (tasks.Count == 0)
                    return "No tasks yet! Type 'add task [name]' to add one.";

                string result = " Your Tasks:\n\n";
                foreach (var t in tasks)
                {
                    string status = t.IsCompleted ? "" : "";
                    string reminder = string.IsNullOrEmpty(t.ReminderDate)
                        ? "" : $" | {t.ReminderDate}";
                    result += $"{status} [{t.Id}] {t.Title}{reminder}\n";
                }
                return result;
            }
            //setting reminders
            if (lower.Contains("remind me in") || lower.Contains("set reminder"))
            {
                var tasks = _db.GetAllTasks();
                if (tasks.Count == 0)
                    return "You have no tasks yet. Add one first: 'add task [name]'";

                string date = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd");
                foreach (var word in lower.Split(' '))
                {
                    if (int.TryParse(word, out int days))
                    {
                        date = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                        break;
                    }
                }
                var latest = tasks[0];
                _db.AddTask(latest.Title, latest.Description, date);
                _activityLog.Add($"[{DateTime.Now:HH:mm}] Reminder set for '{latest.Title}' on {date}");
                return $"Reminder set for '{latest.Title}' on {date}!";
            }

            if (lower.Contains("complete task") || lower.Contains("done task")
                || lower.Contains("mark task"))
            {
                foreach (var word in lower.Split(' '))
                {
                    if (int.TryParse(word, out int id))
                    {
                        bool done = _db.CompleteTask(id);
                        if (done)
                            _activityLog.Add($"[{DateTime.Now:HH:mm}] Task {id} marked as complete");
                        return done
                            ? $"Task {id} marked as complete! Great work!"
                            : $"Could not find task {id}.";
                    }
                }
                return "Specify the task number. Example: 'complete task 1'";
            }
            //deleting a task
            if (lower.Contains("delete task") || lower.Contains("remove task"))
            {
                foreach (var word in lower.Split(' '))
                {
                    if (int.TryParse(word, out int id))
                    {
                        bool deleted = _db.DeleteTask(id);
                        if (deleted)
                            _activityLog.Add($"[{DateTime.Now:HH:mm}] Task {id} deleted");
                        return deleted
                            ? $"Task {id} deleted!"
                            : $"Could not find task {id}.";
                    }
                }
                return "Specify the task number. Example: 'delete task 1'";
            }

           
            // Normalise input to handle varied phrasing
            lower = NormaliseInput(lower);



            // ── QUIZ COMMANDS ──
            if (_quiz.IsActive)
            {
                string answer = lower.Trim();
                if (answer == "a" || answer == "b" || answer == "c" || answer == "d")
                {
                    string result = _quiz.AnswerQuestion(answer);
                    if (!_quiz.IsActive)
                        _activityLog.Add($"[{DateTime.Now:HH:mm}] Quiz completed — Score: {_quiz.Score}/{_quiz.Total}");
                    return result;
                }
                else
                {
                    return "Please answer with A, B, C or D.\n\n" + _quiz.GetCurrentQuestion();
                }
            }

            if (lower.Contains("start quiz") || lower.Contains("quiz")
                || lower.Contains("play quiz") || lower.Contains("test me"))
            {
                _activityLog.Add($"[{DateTime.Now:HH:mm}] Quiz started");
                return _quiz.StartQuiz();
            }
            // ACTIVITY LOG
            if (lower.Contains("show activity log") || lower.Contains("activity log")
                || lower.Contains("what have you done") || lower.Contains("show log")
                || lower.Contains("recent actions") || lower.Contains("what have you done for me"))
            {
                if (_activityLog.Count == 0)
                    return "No activities recorded yet. Start chatting, add tasks or take the quiz!";

                // Show last 10 actions
                int start = Math.Max(0, _activityLog.Count - 10);
                string log = "Recent Activity Log:\n\n";
                for (int i = start; i < _activityLog.Count; i++)
                    log += $"{i - start + 1}. {_activityLog[i]}\n";

                return log;
            }


            //Memory user states interest
            if (lower.Contains("interested in") || lower.Contains("i like")
                || lower.Contains("i love") || lower.Contains("i want to learn about"))
            {
                foreach (var topic in _responseHandler.RandomResponses.Keys
                         .Concat(_responseHandler.KeywordResponses.Keys))
                {
                    if (lower.Contains(topic))
                    {
                        _memory.Remember("interest", topic);
                        _lastTopic = topic;
                        _activityLog.Add($"[{DateTime.Now:HH:mm}] User interest saved: {topic}");
                        string name = _memory.Recall("name");
                        string response = _responseHandler.GetResponse(topic);
                        return $"Great, {name}! I'll remember that you're interested in {topic}.\n\n" +
                               $"It's a crucial part of staying safe online.\n\n{response}";
                    }
                }
            }

            //Detect sentiment first
            string sentimentPrefix = _sentiment.Detect(lower);

            //Find topic response
            string topicResponse = FindTopicResponse(lower);
            if (!string.IsNullOrEmpty(topicResponse))
                return sentimentPrefix + topicResponse;

            //Name recall
            if (lower.Contains("my name") || lower.Contains("who am i")
                || lower.Contains("remember me"))
            {
                if (_memory.Has("name"))
                    return $"Of course! Your name is {_memory.Recall("name")}. ";
                return "I don't think you've told me your name yet. What is it?";
            }

            //Interest recall
            if (lower.Contains("what do i like") || lower.Contains("my interest") || lower.Contains("remember"))
            {
                if (_memory.Has("interest"))
                    return $"You mentioned you're interested in {_memory.Recall("interest")}.\n" +
                           "Would you like more tips on that?";
                return "You haven't told me your interest yet. What topic would you like to focus on?";
            }

            //Greetings
            if (lower.Contains("hello") || lower.Contains("hi")
                || lower.Contains("hey") || lower.Contains("howzit"))
            {
                string name = _memory.Has("name") ? $", {_memory.Recall("name")}" : "";
                return $"Hello{name}!  What cybersecurity topic can I help you with today?";
            }

            //Help menu
            if (lower.Contains("help") 
                || lower.Contains("ask") || lower.Contains("menu"))
            {
                return BuildHelpMenu();
            }

            // ── Task: Add task ──
            if (lower.Contains("add task") || lower.Contains("new task") || lower.Contains("create task"))
            {
                string title = input.Length > 9 ? input.Substring(9).Trim() : "Cybersecurity Task";
                bool added = _db.AddTask(title, "Added via chatbot", "");
                _lastTopic = "task";
                return added
                    ? $"Task added successfully: '{title}'\n\nWould you like to set a reminder for this task? If yes, type:\n'remind me in X days'"
                    : "Could not add task. Please check your database connection.";
            }

            // ── Task: Set reminder ──
            if (lower.Contains("remind me in") || lower.Contains("reminder"))
            {
                string reminderText = input;
                var tasks = _db.GetAllTasks();
                if (tasks.Count > 0)
                {
                    var latest = tasks[0];
                    string date = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd");
                    // Extract days if mentioned
                    foreach (var word in lower.Split(' '))
                    {
                        if (int.TryParse(word, out int days))
                        {
                            date = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                            break;
                        }
                    }
                    _db.AddTask(latest.Title, latest.Description, date);
                    return $"Reminder set for '{latest.Title}' on {date}.";
                }
                return "You don't have any tasks yet. Add one first by typing 'add task [task name]'";
            }

            // ── Task: View tasks ──
            if (lower.Contains("show tasks") || lower.Contains("my tasks")
                || lower.Contains("view tasks") || lower.Contains("list tasks"))
            {
                var tasks = _db.GetAllTasks();
                if (tasks.Count == 0)
                    return "You have no tasks yet. Type 'add task [name]' to add one!";

                string result = "Your Tasks:\n\n";
                foreach (var t in tasks)
                {
                    string status = t.IsCompleted ? "" : "";
                    string reminder = string.IsNullOrEmpty(t.ReminderDate) ? "" : $" | {t.ReminderDate}";
                    result += $"{status} [{t.Id}] {t.Title}{reminder}\n";
                }
                return result;
            }

            // ── Task: Complete task ──
            if (lower.Contains("complete task") || lower.Contains("done task")
                || lower.Contains("finish task") || lower.Contains("mark task"))
            {
                foreach (var word in lower.Split(' '))
                {
                    if (int.TryParse(word, out int id))
                    {
                        bool done = _db.CompleteTask(id);
                        return done
                            ? $"Task {id} marked as completed! Great job staying cybersecurity aware!"
                            : $"Could not find task {id}.";
                    }
                }
                return "Please specify the task number. Example: 'complete task 1'";
            }

            // ── Task: Delete task ──
            if (lower.Contains("delete task") || lower.Contains("remove task"))
            {
                foreach (var word in lower.Split(' '))
                {
                    if (int.TryParse(word, out int id))
                    {
                        bool deleted = _db.DeleteTask(id);
                        return deleted
                            ? $"Task {id} deleted successfully."
                            : $"Could not find task {id}.";
                    }
                }
                return "Please specify the task number. Example: 'delete task 1'";
            }

            //Error handling
            _lastTopic = "";
            string unknown = _memory.Has("name")
                ? $"I'm not sure I understand that, {_memory.Recall("name")}. "
                : "I'm not sure I understand. ";
            return unknown + "Can you try rephrasing?";
        }

        //Find matching topic
        private string FindTopicResponse(string lower)
        {
            // Check random response topics first
            foreach (var kvp in _responseHandler.RandomResponses)
            {
                if (lower.Contains(kvp.Key))
                {
                    _lastTopic = kvp.Key;
                    string response = _responseHandler.PickRandom(kvp.Value);

                    // Personalise if interest matches
                    if (_memory.Has("interest") && _memory.Recall("interest") == kvp.Key)
                    {
                        string name = _memory.Recall("name");
                        response += $"\n\nAs someone interested in {kvp.Key}, " +
                                    $"{name}, this is especially relevant to you!";
                    }
                    return response;
                }
            }

            // Check single keyword responses
            foreach (var kvp in _responseHandler.KeywordResponses)
            {
                if (lower.Contains(kvp.Key))
                {
                    _lastTopic = kvp.Key;
                    return kvp.Value;
                }
            }

            return "";
        }

        //Extract name from input 
        private static string ExtractName(string input)
        {
            string[] words = input.Trim().Split(' ');
            string lower = input.ToLower();

            if (words.Length >= 3 && (lower.Contains("name is")
                || lower.Contains("i am") || lower.Contains("i'm")))
                return CapFirst(words[words.Length - 1]);

            return CapFirst(words[0]);
        }

        private static string CapFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

        //Help menu
        private string BuildHelpMenu()
        {
            string name = _memory.Has("name") ? $", {_memory.Recall("name")}" : "";
            return $"Here are the topics you can ask.{name}:\n\n" +
                   " Passwords\n" +
                   " Phishing\n" +
                   " Scams\n" +
                   " Privacy\n" +
                   "Just type any topic or ask a question!";

        }

         

        //Reset bot
        public void Reset()
        {
            _memory.ForgetAll();
            _lastTopic = "";
            _awaitingName = true;
        }

        // ── NLP: Normalise varied user input ──
        private string NormaliseInput(string input)
        {
            // Password variations
            if (input.Contains("passwd") || input.Contains("pass word")
                || input.Contains("my password") || input.Contains("change password")
                || input.Contains("update password") || input.Contains("reset password"))
                return input.Replace("passwd", "password")
                            .Replace("pass word", "password")
                            .Replace("change password", "password")
                            .Replace("update password", "password")
                            .Replace("reset password", "password");

            // Phishing variations
            if (input.Contains("fake email") || input.Contains("suspicious email")
                || input.Contains("spam email") || input.Contains("dodgy email")
                || input.Contains("weird email"))
                return input + " phishing";

            // Scam variations
            if (input.Contains("got scammed") || input.Contains("being scammed")
                || input.Contains("i was scammed") || input.Contains("fraud")
                || input.Contains("con") || input.Contains("trick"))
                return input + " scam";

            // Hacked variations
            if (input.Contains("got hacked") || input.Contains("been hacked")
                || input.Contains("i was hacked") || input.Contains("account hacked")
                || input.Contains("someone got into my"))
                return input + " data breach";

            // MFA variations
            if (input.Contains("two factor") || input.Contains("2fa")
                || input.Contains("verification code") || input.Contains("otp")
                || input.Contains("one time pin") || input.Contains("double login"))
                return input + " mfa";

            // VPN variations
            if (input.Contains("hide my ip") || input.Contains("anonymous browsing")
                || input.Contains("private browsing") || input.Contains("hide location"))
                return input + " vpn";

            // Malware variations
            if (input.Contains("virus") || input.Contains("trojan")
                || input.Contains("spyware") || input.Contains("adware")
                || input.Contains("infected") || input.Contains("my pc is slow"))
                return input + " malware";

            // Backup variations
            if (input.Contains("lost my files") || input.Contains("deleted files")
                || input.Contains("recover files") || input.Contains("restore files"))
                return input + " backup";

            // Privacy variations
            if (input.Contains("being watched") || input.Contains("someone watching")
                || input.Contains("data collected") || input.Contains("tracking me")
                || input.Contains("my data"))
                return input + " privacy";

            // Safe browsing variations
            if (input.Contains("safe to click") || input.Contains("is this link safe")
                || input.Contains("safe website") || input.Contains("secure site"))
                return input + " https";

            // Quiz variations
            if (input.Contains("test my knowledge") || input.Contains("quiz me")
                || input.Contains("test me") || input.Contains("challenge me")
                || input.Contains("play a game"))
                return input + " start quiz";

            // Task variations
            if (input.Contains("add a task") || input.Contains("create a task")
                || input.Contains("new task for me") || input.Contains("remind me to"))
                return input.Replace("add a task", "add task")
                            .Replace("create a task", "add task")
                            .Replace("remind me to", "add task");

            return input;
        }


    }
}






    


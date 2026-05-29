using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityBotWPF
{
    class ChatBot
    {

        private readonly ResponseHandler _responseHandler = new ResponseHandler();
        private readonly MemoryService _memory = new MemoryService();
        private readonly SentimentDetector _sentiment = new SentimentDetector();

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
    }
}






    


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityBotWPF
{
    class SentimentDetector
    {
        // Maps sentiment keywords to empathetic prefixes
        private readonly Dictionary<string, string> _sentiments = new Dictionary<string, string>()
        {
            ["worried"] = "It's completely understandable to feel worried,cyber threats are real, but you can protect yourself. ",
            ["scared"] = "There is no need to panic. With the right precautions, you can stay safe online. ",
            ["anxious"] = "Take a breath,awareness itself is a powerful defence. ",
            ["frustrated"] = "I hear you,cybersecurity can feel overwhelming. Let's break it down simply. ",
            ["confused"] = "No problem at all,let me explain this as clearly as possible. ",
            ["curious"] = "Great curiosity! Learning about cybersecurity is the first step to staying safe. ",
            ["excited"] = "Love the enthusiasm! Let's channel that into good cyber habits. ",
            ["angry"] = "I understand the frustration. Let's focus on what you can do right now to protect yourself. ",
            ["helpless"] = "You are not helpless,there are concrete steps you can take today. ",
            ["unsafe"] = "Feeling unsafe online is valid. Here are some immediate steps to improve your security. ",
            ["happy"] = "Great to hear you're feeling positive! Let's keep that energy going with some good cyber habits. ",
            ["bored"] = "Let's make cybersecurity interesting! Did you know hackers can crack a simple password in seconds? "
        };

        // It returns empathetic prefix if sentiment found, empty string if not
        public string Detect(string input)
        {
            string lower = input.ToLower();
            foreach (var kvp in _sentiments)
            {
                if (lower.Contains(kvp.Key))
                    return kvp.Value;
            }
            return "";
        }


    }
}

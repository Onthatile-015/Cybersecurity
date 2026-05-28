using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityBotWPF
{
    class MemoryService
    {
        // Stores user details e.g. name, interest
        private readonly Dictionary<string, string> _memory = new Dictionary<string, string>();

        // Save a value
        public void Remember(string key, string value)
        {
            _memory[key] = value;
        }

        // Get a value
        public string Recall(string key)
        {
            return _memory.ContainsKey(key) ? _memory[key] : "";
        }

        // Check if key exists
        public bool Has(string key)
        {
            return _memory.ContainsKey(key);
        }

        // Clear everything
        public void ForgetAll()
        {
            _memory.Clear();
        }


    }
}

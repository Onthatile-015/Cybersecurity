using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CybersecurityBotWPF
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public string CreatedAt { get; set; }
    }



    internal class DatabaseService
    {
        //Connection details for MySQL database
        private readonly string _connectionString =
            "Server=localhost;Database=cybersecuritybot;Uid=root;Pwd=Onthatile@60;";

        // ── Test connection ──
        public bool TestConnection()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                return true;
            }
            catch { return false; }
        }

        // ── Add a task ──
        public bool AddTask(string title, string description, string reminderDate)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                string sql = "INSERT INTO tasks (title, description, reminder_date) VALUES (@title, @desc, @reminder)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@reminder", reminderDate);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        // ── Get all tasks ──
        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                string sql = "SELECT * FROM tasks ORDER BY created_at DESC";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new TaskItem
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                        ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date")) ? "" : reader.GetString("reminder_date"),
                        IsCompleted = reader.GetBoolean("is_completed"),
                        CreatedAt = reader.GetDateTime("created_at").ToString("yyyy-MM-dd HH:mm")
                    });
                }
            }
            catch { }
            return tasks;
        }

        // ── Mark task as complete ──
        public bool CompleteTask(int id)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                string sql = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        // ── Delete a task ──
        public bool DeleteTask(int id)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                string sql = "DELETE FROM tasks WHERE id = @id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }


    }
}

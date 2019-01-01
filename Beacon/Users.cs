using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Beacon
{
    enum Authorization { Guest, Member, Trusted, Administrator };

    

    internal abstract class User
    {

        internal string Username;
        protected Authorization Rank;
        
    }

    internal class Guest : User, ViewHistory, SendMessage
    {
        string connectionString = "Server=THISEAS-PC\\SQLExpress;Database=Beacon;Integrated Security=true;";
        

        public Guest(string Name, Authorization Authority)
        {
            Username = Name;         
            Rank = Authority;
        }

        public void Send(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);


            Console.WriteLine("Please type your message. (Limit = 250 characters)");
            string textMessage = Console.ReadLine();
            DateTime dateTime = DateTime.Now;
            string QueryMessage = "INSERT INTO Messages (Sender, Receiver, Submission, Message) VALUES (@Sender, @Receiver, @Submission, @Message);";

            using (dbcon)
            {
                var SendMessage = dbcon.Query(QueryMessage, new { Sender = Username, Receiver = User2, Submission = dateTime, Message = textMessage });
            }

            View(User2);
        }

        public void View(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);

            var Messages = new List<Message>();
            string HistoryQuery = "SELECT * FROM Messages WHERE((Sender = @Sender AND Receiver = @Receiver) OR(Sender = @Receiver AND Receiver = @Sender)) ORDER BY Submission;";



            using (dbcon)
            {
                Messages.AddRange(dbcon.Query<Message>(HistoryQuery, new { Sender = Username, Receiver = User2 }));
            }

            foreach (var m in Messages)
            {
                Console.WriteLine($"From:{m.Sender}");
                Console.WriteLine($"To:{m.Receiver}, at: {m.dateTime}");
                Console.WriteLine($"{m.Text}");
                Console.WriteLine("========================================");

            }

        }
    }
    //TODO add Interfaces
    internal class Member : Guest, EditMessage
    {
        public Member(string Name,  Authorization Authority) : base(Name,  Authorization.Member)
        {

        }

        public void Edit()
        {
            throw new NotImplementedException();
        }
    }

    internal class Trusted : Member, DeleteMessage
    {
        public Trusted(string Name,  Authorization Authority) : base(Name,  Authorization.Trusted)
        {

        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }

    internal class Admin : Trusted, GiveAuthority, UserManipulation
    {
        public Admin(string Name, Authorization Authority) : base(Name, Authorization.Administrator)
        {

        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        public void Demote()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Promote()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }

    internal class Message
    {
        
        internal string Sender { get; set; }
        internal string Receiver { get; set; }
        internal DateTime dateTime { get; set; }
        internal string Text { get; set; }

        internal Message()
        {
        }

    }
}

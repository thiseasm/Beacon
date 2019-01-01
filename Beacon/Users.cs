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
        public Guest(string Name, Authorization Authority)
        {
            Username = Name;         
            Rank = Authority;
        }

        public void Send(string Recipient)
        {
            string connectionString = "Server=THISEAS-PC\\SQLExpress;Database=Beacon;Integrated Security=true;";
            SqlConnection dbcon = new SqlConnection(connectionString);

            Console.WriteLine("Please type your message. (Limit = 250 characters)");
            string textMessage = Console.ReadLine();
            DateTime dateTime = DateTime.Now;
            string QueryMessage = "INSERT INTO Messages (Sender, Receiver, Submission, Message) VALUES (@Sender, @Receiver, @Submission, @Message);";

            using (dbcon)
            {
                var SendMessage = dbcon.Query(QueryMessage, new { Sender = Username, Receiver = Recipient, Submission = dateTime, Message = textMessage });
            }

            View();
        }

        public void View()
        {
            throw new NotImplementedException();
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
}

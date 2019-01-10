﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Beacon
{
    enum Authorization { Guest, Member, Trusted, Administrator };

    

    internal class User
    {

        internal string Username;
        internal Authorization Rank;        
    }

    internal class Guest : User, ViewHistory, SendMessage
    {
        string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";
        

        public Guest(string Name, Authorization Authority)
        {
            Username = Name;         
            Rank = Authority;
        }

        public void ChangePassword()
        {
            string Pass1;
            string Pass2;

            do
            {

                //TODO ADD PASSWORD MASKING
                Console.WriteLine("Please pick a password:");
                Pass1 = Console.ReadLine();

                Console.WriteLine("Please enter your password a second time:");
                Pass2 = Console.ReadLine();

                if (Pass1 != Pass2)
                {
                    Console.WriteLine("The passwords do not match!");
                }

            } while (Pass1 != Pass2);

            using (SqlConnection dbcon = new SqlConnection(connectionString))
            {
                dbcon.Open();
                var PassUpdate = dbcon.Query("UPDATE Credentials SET Password = @pass WHERE Username = @name;", new { pass = Pass1, name = Username });
            }
            Console.WriteLine("Your password has been successfully changed!");
        }

        public void Send(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);


            Console.WriteLine("Please type your message. (Limit = 250 characters)");
            string textMessage = Console.ReadLine();            
            string QueryMessage = "INSERT INTO Messages (Sender, Receiver, Submission, Message) VALUES (@Sender, @Receiver, @Submission, @Message);";

            using (dbcon)
            {
                var SendMessage = dbcon.Query(QueryMessage, new { Sender = Username, Receiver = User2, Submission = DateTime.UtcNow, Message = textMessage });
            }
            Console.Clear();
            View(User2);
        }

        public void View(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);

            var Messages = new List<Data>();
            string HistoryQuery = "SELECT * FROM Messages WHERE((Sender = @Sender AND Receiver = @Receiver) OR (Sender = @Receiver AND Receiver = @Sender)) ORDER BY Submission;";



            using (dbcon)
            {
                dbcon.Open();

                Messages.AddRange(dbcon.Query<Data>(HistoryQuery, new { Sender = Username, Receiver = User2 }));
            }

            foreach (var m in Messages)
            {
                Console.WriteLine($"From:{m.Sender}");
                Console.WriteLine($"To:{m.Receiver}, at: {m.Submission}");
                Console.WriteLine($"{m.Message}");
                Console.WriteLine($"MessageID:{m.Stamp}");
                Console.WriteLine("========================================");

            }

        }
    }

    internal class Member : Guest, EditMessage
    {

        string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";

        public Member(string Name,  Authorization Authority) : base(Name, Authority)
        {

        }


        public void Edit(int Stamp, string Text, string User2)
        {
            string Sender = Username;
            string Receiver = User2;
            int StampSelected = Stamp;
            string text = Text;

            SqlConnection dbcon = new SqlConnection(connectionString);
            string Query = "UPDATE Messages SET Message = @Message WHERE (Sender = @sender AND Receiver = @receiver  AND Stamp = @stamp);";
            
            using (dbcon)
            {
                dbcon.Open();
                var AlterMessage = dbcon.Query(Query, new { Message = text, sender = Sender, receiver = Receiver,  stamp=StampSelected });
            }
            Console.Clear();
            Console.WriteLine("Message has been altered!");
            View(User2);
        }
    }

    internal class Trusted : Member, DeleteMessage
    {
        string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";

        public Trusted(string Name,  Authorization Authority) : base(Name, Authority)
        {

        }

        public void Delete(int Stamp, string User2)
        {
            string Sender = Username;
            string Receiver = User2;
            int StampSelected = Stamp;

            SqlConnection dbcon = new SqlConnection(connectionString);
            string Query = "DELETE FROM Messages WHERE (Sender = @sender AND Receiver = @receiver  AND Stamp = @stamp);";

            using (dbcon)
            {
                dbcon.Open();
                var DeleteMessage = dbcon.Query(Query, new { sender = Sender, receiver = Receiver, stamp = StampSelected });
            }

            Console.WriteLine("Message has been deleted!");
            View(User2);
        }
    }

    internal class Admin : Trusted, GiveAuthority, UserManipulation
    {
        string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";

        public Admin(string Name, Authorization Authority) : base(Name, Authority)
        {

        }

        public void Create()
        {
            bool NameOriginal = false;
            string Name = "";

            while (NameOriginal == false)
            {
                Console.WriteLine("Please pick a username:");
                Name = Console.ReadLine();

                NameOriginal = Namecheck(Name);

                if (NameOriginal == false)
                {
                    Console.WriteLine("This Username is already taken!");
                    Console.WriteLine("Please choose something else.");
                }

            }

            string Pass1 = "0000";            

            Registration(Name, Pass1);
            Console.Clear();
            ListUsers();
        }

        public void Demote(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string Rank;

            using (dbcon)
            {
                dbcon.Open();

                Rank = dbcon.Query<string>("SELECT Rank FROM Accounts WHERE Username = @name", new { name = User2 }).Single();
            }

            string NewRank = "";

            if (Rank == "Administrator")
            {
                NewRank = NewRank + "Trusted";
            }
            else if (Rank == "Trusted")
            {
                NewRank = NewRank + "Member";
            }
            else if (Rank == "Member")
            {
                NewRank = NewRank + "Guest";
            }
            else
            {
                Console.WriteLine($"{User2} cannot be demoted further!");
                return;
            }

            SqlConnection dbcon1 = new SqlConnection(connectionString);
            using (dbcon1)
            {
                dbcon1.Open();

                var Demotion = dbcon1.Query("UPDATE Accounts SET Rank = @newrank WHERE Username = @name", new { newrank = NewRank, name = User2 });
            }

            Console.WriteLine($"{User2} has been demoted to {NewRank}!");
            Console.Clear();
            ListUsers();
        }

        public void Destroy(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string UsernameQuery = "DELETE FROM Accounts WHERE Username = @user;";
            string CredentialsQuery = "DELETE FROM Credentials WHERE Username = @user;";
            string MessageQuery = "DELETE FROM Messages WHERE (Sender = @user OR Receiver = @user);";

            using (dbcon)
            {
                dbcon.Open();                
                var DeletePass = dbcon.Query(CredentialsQuery, new { user = User2 });
                var DeleteHistory = dbcon.Query(MessageQuery, new { user = User2 });
                var DeleteUser = dbcon.Query(UsernameQuery, new { user = User2 });
            }

            Console.WriteLine($"{User2} has been deleted!");
            ListUsers();
        }

        public void Promote(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string Rank;

            using (dbcon)
            {
                dbcon.Open();

                Rank = dbcon.Query<string>("SELECT Rank FROM Accounts WHERE Username = @name", new { name = User2 }).Single();
                
            }

            string NewRank = "";

            if (Rank == "Guest")
            {
                NewRank = NewRank + "Member";
            }
            else if (Rank == "Member")
            {
                NewRank = NewRank + "Trusted";
            }
            else if (Rank == "Trusted")
            {
                NewRank = NewRank + "Administrator";
            }
            else
            {
                Console.WriteLine($"{User2} cannot be promoted further!");
                return;
            }

            SqlConnection dbcon1 = new SqlConnection(connectionString);
            using (dbcon1)
            {
                dbcon1.Open();

                var Promotion = dbcon1.Query("UPDATE Accounts SET Rank = @newrank WHERE Username = @name", new { newrank = NewRank, name = User2 });
            }
            Console.Clear();
            Console.WriteLine($"{User2} has been promoted to {NewRank}!");
            ListUsers();
        }

        public void Update(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string AccountQuery = "UPDATE Accounts SET Username = @user WHERE Username = @user2;";

            Console.WriteLine($"Pick a new username for {User2}:");
            string usernameNEW = Console.ReadLine();
            using (dbcon)
            {
                dbcon.Open();
                var UpdateUser = dbcon.Query(AccountQuery, new { user=usernameNEW, user2 = User2 });
            }
            Console.Clear();
            Console.WriteLine($"{User2} has been changed to {usernameNEW}");
            ListUsers();
        }

        public void ListUsers()
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string List = "SELECT * FROM Accounts;";
            var Users = new List<User>();

            using (dbcon)
            {
                dbcon.Open();
                Users.AddRange(dbcon.Query<User>(List));
            }

            foreach (var user in Users)
            {
                Console.WriteLine(user.Username + " - " + user.Rank);
            }
        }

        static bool Namecheck(string Name)
        {
            string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";
            SqlConnection dbcon = new SqlConnection(connectionString);
            bool NameOriginal;

            using (dbcon)
            {
                dbcon.Open();
                var UsernameCheck = dbcon.Query("SELECT * FROM Accounts WHERE Username = @Username;", new { Username = Name }).Count();

                if (UsernameCheck == 1)
                {
                    NameOriginal = false;
                }
                else
                {
                    NameOriginal = true;
                }

                return NameOriginal;
            }
        }

        static void Registration(string Name, string Pass1)
        {
            string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";
            SqlConnection dbcon = new SqlConnection(connectionString);
            using (dbcon)
            {
                dbcon.Open();

                string RegistrationQuery = "INSERT INTO Accounts (Username,Rank) VALUES (@name, @guest);";
                var AccountInsertion = dbcon.Query(RegistrationQuery, new { name = Name, guest = "Guest" });

                
                string PassQuery = "INSERT INTO Credentials (Username,Password) VALUES (@name, @pass);";
                var PassInsertion = dbcon.Query(PassQuery, new { name = Name, pass = Pass1 });
            }

            Console.WriteLine($"The user {Name} has been created.");
            Console.WriteLine($"Please use the password:{Pass1} to login for the first time!");
        }

        
    }

    public class Data
    {
        
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime Submission { get; set; }
        public string Message { get; set; }
        public int Stamp { get; set; }
        
    }


}

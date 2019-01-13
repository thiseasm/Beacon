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

    

    internal class User
    {

        internal string Username;
        internal Authorization Rank;        
    }

    internal class Guest : User, IViewHistory, ISendMessage
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
                var passUpdate = dbcon.Query("UPDATE Credentials SET Password = @pass WHERE Username = @name;", new { pass = Pass1, name = Username });
            }
            Console.WriteLine("Your password has been successfully changed!");
        }

        public void Send(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);


            Console.WriteLine("Please type your message. (Limit = 250 characters)");
            string textMessage = Console.ReadLine();            
            string queryMessage = "INSERT INTO Messages (Sender, Receiver, Submission, Message) VALUES (@Sender, @Receiver, @Submission, @Message);";

            using (dbcon)
            {
                var sendMessage = dbcon.Query(queryMessage, new { Sender = Username, Receiver = User2, Submission = DateTime.UtcNow, Message = textMessage });
            }
            Console.Clear();
            View(User2);
        }

        public void View(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);

            var messages = new List<Data>();
            string historyQuery = "SELECT * FROM Messages WHERE((Sender = @Sender AND Receiver = @Receiver) OR (Sender = @Receiver AND Receiver = @Sender)) ORDER BY Submission;";



            using (dbcon)
            {
                dbcon.Open();

                messages.AddRange(dbcon.Query<Data>(historyQuery, new { Sender = Username, Receiver = User2 }));
            }

            foreach (var m in messages)
            {
                Console.WriteLine($"From:{m.Sender}");
                Console.WriteLine($"To:{m.Receiver}, at: {m.Submission}");
                Console.WriteLine($"{m.Message}");
                Console.WriteLine($"MessageID:{m.Stamp}");
                Console.WriteLine("========================================");

            }

        }
    }

    internal class Member : Guest, IEditMessage
    {

        string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";

        public Member(string Name,  Authorization Authority) : base(Name, Authority)
        {

        }


        public void Edit(int Stamp, string Text, string User2)
        {
            string sender = Username;
            string receiver = User2;
            int stampSelected = Stamp;
            string text = Text;

            SqlConnection dbcon = new SqlConnection(connectionString);
            string Query = "UPDATE Messages SET Message = @Message WHERE (Sender = @sender AND Receiver = @receiver  AND Stamp = @stamp);";
            
            using (dbcon)
            {
                dbcon.Open();
                var alterMessage = dbcon.Query(Query, new { Message = text, sender = sender, receiver = receiver,  stamp=stampSelected });
            }
            Console.Clear();
            Console.WriteLine("Message has been altered!");
            View(User2);
        }
    }

    internal class Trusted : Member, IDeleteMessage
    {
        string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";

        public Trusted(string Name,  Authorization Authority) : base(Name, Authority)
        {

        }

        public void Delete(int Stamp, string User2)
        {
            string sender = Username;
            string receiver = User2;
            int stampSelected = Stamp;

            SqlConnection dbcon = new SqlConnection(connectionString);
            string Query = "DELETE FROM Messages WHERE (Sender = @sender AND Receiver = @receiver  AND Stamp = @stamp);";

            using (dbcon)
            {
                dbcon.Open();
                var deleteMessage = dbcon.Query(Query, new { sender = sender, receiver = receiver, stamp = stampSelected });
            }

            Console.WriteLine("Message has been deleted!");
            View(User2);
        }
    }

    internal class Admin : Trusted, IGiveAuthority, IUserManipulation
    {
        string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";

        public Admin(string Name, Authorization Authority) : base(Name, Authority)
        {

        }

        public void Create()
        {
            bool nameOriginal = false;
            string name = "";

            while (nameOriginal == false)
            {
                Console.WriteLine("Please pick a username:");
                name = Console.ReadLine();

                nameOriginal = Namecheck(name);

                if (nameOriginal == false)
                {
                    Console.WriteLine("This Username is already taken!");
                    Console.WriteLine("Please choose something else.");
                }

            }

            string oass1 = "0000";            

            Registration(name, oass1);
            Console.Clear();
            ListUsers();
        }

        public void Demote(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string rank;

            using (dbcon)
            {
                dbcon.Open();

                rank = dbcon.Query<string>("SELECT Rank FROM Accounts WHERE Username = @name", new { name = User2 }).Single();
            }

            string newRank = "";

            if (rank == "Administrator")
            {
                newRank = newRank + "Trusted";
            }
            else if (rank == "Trusted")
            {
                newRank = newRank + "Member";
            }
            else if (rank == "Member")
            {
                newRank = newRank + "Guest";
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

                var Demotion = dbcon1.Query("UPDATE Accounts SET Rank = @newrank WHERE Username = @name", new { newrank = newRank, name = User2 });
            }

            Console.WriteLine($"{User2} has been demoted to {newRank}!");
            Console.Clear();
            ListUsers();
        }

        public void Destroy(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string usernameQuery = "DELETE FROM Accounts WHERE Username = @user;";
            string credentialsQuery = "DELETE FROM Credentials WHERE Username = @user;";
            string messageQuery = "DELETE FROM Messages WHERE (Sender = @user OR Receiver = @user);";

            using (dbcon)
            {
                dbcon.Open();                
                var deletePass = dbcon.Query(credentialsQuery, new { user = User2 });
                var deleteHistory = dbcon.Query(messageQuery, new { user = User2 });
                var deleteUser = dbcon.Query(usernameQuery, new { user = User2 });
            }

            Console.WriteLine($"{User2} has been deleted!");
            ListUsers();
        }

        public void Promote(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string rank;

            using (dbcon)
            {
                dbcon.Open();

                rank = dbcon.Query<string>("SELECT Rank FROM Accounts WHERE Username = @name", new { name = User2 }).Single();
                
            }

            string newRank = "";

            if (rank == "Guest")
            {
                newRank = newRank + "Member";
            }
            else if (rank == "Member")
            {
                newRank = newRank + "Trusted";
            }
            else if (rank == "Trusted")
            {
                newRank = newRank + "Administrator";
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

                var promotion = dbcon1.Query("UPDATE Accounts SET Rank = @newrank WHERE Username = @name", new { newrank = newRank, name = User2 });
            }
            Console.Clear();
            Console.WriteLine($"{User2} has been promoted to {newRank}!");
            ListUsers();
        }

        public void Update(string User2)
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string accountQuery = "UPDATE Accounts SET Username = @user WHERE Username = @user2;";

            Console.WriteLine($"Pick a new username for {User2}:");
            string usernameNEW = Console.ReadLine();
            using (dbcon)
            {
                dbcon.Open();
                var updateUser = dbcon.Query(accountQuery, new { user=usernameNEW, user2 = User2 });
            }
            Console.Clear();
            Console.WriteLine($"{User2} has been changed to {usernameNEW}");
            ListUsers();
        }

        public void ListUsers()
        {
            SqlConnection dbcon = new SqlConnection(connectionString);
            string list = "SELECT * FROM Accounts;";
            var users = new List<User>();

            using (dbcon)
            {
                dbcon.Open();
                users.AddRange(dbcon.Query<User>(list));
            }

            foreach (var user in users)
            {
                Console.WriteLine(user.Username + " - " + user.Rank);
            }
        }

        static bool Namecheck(string Name)
        {
            string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";
            SqlConnection dbcon = new SqlConnection(connectionString);
            bool nameOriginal;

            using (dbcon)
            {
                dbcon.Open();
                var usernameCheck = dbcon.Query("SELECT * FROM Accounts WHERE Username = @Username;", new { Username = Name }).Count();

                if (usernameCheck == 1)
                {
                    nameOriginal = false;
                }
                else
                {
                    nameOriginal = true;
                }

                return nameOriginal;
            }
        }

        static void Registration(string Name, string Pass1)
        {
            string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";
            SqlConnection dbcon = new SqlConnection(connectionString);
            using (dbcon)
            {
                dbcon.Open();

                string registrationQuery = "INSERT INTO Accounts (Username,Rank) VALUES (@name, @guest);";
                var accountInsertion = dbcon.Query(registrationQuery, new { name = Name, guest = "Guest" });

                
                string passQuery = "INSERT INTO Credentials (Username,Password) VALUES (@name, @pass);";
                var passInsertion = dbcon.Query(passQuery, new { name = Name, pass = Pass1 });
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

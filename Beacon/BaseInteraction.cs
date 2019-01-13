using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Beacon
{
    class BaseInteraction
    {
        static string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";
        static SqlConnection dbcon = new SqlConnection(connectionString);

        static internal void PassChange(string Pass1, string Username)
        {
            using (SqlConnection dbcon = new SqlConnection(connectionString))
            {
                dbcon.Open();
                var passUpdate = dbcon.Query("UPDATE Credentials SET Password = @pass WHERE Username = @name;", new { pass = Pass1, name = Username });
            }
            Console.WriteLine("Your password has been successfully changed!");
        }

        static internal void SendMessage(string Username, string User2, string textMessage)
        {
            string queryMessage = "INSERT INTO Messages (Sender, Receiver, Submission, Message) VALUES (@Sender, @Receiver, @Submission, @Message);";

            using (dbcon)
            {
                var sendMessage = dbcon.Query(queryMessage, new { Sender = Username, Receiver = User2, Submission = DateTime.UtcNow, Message = textMessage });
            }
        }

        static internal List<Data> GetMessages(string Username, string User2)
        {
            var messages = new List<Data>();
            string historyQuery = "SELECT * FROM Messages WHERE((Sender = @Sender AND Receiver = @Receiver) OR (Sender = @Receiver AND Receiver = @Sender)) ORDER BY Submission;";

            using (dbcon)
            {
                dbcon.Open();

                messages.AddRange(dbcon.Query<Data>(historyQuery, new { Sender = Username, Receiver = User2 }));

            }
            return messages;       
        }

        static internal void EditMessage(string text, string sender, string receiver, int stampSelected)
        {
            string Query = "UPDATE Messages SET Message = @Message WHERE (Sender = @sender AND Receiver = @receiver  AND Stamp = @stamp);";

            using (dbcon)
            {
                dbcon.Open();
                var alterMessage = dbcon.Query(Query, new { Message = text, sender = sender, receiver = receiver, stamp = stampSelected });
            }
        }

        static internal void MessageDeletion(string sender, string receiver, int stampSelected)
        {
            string Query = "DELETE FROM Messages WHERE (Sender = @sender AND Receiver = @receiver  AND Stamp = @stamp);";

            using (dbcon)
            {
                dbcon.Open();
                var deleteMessage = dbcon.Query(Query, new { sender = sender, receiver = receiver, stamp = stampSelected });
            }
        }

        static internal string RankCheck(string User2)
        {
            string rank;
            using (dbcon)
            {
                dbcon.Open();

                rank = dbcon.Query<string>("SELECT Rank FROM Accounts WHERE Username = @name", new { name = User2 }).Single();
            }
            return rank;
        }

        static internal void Demotion(string newRank, string User2)
        {
            using (dbcon)
            {
                dbcon.Open();

                var Demotion = dbcon.Query("UPDATE Accounts SET Rank = @newrank WHERE Username = @name", new { newrank = newRank, name = User2 });
            }
        }

        static internal void Promotion(string newRank, string User2)
        {
            using (dbcon)
            {
                dbcon.Open();

                var promotion = dbcon.Query("UPDATE Accounts SET Rank = @newrank WHERE Username = @name", new { newrank = newRank, name = User2 });
            }
        }

        static internal void UpdateInfo(string usernameNEW, string User2)
        {
            string accountQuery = "UPDATE Accounts SET Username = @user WHERE Username = @user2;";

            using (dbcon)
            {
                dbcon.Open();
                var updateUser = dbcon.Query(accountQuery, new { user = usernameNEW, user2 = User2 });
            }
        }

        static internal List<User> GetList()
        {
            string list = "SELECT * FROM Accounts;";
            var users = new List<User>();

            using (dbcon)
            {
                dbcon.Open();
                users.AddRange(dbcon.Query<User>(list));
            }
            return users;
        }

        static internal void DeleteUser(string User2)
        {
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
        }

        static internal bool Namecheck(string Name)
        {
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

        static internal void Registration(string Name, string Pass1)
        {
            using (dbcon)
            {
                dbcon.Open();

                string registrationQuery = "INSERT INTO Accounts (Username,Rank) VALUES (@name, @guest);";
                var accountInsertion = dbcon.Query(registrationQuery, new { name = Name, guest = "Guest" });

                string passQuery = "INSERT INTO Credentials (Username,Password) VALUES (@name, @pass);";
                var passInsertion = dbcon.Query(passQuery, new { name = Name, pass = Pass1 });
            }
            Console.WriteLine($"The user {Name} has been created.");
            Console.WriteLine("Please reopen the application to login with your credentials.");
            Console.WriteLine("Press ANY key to terminate:");
            Console.ReadKey();
            Environment.Exit(0);
        }

        static internal bool CredentialCheck(string Username)
        {

            using (dbcon)
            {
                dbcon.Open();
                while (true)
                {
                    Console.WriteLine("Please input your Password:");
                    string password = Console.ReadLine();
                    var paperCheck = dbcon.Query("SELECT * FROM Credentials WHERE Username = @name AND Password = @pass;", new { name = Username, pass = password }).Count();

                    if (paperCheck == 1)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Password is incorrect!");
                        Console.WriteLine("Access Denied!");
                    }
                }

            }

        }
    }
}

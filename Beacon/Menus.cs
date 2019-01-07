using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


namespace Beacon
{
    internal class Menu
    {
        static string connectionString = "Server=THISEAS-PC\\SQLExpress;Database=Beacon;Integrated Security=true;";
        internal User LoginScreen()
        {

            Console.WriteLine("Welcome to the *Beacon Messenger System* est.2018!");
            Console.WriteLine("Please follow the instructions provided below.");
            Console.WriteLine("==============");
            Console.WriteLine("Choose action:");
            Console.WriteLine("L - Login");
            Console.WriteLine("R - Register");
            Console.WriteLine("X - Terminate");
            Console.WriteLine("==============");

            SqlConnection dbcon = new SqlConnection(connectionString);

            string Selection = Console.ReadLine();       
            switch (Selection.ToLower())
            {
                case "x":
                    Console.Clear();
                    Console.WriteLine("Thank you for choosing us for your communication needs.");
                    Console.WriteLine("The program will now terminate.");
                    Environment.Exit(0);
                    break;
                case "r":
                    Console.Clear();
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


                    string Pass1;
                    string Pass2;
                    
                    do
                    {

                        //TODO ADD PASSWORD MASKING
                        Console.WriteLine("Please pick a password:");
                        Pass1 = Console.ReadLine();

                        Console.WriteLine("Please enter your password a second time:");
                        Pass2 = Console.ReadLine();
                        Console.Clear();
                        if (Pass1 != Pass2)
                        {
                            Console.WriteLine("The passwords do not match!");
                        }

                    } while (Pass1 != Pass2);

                    Registration(Name, Pass1);
                    break;
               
                case "l":
                    Console.Clear();
                    bool UserInBase = true;
                    string Username = "";

                    while (UserInBase == true)
                    {
                        Console.WriteLine("Please input your Username:");
                        Username = Console.ReadLine();

                        UserInBase = Namecheck(Username);

                        if (UserInBase == true)
                        {
                            Console.WriteLine($"The user {Username} does not exist!");
                        }

                    }

                    CredentialCheck(Username);
                    using (dbcon)
                    {
                        dbcon.Open();

                        var AuthorityCheck =dbcon.Query<string>("SELECT Rank FROM Accounts WHERE Username = @name;", new { name = Username }).Single();
                        
                        if (AuthorityCheck == "Guest")
                        {
                            return new Guest(Username, Authorization.Guest);
                        }
                        else if (AuthorityCheck == "Member")
                        {
                            return new Member(Username, Authorization.Member);
                        }
                        else if (AuthorityCheck == "Trusted")
                        {
                            return new Trusted(Username, Authorization.Trusted);
                        }
                        else if (AuthorityCheck == "Administrator")
                        {
                            return new Admin(Username, Authorization.Administrator);
                        }
                    }
                    break;
            }
            return null;
        }

        internal void MenuScreen(Trusted User)
        {
            while (true)
            {
                

                Console.Clear();
                Console.WriteLine("*Beacon Messenger System* est.2018!");
                Console.WriteLine($"Welcome back {User.Username}");
                Console.WriteLine("Please follow the instructions provided below.");
                Console.WriteLine("==============");
                Console.WriteLine("Choose action:");
                Console.WriteLine("view - View messages");                
                Console.WriteLine("logout - Logout");
                Console.WriteLine("==============");
                string Selection = Console.ReadLine();

                switch (Selection.ToLower())
                {
                    case "view":
                        Console.Clear();
                        Console.WriteLine("Please enter a valid username:");
                        Console.WriteLine("==============OR==============");
                        Console.WriteLine("return - Return to Main Menu");
                        string User2 = Console.ReadLine();

                        if (User2.ToLower() == "return")
                        {
                            break;
                        }
                        bool UserInSystem = true;

                        while (UserInSystem == true)
                        {
                            UserInSystem = Namecheck(User2);
                            if (UserInSystem == true)
                            {
                                Console.Clear();
                                Console.WriteLine($"The user {User2} cannot be found!");
                                Console.WriteLine("Please enter a valid username:");
                                User2 = Console.ReadLine();
                            }
                        }
                        User.View(User2);
                        Console.WriteLine("Choose action:");
                        Console.WriteLine("send - Send message");
                        if (User.Rank == Authorization.Member)
                        {
                            Console.WriteLine("edit - Edit message");
                        }
                        else if (User.Rank == Authorization.Trusted)
                        {
                            Console.WriteLine("edit - Edit message");
                            Console.WriteLine("delete - Delete message");
                        }
                        Console.WriteLine("==============OR==============");
                        Console.WriteLine("return - Return to Previous Screen");
                        string ActionSelection = Console.ReadLine();

                        switch (ActionSelection.ToLower())
                        {
                            case "return":
                                Console.Clear();
                                break;
                            case "send":
                                User.Send(User2);
                                break;
                            case "edit":
                                if (User.Rank == Authorization.Guest)
                                {
                                    Console.WriteLine("Action Restricted!");
                                    break;
                                }
                                Console.WriteLine("Copy and Paste the date/time of the message you want to alter:");
                                DateTime dateTime = DateTime.Parse(Console.ReadLine());

                                Console.WriteLine("Please type your message. (Limit = 250 characters)");
                                string textMessage = Console.ReadLine();
                                User.Edit(dateTime, textMessage, User2);
                                break;
                            case "delete":
                                Console.WriteLine("Copy and Paste the date/time of the message you want to delete:");
                                DateTime dateTime2 = DateTime.Parse(Console.ReadLine());                                
                                User.Delete(dateTime2, User2);
                                break;
                        }




                }



            }



        }

        internal void AdminMenu(Admin User1)
        {
            //TODO Implement Menu admin
        }

        static bool Namecheck(string Name)
        {
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
            SqlConnection dbcon = new SqlConnection(connectionString);
            using (dbcon)
            {
                dbcon.Open();

                string RegistrationQuery = "INSERT INTO Accounts (Username,Rank) VALUES (@name, @guest);";
                var AccountInsertion = dbcon.Query(RegistrationQuery, new { name = Name, guest = "Guest" });

                //TODO ADD PASSWORD ENCRYPTION
                string PassQuery = "INSERT INTO Credentials (Username,Password) VALUES (@name, @pass);";
                var PassInsertion = dbcon.Query(PassQuery, new { name = Name, pass = Pass1 });
            }
            Console.WriteLine($"The user {Name} has been created.");
            Console.WriteLine("Please reopen the application to login with your credentials.");
            Console.WriteLine("Press ANY key to terminate:");
            Console.ReadKey();
            Environment.Exit(0);
        }

        static bool CredentialCheck(string Username)
        {
            //TODO ADD PASSWORD MASKING            

            SqlConnection dbcon = new SqlConnection(connectionString);
            using (dbcon)
            {
                dbcon.Open();
                while (true)
                {
                    Console.WriteLine("Please input your Password:");
                    string Password = Console.ReadLine();
                    var PaperCheck = dbcon.Query("SELECT * FROM Credentials WHERE Username = @name AND Password = @pass;", new { name = Username, pass = Password }).Count();

                    if (PaperCheck == 1)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace Beacon
{
    
    

    class Program
    {
        //TODO fix classes 


        static string connectionString = "Server=THISEAS-PC\\SQLExpress;Database=Beacon;Integrated Security=true;";



        static void Main(string[] args)
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

            User User1 = null;

            switch (Selection)
            {
                case "X":
                case "x":
                    Console.WriteLine("Thank you for choosing us for your communication needs.");
                    Console.WriteLine("The program will now terminate.");
                    Environment.Exit(0);
                    break;
                case "R":
                case "r":

                    bool NameOriginal = false;
                    string Name="";

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
                        
                        if (Pass1 != Pass2)
                        {
                            Console.WriteLine("The passwords do not match!");
                        }

                    } while (Pass1 != Pass2);

                    Registration(Name, Pass1);                 
                   
                    break;

                case "L":
                case "l":

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

                        string AuthorityCheck = dbcon.Query("SELECT Rank FROM Accounts WHERE Username = @name;", new { name = Username }).ToString();

                        if (AuthorityCheck == "Guest")
                        {
                            User1 = new Guest(Username, Authorization.Guest);
                        }
                        else if (AuthorityCheck == "Member")
                        {
                            User1 = new Member(Username, Authorization.Member);
                        }
                        else if (AuthorityCheck == "Trusted")
                        {
                            User1 = new Trusted(Username, Authorization.Trusted);
                        }
                        else if (AuthorityCheck == "Administrator")
                        {
                            User1 = new Admin(Username, Authorization.Administrator);
                        }

                        
                    }

                    Console.WriteLine("Login Successful!");
                    Console.WriteLine($"Welcome back {User1.Username}");                  
                    
                    break;

            }

            //TODO ADD MENU AFTER LOGIN
            
        }

        static bool Namecheck (string Name)
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

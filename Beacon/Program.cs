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
                        //TODO check for usernameduplication

                        NameOriginal = Namecheck(Name);

                        
                    } 
                    

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

                    Guest guest = new Guest(Name, Pass1, Authorization.Guest);

                    //TODO add BASE interaction

                    Console.WriteLine($"The user {Name} has been created.");
                    Console.WriteLine("Please reopen the application to login with your credentials.");                    
                    Console.WriteLine("Press ANY key to terminate:");
                    Console.ReadKey();
                    Environment.Exit(0);
                    break;








            }
            
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
                    Console.WriteLine("This Username is already taken!");
                    Console.WriteLine("Please choose something else.");
                    NameOriginal = false;
                    
                }
                else
                {
                    NameOriginal = true;
                }

                return NameOriginal;
            }
        }

    }
}

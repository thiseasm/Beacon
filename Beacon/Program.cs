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

        static void Main(string[] args)
        {

            LoginMenu NewLogin = new LoginMenu();
            User User1 = NewLogin.LoginScreen();
            Console.WriteLine("Login Successful!");
            Console.WriteLine($"Welcome back {User1.Username}");
            Console.WriteLine(User1.GetType());
            Console.ReadLine();

            //TODO ADD MENU AFTER LOGIN
            
        }

        
    }
}

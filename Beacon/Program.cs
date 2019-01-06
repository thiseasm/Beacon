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

            LoginMenu NewLogin = new LoginMenu();
            var User1 = NewLogin.LoginScreen();

            Console.WriteLine(User1.GetType());
            Console.ReadLine();

            //TODO ADD MENU AFTER LOGIN
            
        }

        
    }
}

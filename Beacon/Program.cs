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

            Menu NewMenu = new Menu();
            User User1 = NewMenu.LoginScreen();
            Console.WriteLine("Login Successful!");
            NewMenu.MenuScreen(User1);
            Console.ReadLine();

            //TODO ADD MENU AFTER LOGIN
            
        }

        
    }
}

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
            Console.Clear();
            Menu NewMenu = new Menu();
            User User = NewMenu.LoginScreen();
            Console.WriteLine("Login Successful!");

            if (User.Rank == Authorization.Administrator)
            {
                Admin User1 = new Admin(User.Username, User.Rank);
                NewMenu.AdminMenu(User1);
            }
            else if (User.Rank == Authorization.Trusted)
            {
                Trusted User1 = new Trusted(User.Username, User.Rank);
                NewMenu.MenuScreen(User1);
            }
            else if (User.Rank == Authorization.Member)
            {
                Member User1 = new Member(User.Username, User.Rank);
                NewMenu.MenuScreen(User1);
            }
            else
            {
                Guest User1 = new Guest(User.Username, User.Rank);
                NewMenu.MenuScreen(User1);
            }

                Console.ReadLine();

            //TODO ADD MENU AFTER LOGIN
            
        }

        
    }
}

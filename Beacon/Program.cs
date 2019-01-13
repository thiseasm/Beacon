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

        static void Main(string[] args)
        {
            while (true)
            {                
                Menu NewMenu = new Menu();
                User User = NewMenu.LoginScreen();
                Console.WriteLine("Login Successful!");

                if (User.Rank == Authorization.Administrator)
                {
                    Admin User1 = new Admin(User.Username, Authorization.Administrator);
                    NewMenu.AdminScreen(User1);
                }
                else if (User.Rank == Authorization.Trusted)
                {
                    Trusted User1 = new Trusted(User.Username, User.Rank);
                    NewMenu.TrustedScreen(User1);
                }
                else if (User.Rank == Authorization.Member)
                {
                    Member User1 = new Member(User.Username, User.Rank);
                    NewMenu.MemberScreen(User1);
                }
                else
                {
                    Guest User1 = new Guest(User.Username, User.Rank);
                    NewMenu.GuestScreen(User1);
                }
            }                       
        }       
    }
}

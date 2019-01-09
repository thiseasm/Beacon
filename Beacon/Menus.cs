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
        static string connectionString = "Server=LAPTOP-GFPB19JQ\\SQLExpress;Database=Beacon;Integrated Security=true;";
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
            string Selection = "";
            do
            {
                Selection = Console.ReadLine();
                Selection = Selection.ToLower();
            } while (Selection != "l" && Selection != "r" && Selection != "x");
                   
            switch (Selection)
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

        internal void GuestScreen(Guest User)
        {
            bool InUse = true;
            Console.Clear();
            Console.WriteLine("*Beacon Messenger System* est.2018!");
            Console.WriteLine($"Welcome back {User.Username}");
            while (InUse == true)
            {
                Console.WriteLine("Please follow the instructions provided below.");
                Console.WriteLine("====================");
                Console.WriteLine("Choose action:");
                Console.WriteLine("view - View messages");
                Console.WriteLine("change - Change password");
                Console.WriteLine("logout - Logout");
                Console.WriteLine("====================");
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
                            default:
                                Console.WriteLine("Please use one of the available actions.");
                                break;
                        }
                        break;
                    case "change":
                        Console.Clear();
                        User.ChangePassword();
                        break;
                    case "logout":
                        Console.Clear();
                        InUse = false;
                        Console.WriteLine($"Thank you {User.Username} for using *Beacon*!");
                        break;
                    default:
                        Console.WriteLine("Please use one of the available actions.");
                        break;
                }
            }
        }

        internal void MemberScreen(Member User)
        {
            bool InUse = true;
            Console.Clear();
            Console.WriteLine("*Beacon Messenger System* est.2018!");
            Console.WriteLine($"Welcome back {User.Username}");
            while (InUse == true)
            {
                Console.WriteLine("Please follow the instructions provided below.");
                Console.WriteLine("====================");
                Console.WriteLine("Choose action:");
                Console.WriteLine("view - View messages");
                Console.WriteLine("change - Change password");
                Console.WriteLine("logout - Logout");
                Console.WriteLine("====================");
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
                        Console.WriteLine("edit - Edit message");
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
                                Console.WriteLine("Copy and Paste the date/time of the message you want to alter:");
                                int Stamp = int.Parse(Console.ReadLine());
                                Console.WriteLine("Please type your message. (Limit = 250 characters)");
                                string textMessage = Console.ReadLine();
                                User.Edit(Stamp, textMessage, User2);
                                break;
                            default:
                                Console.WriteLine("Please use one of the available actions.");
                                break;
                        }
                        break;
                    case "change":
                        Console.Clear();
                        User.ChangePassword();
                        break;
                    case "logout":
                        Console.Clear();
                        InUse = false;
                        Console.WriteLine($"Thank you {User.Username} for using *Beacon*!");
                        break;
                    default:
                        Console.WriteLine("Please use one of the available actions.");
                        break;
                }
            }
        }
        internal void TrustedScreen(Trusted User)
        {
            bool InUse = true;
            Console.Clear();
            Console.WriteLine("*Beacon Messenger System* est.2018!");
            Console.WriteLine($"Welcome back {User.Username}");
            while (InUse == true)
            {                               
                Console.WriteLine("Please follow the instructions provided below.");
                Console.WriteLine("====================");
                Console.WriteLine("Choose action:");
                Console.WriteLine("view - View messages");
                Console.WriteLine("change - Change password");
                Console.WriteLine("logout - Logout");
                Console.WriteLine("====================");
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
                        Console.WriteLine("edit - Edit message");                       
                        Console.WriteLine("delete - Delete message");                        
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
                                Console.WriteLine("Copy and Paste the date/time of the message you want to alter:");
                                int Stamp =int.Parse(Console.ReadLine());
                                Console.WriteLine("Please type your message. (Limit = 250 characters)");
                                string textMessage = Console.ReadLine();
                                User.Edit(Stamp, textMessage, User2);
                                break;
                            case "delete":
                                Console.WriteLine("Copy and Paste the date/time of the message you want to delete:");
                                int Stamp1 = int.Parse(Console.ReadLine());                                
                                User.Delete(Stamp1, User2);
                                break;
                            default:
                                Console.WriteLine("Please use one of the available actions.");
                                break;
                        }
                        break;
                    case "change":
                        Console.Clear();
                        User.ChangePassword();
                        break;
                    case "logout":
                        Console.Clear();
                        InUse = false;
                        Console.WriteLine($"Thank you {User.Username} for using *Beacon*!");
                        break;
                    default:
                        Console.WriteLine("Please use one of the available actions.");
                        break;
                }
            }
        }

        internal void AdminScreen(Admin User)
        {
            bool InUse = true;
            Console.Clear();
            Console.WriteLine("*Beacon Messenger System* est.2018!");
            Console.WriteLine($"Welcome back {User.Username}");
            while (InUse == true)
            {
                Console.WriteLine("Please follow the instructions provided below.");
                Console.WriteLine("====================");
                Console.WriteLine("Choose action:");
                Console.WriteLine("view - View messages");
                Console.WriteLine("admin - Admin actions");
                Console.WriteLine("change - Change password");
                Console.WriteLine("logout - Logout");
                Console.WriteLine("====================");
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
                        bool UserOtherInSystem = true;

                        while (UserOtherInSystem == true)
                        {
                            UserOtherInSystem = Namecheck(User2);
                            if (UserOtherInSystem == true)
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
                        Console.WriteLine("edit - Edit message");
                        Console.WriteLine("delete - Delete message");
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
                                Console.WriteLine("Copy and Paste the date/time of the message you want to alter:");
                                int Stamp = int.Parse(Console.ReadLine());
                                Console.WriteLine("Please type your message. (Limit = 250 characters)");
                                string textMessage = Console.ReadLine();
                                User.Edit(Stamp, textMessage, User2);
                                break;
                            case "delete":
                                Console.WriteLine("Copy and Paste the date/time of the message you want to delete:");
                                int Stamp1 = int.Parse(Console.ReadLine());
                                User.Delete(Stamp1, User2);
                                break;
                            default:
                                Console.WriteLine("Please use one of the available actions.");
                                break;
                        }
                        break;
                    case "admin":
                        Console.Clear();
                        Console.WriteLine("list - List registered users");
                        Console.WriteLine("==============OR==============");
                        Console.WriteLine("return - Return to Main Menu");
                        string AdminSelection = Console.ReadLine();

                        switch (AdminSelection.ToLower())
                        {
                            case "return":
                                break;
                            case "list":
                                Console.Clear();
                                User.ListUsers();
                                Console.WriteLine("Choose action:");
                                Console.WriteLine("create - Create a new account");
                                Console.WriteLine("update - Change a user's username");
                                Console.WriteLine("promote - Promote an existing user");
                                Console.WriteLine("demote - Demote an existing user");
                                Console.WriteLine("delete - Delete an existing user");
                                Console.WriteLine("==============OR==============");
                                Console.WriteLine("return - Return to Previous Screen");
                                string SuperUserSelection = Console.ReadLine();
                                string UserOther = "";
                                bool UserInList = true;
                                switch (SuperUserSelection.ToLower())
                                {
                                    case "return":
                                        Console.Clear();
                                        break;
                                    case "create":
                                        User.Create();
                                        break;
                                    case "update":
                                        Console.WriteLine("Please pick a user from the list:");
                                        UserOther = Console.ReadLine();
                                        
                                        while (UserInList == true)
                                        {
                                            UserInList = Namecheck(UserOther);
                                            if (UserInList == true)
                                            {
                                                Console.Clear();
                                                Console.WriteLine($"The user {UserOther} cannot be found!");
                                                Console.WriteLine("Please enter a valid username:");
                                                UserOther = Console.ReadLine();
                                            }
                                        }
                                        User.Update(UserOther);
                                        break;
                                    case "promote":
                                        Console.WriteLine("Please pick a user from the list:");
                                        UserOther = Console.ReadLine();
                                        while (UserInList == true)
                                        {
                                            UserInList = Namecheck(UserOther);
                                            if (UserInList == true)
                                            {
                                                Console.Clear();
                                                Console.WriteLine($"The user {UserOther} cannot be found!");
                                                Console.WriteLine("Please enter a valid username:");
                                                UserOther = Console.ReadLine();
                                            }
                                        }
                                        User.Promote(UserOther);
                                        break;
                                    case "demote":
                                        Console.WriteLine("Please pick a user from the list:");
                                        UserOther = Console.ReadLine();
                                        while (UserInList == true)
                                        {
                                            UserInList = Namecheck(UserOther);
                                            if (UserInList == true)
                                            {
                                                Console.Clear();
                                                Console.WriteLine($"The user {UserOther} cannot be found!");
                                                Console.WriteLine("Please enter a valid username:");
                                                UserOther = Console.ReadLine();
                                            }
                                        }
                                        User.Demote(UserOther);
                                        break;
                                    case "delete":
                                        Console.WriteLine("Please pick a user from the list:");
                                        UserOther = Console.ReadLine();
                                        while (UserInList == true)
                                        {
                                            UserInList = Namecheck(UserOther);
                                            if (UserInList == true)
                                            {
                                                Console.Clear();
                                                Console.WriteLine($"The user {UserOther} cannot be found!");
                                                Console.WriteLine("Please enter a valid username:");
                                                UserOther = Console.ReadLine();
                                            }
                                        }
                                        Console.Clear();
                                        string SecurityCheck = "";
                                        while (SecurityCheck.ToLower() != "y" && SecurityCheck.ToLower() != "n")
                                        {
                                            Console.WriteLine($"The user {UserOther} will be PERMANENTLY DELETED");
                                            Console.WriteLine("as will all the conversations he/she has participated!");
                                            Console.WriteLine("ARE YOU SURE?");
                                            Console.WriteLine("[ Y / N]");
                                            SecurityCheck = Console.ReadLine();
                                        }
                                        if (SecurityCheck.ToLower() == "y")
                                        {
                                            User.Destroy(UserOther);
                                        }
                                        else
                                        {
                                            break;
                                        }                                        
                                        break;
                                    default:
                                        Console.WriteLine("Please use one of the available actions.");
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine("Please use one of the available actions.");
                                break;
                        }
                        break;
                    case "change":
                        Console.Clear();
                        User.ChangePassword();
                        break;
                    case "logout":
                        Console.Clear();
                        InUse = false;
                        Console.WriteLine($"Please {User.Username},you are our only hope!");
                        break;
                    default:
                        Console.WriteLine("Please use one of the available actions.");
                        break;
                }
            }
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

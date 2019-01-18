using System;

namespace Beacon
{
    enum Authorization { Guest, Member, Trusted, Administrator };  
    internal class User
    {

        internal string Username;
        internal Authorization Rank;        
    }

    internal class Guest : User, IViewHistory, ISendMessage
    {
             
        public Guest(string Name, Authorization Authority)
        {
            Username = Name;         
            Rank = Authority;
        }

        public void ChangePassword()
        {
            string pass1;
            string pass2;

            do
            {

                Console.WriteLine("Please pick a password:");
                pass1 = Console.ReadLine();

                Console.WriteLine("Please enter your password a second time:");
                pass2 = Console.ReadLine();

                if (pass1 != pass2)
                {
                    Console.WriteLine("The passwords do not match!");
                }

            } while (pass1 != pass2);

            BaseInteraction.PassChange(pass1, Username);
        }

        public void Send(string User2)
        {
            Console.WriteLine("Please type your message. (Limit = 250 characters)");
            string textMessage = Console.ReadLine();            

            int stamp =  BaseInteraction.SendMessage(Username, User2, textMessage);
            Console.Clear();
            View(User2);
            Data lastSended = BaseInteraction.GetLastSended(stamp);
            TextOutput.NewText(lastSended);
        }

        public void View(string User2)
        {
            var messages = BaseInteraction.GetMessages(Username, User2);

            foreach (var m in messages)
            {
                Console.WriteLine($"From:{m.Sender}");
                Console.WriteLine($"To:{m.Receiver}, at: {m.Submission}");
                Console.WriteLine($"{m.Message}");
                Console.WriteLine($"MessageID:{m.Stamp}");
                Console.WriteLine("========================================");

            }

        }
    }

    internal class Member : Guest, IEditMessage
    {

        public Member(string Name,  Authorization Authority) : base(Name, Authority)
        {

        }


        public void Edit(int Stamp, string User2)
        {
            string sender = Username;
            string receiver = User2;
            int stampSelected = Stamp;
            bool securityCheck = BaseInteraction.SenderIdMatching(sender, stampSelected);

            if (securityCheck == true)
            {
                Console.WriteLine("Please type your message. (Limit = 250 characters)");
                string textMessage = Console.ReadLine();
                BaseInteraction.EditMessage(textMessage, sender, receiver, stampSelected);                
                Console.Clear();
                Console.WriteLine("Message has been altered!");
                View(User2);
                Data message = BaseInteraction.GetLastSended(stampSelected);
                TextOutput.EditText(message);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("You can only edit messages YOU have sent!");
            }                                               
        }
    }

    internal class Trusted : Member, IDeleteMessage
    {
        public Trusted(string Name,  Authorization Authority) : base(Name, Authority)
        {

        }

        public void Delete(int Stamp, string User2)
        {
            string sender = Username;
            string receiver = User2;
            int stampSelected = Stamp;
            bool securityCheck = BaseInteraction.SenderIdMatching(sender, stampSelected);

            if (securityCheck == true)
            {
                BaseInteraction.MessageDeletion(sender, receiver, stampSelected);
                Console.Clear();
                Console.WriteLine("Message has been deleted!");
                View(User2);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("You can only delete messages YOU have sent!");
            }

            
        }
    }

    internal class Admin : Trusted, IGiveAuthority, IUserManipulation
    {
        public Admin(string Name, Authorization Authority) : base(Name, Authority)
        {

        }

        public void Create()
        {
            bool nameOriginal = false;
            string name = "";

            while (nameOriginal == false)
            {
                Console.WriteLine("Please pick a username:");
                name = Console.ReadLine();

                nameOriginal = BaseInteraction.Namecheck(name);

                if (nameOriginal == false)
                {
                    Console.WriteLine("This Username is already taken!");
                    Console.WriteLine("Please choose something else.");
                }

            }

            string pass1 = "0000";            

            BaseInteraction.Registration(name, pass1);
            Console.Clear();
            ListUsers();
        }

        public void Demote(string User2)
        {
            string rank = BaseInteraction.RankCheck(User2);
            string newRank = "";

            if (rank == "Administrator")
            {
                newRank = newRank + "Trusted";
            }
            else if (rank == "Trusted")
            {
                newRank = newRank + "Member";
            }
            else if (rank == "Member")
            {
                newRank = newRank + "Guest";
            }
            else
            {
                Console.WriteLine($"{User2} cannot be demoted further!");
                return;
            }

            BaseInteraction.Demotion(newRank, User2);
            Console.WriteLine($"{User2} has been demoted to {newRank}!");
            Console.Clear();
            ListUsers();
        }

        public void Destroy(string User2)
        {
            BaseInteraction.DeleteUser(User2);

            Console.WriteLine($"{User2} has been deleted!");
            ListUsers();
        }

        public void Promote(string User2)
        {
            string rank = BaseInteraction.RankCheck(User2);
            string newRank = "";

            if (rank == "Guest")
            {
                newRank = newRank + "Member";
            }
            else if (rank == "Member")
            {
                newRank = newRank + "Trusted";
            }
            else if (rank == "Trusted")
            {
                newRank = newRank + "Administrator";
            }
            else
            {
                Console.WriteLine($"{User2} cannot be promoted further!");
                return;
            }

            BaseInteraction.Promotion(newRank, User2);
            Console.Clear();
            Console.WriteLine($"{User2} has been promoted to {newRank}!");
            ListUsers();
        }

        public void Update(string User2)
        {
            Console.WriteLine($"Pick a new username for {User2}:");
            string usernameNEW = Console.ReadLine();

            BaseInteraction.UpdateInfo(usernameNEW, User2);
            Console.Clear();
            Console.WriteLine($"{User2} has been changed to {usernameNEW}");
            ListUsers();
        }

        public void ListUsers()
        {
            var users = BaseInteraction.GetList();

            foreach (var user in users)
            {
                Console.WriteLine(user.Username + " - " + user.Rank);
            }
        }
       
    }

    public class Data
    {
        
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime Submission { get; set; }
        public string Message { get; set; }
        public int Stamp { get; set; }
        
    }


}

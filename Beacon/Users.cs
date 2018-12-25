using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beacon
{
    enum Authorization { Guest, Member, Trusted, Administrator };

    //TODO Add UserID
    internal abstract class User
    {

        protected string UserName;
        protected string PassWord;
        protected Authorization Rank;
        
    }

    internal class Guest : User
    {
        public Guest(string Name, string Pass, Authorization Authority)
        {
            UserName = Name;
            PassWord = Pass;
            Rank = Authority;
        }
     
    }
    //TODO add Interfaces
    internal class Member : Guest
    {
        public Member(string Name, string Pass, Authorization Authority) : base(Name, Pass, Authorization.Member)
        {

        }
    }

    internal class Trusted : Member
    {
        public Trusted(string Name, string Pass, Authorization Authority) : base(Name, Pass, Authorization.Trusted)
        {

        }
    }

    internal class Admin : Trusted
    {
        public Admin(string Name, string Pass, Authorization Authority) : base(Name, Pass, Authorization.Administrator)
        {

        }
    }
}

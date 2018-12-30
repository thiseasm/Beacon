﻿using System;
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

    internal class Guest : User, ViewHistory
    {
        public Guest(string Name, Authorization Authority)
        {
            UserName = Name;         
            Rank = Authority;
        }

        public void View()
        {
            throw new NotImplementedException();
        }
    }
    //TODO add Interfaces
    internal class Member : Guest, EditMessage
    {
        public Member(string Name,  Authorization Authority) : base(Name,  Authorization.Member)
        {

        }

        public void Edit()
        {
            throw new NotImplementedException();
        }
    }

    internal class Trusted : Member, DeleteMessage
    {
        public Trusted(string Name,  Authorization Authority) : base(Name,  Authorization.Trusted)
        {

        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }

    internal class Admin : Trusted, GiveAuthority, CreateUser
    {
        public Admin(string Name, Authorization Authority) : base(Name, Authorization.Administrator)
        {

        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        public void Demote()
        {
            throw new NotImplementedException();
        }

        public void Promote()
        {
            throw new NotImplementedException();
        }
    }
}

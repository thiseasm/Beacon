using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beacon
{
    interface ISendMessage
    {
        void Send(string User2);
    }

    interface IViewHistory
    {
        void View(string User2);
    }

    interface IEditMessage
    {
        void Edit(int Stamp, string Text, string User2);
    }

    interface IDeleteMessage
    {
        void Delete(int Stamp, string User2);
    }

    interface IGiveAuthority
    {
        void Promote(string User2);
        void Demote(string User2);
    }

    interface IUserManipulation
    {
        void Create();
        void Destroy(string User2);
        void Update(string User2);
        void ListUsers();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beacon
{
    interface SendMessage
    {
        void Send(string User2);
    }

    interface ViewHistory
    {
        void View(string User2);
    }

    interface EditMessage
    {
        void Edit(int Stamp, string Text, string User2);
    }

    interface DeleteMessage
    {
        void Delete(int Stamp, string User2);
    }

    interface GiveAuthority
    {
        void Promote(string User2);
        void Demote(string User2);
    }

    interface UserManipulation
    {
        void Create();
        void Destroy(string User2);
        void Update(string User2);
        void ListUsers();
    }
}

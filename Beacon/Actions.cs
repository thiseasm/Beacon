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
        void Edit();
    }

    interface DeleteMessage
    {
        void Delete();
    }

    interface GiveAuthority
    {
        void Promote(string User2);
        void Demote(string User2);
    }

    interface UserManipulation
    {
        void Create();
        void Destroy();
        void Update();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beacon
{
    interface ViewHistory
    {
        void View();
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
        void Promote();
        void Demote();
    }
}

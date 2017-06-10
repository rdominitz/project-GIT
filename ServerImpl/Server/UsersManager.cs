using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class UsersManager
    {
        private IMedTrainDBContext _db;

        public Tuple<string, int> register(string eMail, string password, string medicalTraining, string firstName, string lastName)
        {
            return null;
        }

        // has tests - 100% coverage
        public Tuple<string, int> login(string eMail, string password)
        {
            return null;
        }

        // has tests - 100% coverage
        public string restorePassword(string eMail)
        {
            return "";
        }

        public string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin)
        {
            return "";
        }
    }
}

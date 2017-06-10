using Constants;
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

        public UsersManager(IMedTrainDBContext db)
        {
            _db = db;
        }

        public Tuple<string, int> register(string eMail, string password, string medicalTraining, string firstName, string lastName)
        {
            return null;
        }

        public Tuple<string, User> login(string eMail, string password)
        {
            // search DB
            User user = _db.getUser(eMail);
            if (user == null)
            {
                return new Tuple<string, User>("Wrong eMail or password.", null);
            }
            // if found add to cache and return relevant message as shown above
            if (!user.userPassword.Equals(password))
            {
                return new Tuple<string, User>("Wrong password", null);
            }
            return new Tuple<string, User>(Replies.SUCCESS, user);
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

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
        private int _userUniqueInt;
        private readonly object _syncLockUserUniqueInt;

        public UsersManager(IMedTrainDBContext db)
        {
            _db = db;
            _userUniqueInt = 100000;
            _syncLockUserUniqueInt = new object();
        }

        public Tuple<string, int> register(string eMail, string password, string medicalTraining, string firstName, string lastName)
        {
            int userUniqueInt = 0; 
            User user = null;
            lock (_syncLockUserUniqueInt)
            {
                // search DB
                if (_db.getUser(eMail) != null)
                {
                    return new Tuple<string, int>(Replies.EMAIL_IN_USE, -1);
                }
                // if DB contains user with that eMail return error message
                userUniqueInt = _userUniqueInt;
                user = new User { UserId = eMail, userPassword = password, userMedicalTraining = medicalTraining, userFirstName = firstName, userLastName = lastName, uniqueInt = _userUniqueInt };
                _userUniqueInt++;
            }
            // add to DB
            _db.addUser(user);
            return new Tuple<string, int>(Replies.SUCCESS, userUniqueInt);
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

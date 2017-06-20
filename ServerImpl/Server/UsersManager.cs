using Constants;
using Entities;
using System;
using System.Text;
using DB;

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

        public Tuple<string, int> register(string email, string password, string medicalTraining, string firstName, string lastName)
        {
            int userUniqueInt = 0; 
            User user = null;
            lock (_syncLockUserUniqueInt)
            {
                // search DB
                if (_db.getUser(email) != null)
                {
                    return new Tuple<string, int>(Replies.EMAIL_IN_USE, -1);
                }
                // if DB contains user with that email return error message
                userUniqueInt = _userUniqueInt;
                user = new User { UserId = email, userPassword = password, userMedicalTraining = medicalTraining, userFirstName = firstName, userLastName = lastName, uniqueInt = _userUniqueInt };
                _userUniqueInt++;
            }
            // add to DB
            _db.addUser(user);
            return new Tuple<string, int>(Replies.SUCCESS, userUniqueInt);
        }

        public Tuple<string, User> login(string email, string password)
        {
            // search DB
            User user = _db.getUser(email);
            if (user == null)
            {
                return new Tuple<string, User>("Wrong email or password.", null);
            }
            // if found add to cache and return relevant message as shown above
            if (!user.userPassword.Equals(password))
            {
                return new Tuple<string, User>("Wrong password", null);
            }
            return new Tuple<string, User>(Replies.SUCCESS, user);
        }

        public string restorePassword(string email)
        {
            // search user in DB
            User user = _db.getUser(email);
            // if doesn't exist return error message
            if (user == null)
            {
                return "email address does not exist in the system.";
            }
            // send email
            StringBuilder sb = new StringBuilder();
            sb.Append("Hello " + user.userFirstName + " " + user.userLastName + "," + Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Your password for our system is: " + user.userPassword + Environment.NewLine);
            EmailSender.sendMail(email, "Medical Training System Password Restoration", sb.ToString());
            return Replies.SUCCESS;
        }

        public string setUserAsAdmin(string usernameToTurnToAdmin)
        {
            User u = _db.getUser(usernameToTurnToAdmin);
            // verify user exist
            if (u == null)
            {
                return "Error. Cannot make " + usernameToTurnToAdmin + " an admin since " + usernameToTurnToAdmin + " is not registered to the system.";
            }
            _db.addAdmin(new Admin { AdminId = u.UserId });
            _db.SaveChanges();
            return Replies.SUCCESS;
        }
    }
}

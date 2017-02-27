using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public class Users
    {
        public static int USER_UNIQUE_INT = 100000;
        public static readonly IList<string> medicalTrainingLevels = new ReadOnlyCollection<string>
            (new List<string> 
            { 
                "Pre-Medical student",
                "Medical student - 1st year",
                "Medical student - 2nd year",
                "Medical student - 3rd year",
                "Medical student - 4th year +",
                "Resident PGY 1",
                "Resident PGY 2",
                "Resident PGY 3",
                "Resident PGY 4",
                "Resident PGY 5",
                "Resident PGY 6",
                "Resident PGY 7",
                "Fellow", 
                "Attending" 
            });
    }
}

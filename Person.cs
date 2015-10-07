using System;
using System.Data;
using System.Collections;
using System.Text;

namespace EmailSender
{
    class Person
    {
        private const char DELIMITER = '/';
        private const string PERSON_NAME_DELIMITER = " and ";
        private const string DEFAULT_COUNTRY = "USA";

        private int ID;
        private string firstName;
        private string lastName;
        private string address;
        private string city;
        private string state;
        private string zip;
        private string country;
        private string email;
        private ArrayList donations;

        public Person(DataRow personData)
        {
            ID = Convert.ToInt32(personData["MailingListID"]);
            firstName = personData["FirstName"].ToString();
            lastName = personData["LastName"].ToString();
            address = personData["Address"].ToString();
            city = personData["City"].ToString();
            state = personData["State"].ToString();
            zip = personData["PostalCode"].ToString();
            country = personData["Country/Region"].ToString();
            email = personData["EmailAddress"].ToString();
            donations = new ArrayList();
        }

        public int GetID()
        {
            return ID;
        }

        public ArrayList GetDonations()
        {
            return donations;
        }

        // The church's data is extremely messy, and names are stored horribly.  This function cleans up the values we read from the database.
        public string GetFullName()
        {
            // Sometimes, names are delimited using slashes.  Split these up:
            string[] firstNames = firstName.Split(new Char[] { DELIMITER });
            string[] lastNames = lastName.Split(new Char[] { DELIMITER });

            StringBuilder returnStringBuilder = new StringBuilder();

            // Singular person with one first name and one last name:
            if ((firstNames.Length == 1) && (lastNames.Length == 1))
            {
                returnStringBuilder.Append(firstNames[0]).Append(" ").Append(lastNames[0]);
            }
            // Two people with different first names but the same last name:
            else if ((firstNames.Length == 2) && (lastNames.Length == 1))
            {
                returnStringBuilder.Append(firstNames[0]).Append(PERSON_NAME_DELIMITER).Append(firstNames[1]).Append(" ").Append(lastNames[0]);
            }
            // Two people with the same first name and different last names:
            else if ((firstNames.Length == 1) && (lastNames.Length == 2))
            {
                returnStringBuilder.Append(firstNames[0]).Append(" ").Append(lastNames[0]).Append(PERSON_NAME_DELIMITER).Append(firstNames[0]).Append(" ").Append(lastNames[1]);

            }
            // Two people with different first and last names:
            else
            {
                returnStringBuilder.Append(firstNames[0]).Append(" ").Append(lastNames[0]).Append(PERSON_NAME_DELIMITER).Append(firstNames[1]).Append(" ").Append(lastNames[1]);
            }
            return returnStringBuilder.ToString();
        }

        // This should use a template!  TODO (address.template)
        public string GetMailingAddress()
        {
            Hashtable addressValues = new Hashtable();
            addressValues.Add("address", address);

            StringBuilder returnStringBuilder = new StringBuilder(address).Append("\n<br>").Append(city).Append(", ").Append(state).Append(" ").Append(zip);
            if (String.Compare(this.country, DEFAULT_COUNTRY) != 0)
            {
                returnStringBuilder.Append("\n<br>").Append(country);
            }
            return returnStringBuilder.ToString();
        }

        // Parse out the first email and return that, if it exists.
        public string GetEmail()
        {
            if (email.Length == 0)
            {
                throw new Exception("Error: there is no email in the database for this user.");
            }
            string [] emailList = email.Replace(" ", "").Split(new Char [] { DELIMITER });
            return emailList[0];
        }
    }
}

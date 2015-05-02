using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;

namespace EmailSender
{
    class Person
    {

        public int ID;
        public string firstName;
        public string lastName;
        public string address;
        public string city;
        public string state;
        public string zip;
        public string country;
        public string email;
        public ArrayList donations;

        public Person(DataRow personData)
        {
            this.ID = Convert.ToInt32(personData["MailingListID"]);
            this.firstName = personData["FirstName"].ToString();
            this.lastName = personData["LastName"].ToString();
            this.address = personData["Address"].ToString();
            this.city = personData["City"].ToString();
            this.state = personData["State"].ToString();
            this.zip = personData["PostalCode"].ToString();
            this.country = personData["Country/Region"].ToString();
            this.email = personData["EmailAddress"].ToString();
            this.donations = new ArrayList();
        }

        public string GetFullName()
        {
            string[] firstNames = firstName.Split(new Char[] {'/'});
            string[] lastNames = lastName.Split(new Char[] {'/'});
            // If singular person with one first name and one last name
            if ((firstNames.Length == 1) && (lastNames.Length == 1))
            {
                return firstNames[0] + " " + lastNames[0];
            }
            // If two people with different first names but the same last name
            if ((firstNames.Length == 2) && (lastNames.Length == 1))
            {
                return firstNames[0] + " and " + firstNames[1] + " " + lastNames[0];
            }
            // If two people with the same first name and different last names
            if ((firstNames.Length == 1) && (lastNames.Length == 2))
            {
                return firstNames[0] + " " + lastNames[0] + " and " + firstNames[0] + " " + lastNames[1];
            }
            // If two people with different first and last names
            return firstNames[0] + " " + lastNames[0] + " and " + firstNames[1] + " " + lastNames[1];
        }

        public string GetMailingAddress()
        {
            string returnString = address + "\n<br>" + city + ", " + state + " " + zip;
            if (String.Compare(this.country, "USA") != 0)
            {
                returnString += "\n" + country;
            }
            return returnString;
        }

        public string GetValidEmail()
        {
            if (email.Length == 0)
            {
                throw new Exception("Error: there is no email in the database for this user.");
            }
            string [] emailList = email.Replace(" ", "").Split(new Char [] {'/'});
            return emailList[0];
        }
    }
}

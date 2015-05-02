using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Xml.Serialization;
using System.Collections;
using Pechkin;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Windows.Resources;
using System.Windows.Forms;
using System.Reflection;

namespace EmailSender
{
    class Program
    {
        public static byte[] CreatePDF(string html, Hashtable values)
        {
            foreach (DictionaryEntry entry in values)
            {
                string toReplace = "{{" + entry.Key + "}}";
                html = html.Replace(toReplace, (string)entry.Value);
            }
            return new SimplePechkin(new GlobalConfig()).Convert(html);
        }

        public static void SavePDF(string path, byte[] bytefile)
        {
            File.WriteAllBytes(path, bytefile);
        }

        public static void EmailPDF(string username, string password, string recipientEmail, string subject, string body, byte[] bytefile)
        {
            MailAddress mailfrom = new MailAddress(username + "@gmail.com");
            MailAddress mailto = new MailAddress(recipientEmail);
            MailMessage newmsg = new MailMessage(mailfrom, mailto);
    
            newmsg.Subject = subject;
            newmsg.Body = body;
    
            Attachment att = new Attachment(new MemoryStream(bytefile), "Donations_Record.PDF");
            newmsg.Attachments.Add(att);
    
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(username, password);
            smtp.EnableSsl = true;
            smtp.Send(newmsg);
        }

        // Returns a list of users with their associated donations in hashtable format.
        public static Hashtable InitializeData(string year, string filename)
        {
            // Get data from database
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename;
            DataTable peopleTable = ExecuteQuery(connectionString, "Select * from [mailing list]").Tables["Table"];
            DataTable donationsTable = ExecuteQuery(connectionString, "Select * from donation where Date >= #01/01/" + year + "# and Date <= #12/31/" + year + "#").Tables["Table"];

            // Parse data
            Hashtable userList = new Hashtable();
            foreach(DataRow personRow in peopleTable.Rows)
            {
                Person person = new Person(personRow);
                userList.Add(person.ID, person);
            }

            foreach(DataRow donationRow in donationsTable.Rows)
            {
                Donation donation = new Donation(donationRow);
                ((Person)userList[donation.ID]).donations.Add(donation);
            }
            return userList;
        }

        public static string readResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        static DataSet ExecuteQuery(string connectionString, string query)
        {
            DataSet returnSet = new DataSet();
            OleDbConnection connection = null;
            connection = new OleDbConnection(connectionString);

            OleDbCommand myAccessCommand = new OleDbCommand(query, connection);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);

            connection.Open();
            myDataAdapter.Fill(returnSet);
            connection.Close();
            return returnSet;
        }
    }
}
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace EmailSender
{
    class DbUtilities
    {
        private const string CONNECTION_PROVIDER = "Microsoft.Jet.OLEDB.4.0";
        private const string DEFAULT_TABLE_NAME = "Table";
        private const string PEOPLE_QUERY = "select * from [mailing list] where [MailingListID] in (select distinct [Maillist id] from donation where Year(Date) = ?)";
        private const string DONATIONS_QUERY = "select * from donation where Year(Date) = ?";

        // Returns a list of users with their associated donations in hashtable format.
        public static Hashtable RetrieveDBData(string year, string filename)
        {
            // Get data from database
            OleDbConnectionStringBuilder connectionBuilder = new OleDbConnectionStringBuilder();
            connectionBuilder.Provider = CONNECTION_PROVIDER;
            connectionBuilder.DataSource = filename;
            OleDbConnection connection = new OleDbConnection(connectionBuilder.ToString());

            OleDbCommand peopleQueryCommand = new OleDbCommand(PEOPLE_QUERY, connection);
            OleDbCommand donationsQueryCommand = new OleDbCommand(DONATIONS_QUERY, connection);

            // Prepare statements using AddWithValue to prevent SQLInjection
            peopleQueryCommand.Parameters.AddWithValue("?", year);
            donationsQueryCommand.Parameters.AddWithValue("?", year);

            connection.Open();

            DataTable peopleTable = ExecuteQuery(connection, peopleQueryCommand).Tables[DEFAULT_TABLE_NAME];
            DataTable donationsTable = ExecuteQuery(connection, donationsQueryCommand).Tables[DEFAULT_TABLE_NAME];

            connection.Close();

            // Parse data into appropriate objects:
            Hashtable userList = new Hashtable(); // IDs aren't always consecutive, so use a hashtable to save a bit of space with IDs as keys.
            foreach (DataRow personRow in peopleTable.Rows)
            {
                Person person = new Person(personRow);
                userList.Add(person.GetID(), person);
            }

            foreach (DataRow donationRow in donationsTable.Rows)
            {
                Donation donation = new Donation(donationRow);
                ((Person) userList[donation.GetID()]).GetDonations().Add(donation);
            }
            return userList;
        }

        private static DataSet ExecuteQuery(OleDbConnection connection, OleDbCommand command)
        {
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            adapter.Fill(dataSet);
            return dataSet;
        }
    }
}

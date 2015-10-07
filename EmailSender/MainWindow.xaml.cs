using Microsoft.Win32;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;

namespace EmailSender
{
    public partial class MainWindow : Window
    {
        private const string FILTER_OPTIONS = "MDB Files (.MDB)|*.MDB|All Files (*.*)|*.*";
        private static string DEFAULT_OUTPUT_DIR = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Output";

        public MainWindow()
        {
            InitializeComponent();
            DirectoryPath.Text = DEFAULT_OUTPUT_DIR;
        }

        public void SelectDatabaseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = FILTER_OPTIONS;
            openFileDialog.FilterIndex = 1;
            bool? userClickedOK = openFileDialog.ShowDialog();

            if (userClickedOK == true)
            {
                SelectedFile.Text = openFileDialog.FileName; // Open the selected file to read.
            }
        }

        public void Execute_Click(object sender, RoutedEventArgs e)
        {
            string year = QueryYear.Text;
            string filename = SelectedFile.Text;
            string outputDirectory = DirectoryPath.Text;
            Hashtable people = null;
            try
            {
                Log("Reading from Database");
                people = DbUtilities.RetrieveDBData(year, filename); // Get data from database
            }
            catch (Exception ex)
            {
                Log("Error: could not load database.  " + ex.Message);
                return;
            }

            // Setup output directory
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Write out data to PDFs
            foreach (DictionaryEntry entry in people)
            {
                Person person = (Person)entry.Value;
                Log("Generating PDF for " + person.GetFullName());
                PDFData pdfData = new PDFData(person, year);

                if ((bool)SavePDFsOption.IsChecked)
                {
                    try
                    {
                        pdfData.WriteToFile(outputDirectory + "/");
                    }
                    catch (Exception ex)
                    {
                        Log("Error: PDF cannot be saved.  Try closing the output folder and trying again.  " + ex.Message);
                    }
                    Log("\tSaved PDF.");
                }

                if ((bool)SendEmails.IsChecked)
                {
                    try
                    {
                        EmailData email = new EmailData(pdfData);
                        email.recipientEmail = person.GetEmail();
                        email.subject = EmailSubject.Text;
                        email.body = EmailBody.Text;
                        email.Send(GmailUsername.Text, GmailPassword.Password);

                        Log("\tEmailed PDF.");
                    }
                    catch (Exception ex)
                    {
                        Log("\tError: email will not be sent.  " + ex.Message);
                    }
                }
            }
            Log("Done.  Evaluated " + people.Count + " records.");
        }

        private void Log(string logEntry)
        {
            LogTextbox.AppendText(logEntry + "\n");
        }
    }
}

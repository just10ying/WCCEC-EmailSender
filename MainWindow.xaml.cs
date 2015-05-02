using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EmailSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string dbFile;

        public MainWindow()
        {
            InitializeComponent();
            DirectoryPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Output";
        }

        public void SelectDatabaseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MDB Files (.MDB)|*.MDB|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            bool? userClickedOK = openFileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                dbFile = openFileDialog.FileName;
                SelectedFile.Text = dbFile;
            }
        }

        public void Execute_Click(object sender, RoutedEventArgs e)
        {
            WriteToLog("Reading from Database");
            
            int numRecords = 0;

            string year = QueryYear.Text;
            string filename = dbFile;
            string outputDirectory = DirectoryPath.Text;
            Hashtable people = null;
            try
            {
                people = Program.InitializeData(year, filename); // Get data from database
            }
            catch (Exception ex)
            {
                WriteToLog("Error: could not load database.  " + ex.Message);
                return;
            }
            Hashtable pdfValues; // Holds the values that should be written out to the PDF.

            // Setup output directory
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }
            Directory.CreateDirectory(outputDirectory);

            // Write out data to PDFs
            foreach (DictionaryEntry entry in people)
            {
                Person person = (Person)entry.Value;
                if (person.donations.Count > 0)
                {
                    WriteToLog("Generating PDF for " + person.GetFullName());

                    double totalDonation = 0;
                    string donationsString = "";
                    foreach (Donation donation in person.donations)
                    {
                        donationsString += "<tr><td>" + donation.date.ToString("MMMM dd, yyyy") + "</td><td>" + donation.checkNumber + "</td><td>" + donation.fundName + "</td><td>$" + donation.fundAmount + "</td><td>$" + donation.amount + "</td></tr>";
                        totalDonation += donation.amount;
                    }

                    // Reset the hash table.
                    pdfValues = new Hashtable();
                    pdfValues.Add("FullName", person.GetFullName());
                    pdfValues.Add("TotalDonation", totalDonation.ToString());
                    pdfValues.Add("Year", year);
                    pdfValues.Add("ID", person.ID.ToString());
                    pdfValues.Add("FullAddress", person.GetMailingAddress());
                    pdfValues.Add("DonationsString", donationsString);

                    byte[] pdfBytes = Program.CreatePDF(Program.readResource("EmailSender.templates.PdfTemplate.html"), pdfValues);
                    
                    if ((bool)SavePDFsOption.IsChecked)
                    {
                        try
                        {
                            string path = outputDirectory + "/" + person.ID.ToString() + ".pdf";
                            Program.SavePDF(path, pdfBytes);
                        }
                        catch (Exception ex)
                        {
                            WriteToLog("Error: PDF cannot be saved.  Try closing the output folder and trying again.  " + ex.Message);
                        }
                        WriteToLog("\tSaving PDF.");
                    }

                    string recipientEmail;
                    if ((bool)SendEmails.IsChecked)
                    {
                        try
                        {
                            recipientEmail = person.GetValidEmail();
                            Program.EmailPDF(GmailUsername.Text, GmailPassword.Password, recipientEmail, EmailSubject.Text, EmailBody.Text, pdfBytes);
                            WriteToLog("\tEmailing PDF.");
                        }
                        catch (Exception ex)
                        {
                            WriteToLog("\tError: email will not be sent.  " + ex.Message);
                        }
                    }
                    numRecords++;
                }
            }
            WriteToLog("Done.  Evaluated " + numRecords.ToString() + " records.");
        }

        public void WriteToLog(string logEntry)
        {
            Log.Text = Log.Text + logEntry + "\n";
        }
    }
}

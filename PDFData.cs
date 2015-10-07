using Pechkin;
using System.Collections;
using System.Text;

namespace EmailSender
{
    class PDFData
    {

        private const string TEMPLATE_PATH = "templates/pdf.template";
        private string fileName;
        private byte[] pdfContents;
        
        public PDFData(Person person, string year)
        {
            StringBuilder donationsSB = new StringBuilder();
            double donationsTotal = 0;
            foreach (Donation donation in person.GetDonations())
            {
                donationsSB.Append(donation.ToTemplateString());
                donationsTotal += donation.GetAmount();
            }

            Hashtable values = new Hashtable();
            values.Add("FullName", person.GetFullName());
            values.Add("TotalDonation", donationsTotal);
            values.Add("Year", year);
            values.Add("ID", person.GetID().ToString());
            values.Add("FullAddress", person.GetMailingAddress());
            values.Add("DonationsString", donationsSB.ToString());

            fileName = person.GetID().ToString() + "-" + person.GetFullName() + "(" + year + ").pdf";

            string contentString = FileUtilities.PopulateTemplate(TEMPLATE_PATH, values);
            pdfContents = new SimplePechkin(new GlobalConfig()).Convert(contentString);
        }

        public byte[] GetByteContents()
        {
            return pdfContents;
        }

        public string GetFileName()
        {
            return fileName;
        }

        public void WriteToFile(string outDir)
        {
            FileUtilities.SaveFile(outDir + GetFileName(), pdfContents);
        }


    }
}

using System;
using System.Collections;
using System.Data;

namespace EmailSender
{
    class Donation
    {
        private const string TEMPLATE_PATH = "templates/donation.template";
        private const string DATE_FORMAT = "MMMM dd, yyyy";

        private int ID;
        private DateTime date;
        private double amount;
        private string checkNumber;
        private string fundName;
        private double fundAmount;

        public Donation(DataRow donationData)
        {
            this.ID = Convert.ToInt32(donationData["Maillist id"]);
            this.date = Convert.ToDateTime(donationData["Date"]);
            this.amount = Convert.ToDouble(donationData["Amount"]);
            this.checkNumber = donationData["Check number"].ToString();
            this.fundName = donationData["fund"].ToString();
            this.fundAmount = Convert.ToDouble(donationData["designate amount for the fund"]);
        }

        public int GetID()
        {
            return ID;
        }

        public double GetAmount()
        {
            return amount;
        }

        public string ToTemplateString()
        {
            Hashtable values = new Hashtable();
            values.Add("date", date.ToString(DATE_FORMAT));
            values.Add("checkNumber", checkNumber);
            values.Add("fundName", fundName);
            values.Add("fundAmount", fundAmount);
            values.Add("amount", amount);
            return FileUtilities.PopulateTemplate(TEMPLATE_PATH, values);
        }

    }
}

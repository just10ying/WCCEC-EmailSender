using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender
{
    class Donation
    {
        public int ID;
        public DateTime date;
        public double amount;
        public string checkNumber;
        public string fundName;
        public double fundAmount;

        public Donation(DataRow donationData)
        {
            this.ID = Convert.ToInt32(donationData["Maillist id"]);
            this.date = Convert.ToDateTime(donationData["Date"]);
            this.amount = Convert.ToDouble(donationData["Amount"]);
            this.checkNumber = donationData["Check number"].ToString();
            this.fundName = donationData["fund"].ToString();
            this.fundAmount = Convert.ToDouble(donationData["designate amount for the fund"]);
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Week_12___Travel_Extravaganza
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        List<SortedList<string, string>> lineItems = new List<SortedList<string, string>> { };
        //Call form and retrieve data from form
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Gets Button Control
            Button btnSender = (Button)sender;
            // Init. frmEdit Form
            Form frmEdit = new frmEdit();
            // Button Control Text Passed As Switch Case
            switch (btnSender.Text)
            {
                case "Add":
                    // Shows frmEdit as Modal
                    frmEdit.ShowDialog();
                    break;
                case "Edit":
                    // Shows frmEdit Only If Item(s) Are In ListBox
                    if (lstItinerary.SelectedIndex != -1)
                    {
                        frmEdit.Tag = lineItems[lstItinerary.SelectedIndex];
                        frmEdit.ShowDialog();
                        lstItinerary.Items.RemoveAt(lstItinerary.SelectedIndex);
                    }
                   
                    break;
                case "Delete":
                    // Shows frmEdit Only If Item(s) Are In ListBox
                    if (lstItinerary.SelectedIndex != -1)
                    {
                        lineItems.RemoveAt(lstItinerary.SelectedIndex);
                        lstItinerary.Items.RemoveAt(lstItinerary.SelectedIndex);
                    }
                    
                    break;
                default:
                    break;
            }


            //Handle if User confirms choice
            if (frmEdit.DialogResult == DialogResult.OK)
            {

                SortedList<string, string> formList = (SortedList<string, string>)frmEdit.Tag;

                string frmData;
                int index;
                index = lstItinerary.SelectedIndex;
                // Sets Index Based On SelectedIndex w/ out of range prevention
                if (lstItinerary.SelectedIndex < 0)
                {
                    index = lstItinerary.Items.Count;
                }
                // Adds Items From frmEdit To ListBox & Formated
                switch (formList["tabName"])
               
                {
                    case "Flights":
                        frmData = formList["tabName"] + " " + formList["date"] + " " + formList["title"] + " " +
                                  formList["origin"] + " " + formList["destination"];
                        lstItinerary.Items.Insert(index,frmData);
                        lineItems.Insert(index,formList);
                        break;
                    case "Hotels":
                        frmData = formList["tabName"] + " " + formList["checkIn"] + " " + formList["hotelName"] + " " +
                                         formList["address"] + " " + formList["guestsNumber"] + " " + formList["checkOut"];
                        lstItinerary.Items.Add(frmData);
                        lineItems.Insert(index, formList);
                        break;
                    case "Events":
                        frmData = formList["tabName"] + " " + formList["date"] + " " + formList["title"] + " " +
                                         formList["details"];
                        lstItinerary.Items.Add(frmData);
                        lineItems.Insert(index, formList);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

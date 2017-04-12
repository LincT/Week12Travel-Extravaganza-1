using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Week_12___Travel_Extravaganza.Properties;
using System.IO;

namespace Week_12___Travel_Extravaganza
{
    public partial class frmEdit : Form
    {
        public frmEdit()
        {
            InitializeComponent();
        }
        bool edited = false;
        string strButtonText;
        SortedList<string, string> tabData = new SortedList<string, string> { };
        // On Load: Collects API, Sets frmEdit Text Based On Tab, Builds Data Controls From SortedList
        private void frmEdit_Load(object sender, EventArgs e)
        {

            var pathWithEnv = @"%USERPROFILE%\Source\Configs\wk12\";
            var apiPath = Environment.ExpandEnvironmentVariables(pathWithEnv);

            string apiToken = "";
            string apiSecret = "";

            StreamReader apiTokenRead = new StreamReader(apiPath + "apiToken.txt");
            apiToken = apiTokenRead.ReadLine();

            StreamReader apiSecretRead = new StreamReader(apiPath + "apiSecret.txt");
            apiSecret = apiSecretRead.ReadLine();


            apiCall.Headers.Add("APC-Auth: " + apiToken);
            apiCall.Headers.Add("APC-Auth-Secret: " + apiSecret);
            lblDestinationAP.Text = "";
            lblOriginAP.Text = "";
            this.Text = "Add/Edit " + tabControl1.SelectedTab.Text;
            SortedList<string, string> editData = (SortedList<string, string>)this.Tag;
            if (this.Tag is SortedList<string, string>)
            {
                switch (editData["tabName"])
                {
                    case "Flights":
                        txtOriginAP.Text = editData["origin"];
                        txtDestinationAP.Text = editData["destination"];
                        dtpFlights.Text = editData["date"];
                        break;
                    case "Hotels":
                        txtHotel.Text = editData["hotelName"];
                        txtAddress.Text = editData["address"];
                        dtpCheckIn.Text = editData["checkIn"];
                        dtpCheckOut.Text = editData["checkOut"];
                        cbGuests.Text = editData["guestsNumber"];
                        break;
                    case "Events":
                        dtpEvents.Text = editData["date"];
                        txtEventTitle.Text = editData["title"];
                        txtEventDetails.Text = editData["details"];
                        break;
                    default:
                        break;
                }
                tabControl1.SelectTab(enumTabs(editData["tabName"]));

            }
        }

        private int enumTabs(string tabName)
        {
            switch (tabName)
            {
                case "Flights":
                    return 0;
                case "Hotels":
                    return 1;
                default:
                    return 2;
            }
        }

        WebClient apiCall = new WebClient();

        private void btnForm_Click(object sender, EventArgs e)
        {
            strButtonText = ((Button)sender).Text;
            Debug.Write(strButtonText + "\n");
            this.Close();
        }

        private void frmEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.Write("Sender:" + sender.ToString() + "\n");
            string selectedTab = tabControl1.SelectedTab.Text;
            switch (selectedTab)
            {
                case "Flights":
                    if (strButtonText == "Okay")
                    {
                        tabData.Add("tabName", selectedTab);
                        tabData.Add("date", dtpFlights.Text);
                        tabData.Add("title", "Flight");
                        tabData.Add("origin", txtOriginAP.Text);
                        tabData.Add("destination", txtDestinationAP.Text);

                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        e.Cancel = unsavedData();
                    }
                    break;
                case "Hotels":
                    if (strButtonText == "Okay")
                    {
                        tabData.Add("tabName", selectedTab);
                        tabData.Add("checkIn", dtpCheckIn.Text);
                        tabData.Add("hotelName", txtHotel.Text);
                        tabData.Add("address", txtAddress.Text);
                        tabData.Add("guestsNumber", cbGuests.Text);
                        tabData.Add("checkOut", dtpCheckOut.Text);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        e.Cancel = unsavedData();
                    }
                    break;
                case "Events":
                    if (strButtonText == "Okay")
                    {
                        tabData.Add("tabName", selectedTab);
                        tabData.Add("date", dtpEvents.Text);
                        tabData.Add("title", txtEventTitle.Text);
                        tabData.Add("details", txtEventDetails.Text);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        e.Cancel = unsavedData();
                    }
                    break;
                default:
                    break;
            }
            if (strButtonText == "Okay")
            {
                this.Tag = tabData;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.Tag = "cancel";
                this.DialogResult = DialogResult.Cancel;
            }
        }
        // Throws Reminder To Save
        private bool unsavedData()
        {
            if (edited)
            {
                DialogResult reply = MessageBox.Show("Exit without Saving", 
                                                     "Exit", MessageBoxButtons.OKCancel, 
                                                     MessageBoxIcon.Exclamation, 
                                                     MessageBoxDefaultButton.Button2);
                if (reply == DialogResult.Cancel)
                {
                    return true;
                }
            }
            return false;
        }
        // Takes ListBox Index Based On SortedList Index --Retirves Data To frmEdit Controls
        private void tabEdited_TextChanged(object sender, EventArgs e)
        {

            edited = true;
            if (sender is TextBox && tabControl1.SelectedTab.Text == "Flights")
            {
                TextBox txtBox = (TextBox)sender;

                txtBox.Text = txtBox.Text.ToUpper();
                txtBox.Select(txtBox.Text.Length, txtBox.Text.Length);

                switch (txtBox.Name)
                {
                    case "txtOriginAP":
                        picOrigin.Visible = false;
                        lblOriginAP.Visible = false;
                        lblOriginCity.Visible = false;
                        if (txtBox.Text.Length == 3)
                        {
                            string source = getAirportInfo(txtBox.Text);
                            string strImage = "";
                            lblOriginAP.Text = source.Split(',')[0];
                            lblOriginAP.Visible = showLabel(lblOriginAP.Text);
                            lblOriginCity.Visible = showLabel(lblOriginAP.Text);
                            picOrigin.Visible = showLabel(lblOriginAP.Text);
                            //Debug.Write(getAirportInfo(txtBox.Text));
                            if (source.Split(':').Count() > 1)
                            {
                                lblOriginCity.Text = source.Split(':')[1];
                                strImage = getAirportContinent(source.Split(':')[2]);

                            }
                            else
                            {
                                lblOriginCity.Text = "";
                            }
                            
                            object contPic = Resources.ResourceManager.GetObject(strImage);
                            
                            picOrigin.Image = (Image)contPic;
                        }
                        break;
                    case "txtDestinationAP":

                        picDestination.Visible = false;
                        lblDestinationAP.Visible = false;
                        lblDestinationCity.Visible = false;

                        if (txtBox.Text.Length == 3)
                        {
                            string source = getAirportInfo(txtBox.Text);
                            string strImage = "";
                            lblDestinationAP.Text = source.Split(',')[0];
                            lblDestinationAP.Visible = showLabel(lblDestinationAP.Text);
                            lblDestinationCity.Visible = showLabel(lblDestinationAP.Text);
                            picDestination.Visible = showLabel(lblDestinationAP.Text);
                            //MessageBox.Show(getAirportInfo(txtBox.Text));
                            if (source.Split(':').Count() > 1)
                            {
                                lblDestinationCity.Text = source.Split(':')[1];
                                strImage = getAirportContinent(source.Split(':')[2]);
                                
                            }
                             
                            //MessageBox.Show(strImage);
                            object contPic = Resources.ResourceManager.GetObject(strImage);
                            
                            picDestination.Image = (Image)contPic;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        // Gets Airport Continent Icon based on Continent
        private string getAirportContinent(string cont)
        {
            switch (cont)
            {
                case "Oceanic":
                    return "aus";
                case "Asia":
                    return "ais";
                case "North America":
                    return "nor";
                case "South America":
                    return "sou";
                case "Europe":
                    return "eur";
                default:
                    return "wor";
            }
        }

        private bool showLabel(string text)
        {
            if (text == "")
            {
                return false;
            }
            return true;
        }
        // Main API Call
        private string getAirportInfo(string strAirPortCode)
        {
            try
            {
                Debug.Write("getAirportInfo Called!");
                StringBuilder str = new StringBuilder();
                if (validAirPortCode(strAirPortCode))
                {
                    SortedList<string, string> tempData = new SortedList<string, string> { };
                    // Main API Call + Raw data
                    string res = apiCall.DownloadString("https://www.air-port-codes.com/api/v1/single?iata=" + 
                        strAirPortCode);
                    // Convert to extended characters if non-ascii
                    var test = JsonConvert.DeserializeObject(res);
                    //MessageBox.Show(test.ToString());
                    // Parsed API Data To List
                    foreach(string line in test.ToString().Split('\n'))
                    {

                        string key = "", val = "",category = ">";
                        int i = 1;
                        
                        // Taking List As Individual Element
                        if (line.Contains(":"))
                        {
                            
                            key = line.Split(':')[0].Replace("\"","").Trim();
                            val = line.Split(':')[1].Replace("\"","").Trim();
                            if (line.Contains('{'))
                            {
                                category = i.ToString("0m");
                            }
                           
                            if (tempData.ContainsKey(key))
                            {
                                i++;
                                key = key + str.Append('.',i);
                                //Debug.Print(key);
                                tempData.Add(key, val);
                            }
                            else
                            {
                                tempData.Add(key, val);
                            }
                        }
                    }
                    // Json return from api can vary depending on whether foreign 
                    //non -ascii characters present. created handling to give each element unique key
                    // and handling to make sure Continent is populated depending on what's returned.
                    string strName = tempData["name"];
                    string strCont = "";
                    if (tempData.ContainsKey("name........"))
                    { strCont = tempData["name........"]; }
                    else if (tempData.ContainsKey("name.........."))
                    { strCont = tempData["name.........."];}
                    else { strCont = "wor"; }
                    string strLocation = tempData["full_location"].Replace(",","");

                    if (strName == "404")
                    {
                        strName = "Invalid Code";
                        strLocation = "";
                    }
                    
                    
                    return strName + ":" + strLocation + ":" + strCont;
                    
                }
                return "";
            }
            // Catches Exception For Absent Key
            catch (KeyNotFoundException ex)
            {
                Debug.Write(ex.Message + "\n");
                return "NoData";
            }
            // Catch All Exception
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        // Takes Text Param To Bool Function validCodeLength
        private bool validAirPortCode(string text)
        {
            return (validCodeLength(text));
        }
        // Function To Return Bool Based On Text Length
        private bool validCodeLength(string text)
        {
            if (text.Length == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Changes frmEdit Text Based On TabPage SelectedIndex
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = "Add/Edit " + tabControl1.SelectedTab.Text;
        }
    }
}

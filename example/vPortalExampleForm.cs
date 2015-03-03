using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using vportal.api.vpapi;

namespace vportal.api
{
    public partial class vPortalExampleForm : Form
    {
        //Create an instance of the vPortal API class
        public vPortalAPI vpApi;

        public vPortalExampleForm()
        {
            InitializeComponent();
        }
        private void vPortalExampleForm_Load(object sender, EventArgs e)
        {
            apiURLtextBox.Text = vportal.api.Properties.Settings.Default.API_URL.ToString();
            apiUSERtextBox.Text = vportal.api.Properties.Settings.Default.username.ToString();
            apiPSWDtextBox.Text = vportal.api.Properties.Settings.Default.password.ToString();
            apiTOKENtextBox.Text = vportal.api.Properties.Settings.Default.token.ToString();

            vpApi = new vPortalAPI("My App", true, apiURLtextBox.Text, apiUSERtextBox.Text, apiPSWDtextBox.Text, apiTOKENtextBox.Text);

            //We have an enumerator in place to return Search Criteria values to populate your combobox
            //you may use your own criteria names, but refer to the vPortal API documentation for the correct integer to pass back
            //to the API's constructor. if this is not correct, then no data will be return (or possibly wrong data because the search criteria may be wrong)
            //Here is an example of populating your combobox with our Search Values:
            this.SearchCriteriaComboBox.DataSource = vpApi.GetSearchCriteriaValues();
            this.SearchCriteriaComboBox.ValueMember = "Key";
            this.SearchCriteriaComboBox.DisplayMember = "Value";
        }

        #region vPortalAPI_Data_GET
        //Get all MASTER records from Visitor Portal
        private void GetNewMasterRecordsButton_Click(object sender, EventArgs e)
        {
            GetNewMasterRecordsResponseTextBox.Text = "Master Record: MASTER_ID, Title, Firstname, Middlename, Lastname, Displayname, " +
                "ID Number, Gender, Current, Site ID, Master Type, Site Master ID, Flag" + Environment.NewLine +
                "     Master Detail: MASTER_DETAIL_ID, Detail Value, Detail Type, MASTER_ID" + Environment.NewLine + Environment.NewLine;

            //checkbox is ticked return true else false
            bool NewRecordsOnly = (NewOnlyCheckBox.Checked) ? true : false;

            //create a string variable = vp.GetMasterData(Visitor Portal URL, username, password, New Records Only or All)<---Username and password must be plain text,
            //you should encrypt this in your own configuration and decrypt when presenting it here
            string GetMasterRecords = vpApi.vPortalGetAllMasterData(NewRecordsOnly);

            //make sure there is actually a return result by checking the string length
            if (GetMasterRecords.Length > 0)
            {
                //for each record in the response, do something with it...
                for (int i = 0; i < GetMasterRecords.ParseJSON<MASTER>().Count(); i++)
                {
                    //Here we are returning the result in comma delimited form for representational purposes, but you can assign each value to whatever you want to.
                    //important to note [i] is the index for each Master record
                    GetNewMasterRecordsResponseTextBox.AppendText(
                        "Master Record: " + Environment.NewLine + "     " + GetMasterRecords.ParseJSON<MASTER>()[i].MASTER_ID.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_TITLE.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_FIRSTNAME.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_MIDDLENAME.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_LASTNAME.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_DISPLAYNAME.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_IDNUMBER.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_GENDER.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_CURRENT.ToString() + ", " +
                     GetMasterRecords.ParseJSON<MASTER>()[i].SITE_ID.ToString() + ", " +
                        //Refer to vPortal API documentation for list of MASTER_TYPE descriptions
                     GetMasterRecords.ParseJSON<MASTER>()[i].MASTER_TYPE_ID.ToString() + ", " +
                        //Refer to vPortal API documentation for SITE_MASTER_ID explanation
                     GetMasterRecords.ParseJSON<MASTER>()[i].SITE_MASTER_ID.ToString() + ", " +
                        //Refer to vPortal API documentation for flags explanation
                     GetMasterRecords.ParseJSON<MASTER>()[i].MST_FLAG.ToString() +
                     Environment.NewLine + "     Master Detail: " + Environment.NewLine);

                    //Check if result has any MASTER_DETAILS data
                    int masterDetailCount = GetMasterRecords.ParseJSON<MASTER>()[i].MASTER_DETAIL.Count();
                    //if we have some, then for each record do something with it...
                    if (masterDetailCount > 0)
                    {
                        for (int ii = 0; ii < GetMasterRecords.ParseJSON<MASTER>()[i].MASTER_DETAIL.Count(); ii++)
                        {
                            //important to note [i] is the index for each Master record and [ii] is the index for each Master Detail record linked to each [i]
                            GetNewMasterRecordsResponseTextBox.AppendText("          " + GetMasterRecords.ParseJSON<MASTER>()[i].MASTER_DETAIL[ii].MASTER_DETAIL_ID.ToString() + ", " +
                                GetMasterRecords.ParseJSON<MASTER>()[i].MASTER_DETAIL[ii].MD_VALSTRING.ToString() + ", " +
                                //Refer to vPortal API documentation for list of MASTER_DETAIL_TYPE descriptions
                                GetMasterRecords.ParseJSON<MASTER>()[i].MASTER_DETAIL[ii].MASTER_DETAIL_TYPE_ID.ToString() + ", " +
                                GetMasterRecords.ParseJSON<MASTER>()[i].MASTER_DETAIL[ii].MASTER_ID.ToString() +
                                Environment.NewLine);
                        }
                    }
                    GetNewMasterRecordsResponseTextBox.AppendText(Environment.NewLine);
                }
            }
        }
               //Get specific MASTER record/s from Visitor Portal based on criteria
        private void GetSpecificMasterRecordButton_Click(object sender, EventArgs e)
        {
            GetSpecificMasterRecordResponseTextBox.Text = "";
            if ((SearchCriteriaTextBox.Text == "") || (SearchCriteriaComboBox.Text == ""))
            {
                MessageBox.Show("Select criteria type and input a value into the Criteria Search text box first.");
            }
            else
            {
                //if using our Search Criteria values, this is how you will get the correct integer value to pass to the API constructor:
                int CriteriaType = vpApi.GetSearchCriteriaType(SearchCriteriaComboBox.Text);

                //create a string variable = vp.GetMasterData(Visitor Portal URL, username, password, New Records Only or All)<---Username and password must be plain text,
                //you should encrypt this in your own configuration and decrypt when presenting it here
                string GetSpecificMasterRecord = vpApi.vPortalGetSpecificMaster(CriteriaType, SearchCriteriaTextBox.Text);

                //make sure there is actually a return result by checking the string length
                if (GetSpecificMasterRecord.Length > 0)
                {
                    //the result may return more than one record, as you may have more than one 'Bob' for example
                    //be sure to filter this in your code
                    for (int i = 0; i < GetSpecificMasterRecord.ParseJSON<MASTER>().Count(); i++)
                    {
                        GetSpecificMasterRecordResponseTextBox.AppendText(
                            "MASTER_ID = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MASTER_ID.ToString() + Environment.NewLine +
                            "Title = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_TITLE.ToString() + Environment.NewLine +
                            "First Name = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_FIRSTNAME.ToString() + Environment.NewLine +
                            "Middle Name = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_MIDDLENAME.ToString() + Environment.NewLine +
                            "Last Name = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_LASTNAME.ToString() + Environment.NewLine +
                            "Display Name = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_DISPLAYNAME.ToString() + Environment.NewLine +
                            "ID Number = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_IDNUMBER.ToString() + Environment.NewLine +
                            "Gender = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_GENDER.ToString() + Environment.NewLine +
                            "Current = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_CURRENT.ToString() + Environment.NewLine +
                            "SITE_ID = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].SITE_ID.ToString() + Environment.NewLine +
                            "MASTER_TYPE_ID = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MASTER_TYPE_ID.ToString() + Environment.NewLine +
                            "SITE_MASTER_ID = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].SITE_MASTER_ID.ToString() + Environment.NewLine +
                            "Flag = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MST_FLAG.ToString() + Environment.NewLine);

                        //Check if result has any MASTER_DETAILS data
                        int masterDetailCount = GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MASTER_DETAIL.Count();
                        //if we have some, then for each record do something with it... 
                        if (masterDetailCount > 0)
                        {
                            for (int ii = 0; ii < GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MASTER_DETAIL.Count(); ii++)
                            {
                                //important to note [i] is the index for each Master record and [ii] is the index for each Master Detail record linked to each [i]
                                GetSpecificMasterRecordResponseTextBox.AppendText(
                                    "MASTER_DETAIL_ID = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MASTER_DETAIL[ii].MASTER_DETAIL_ID.ToString() + Environment.NewLine +
                                    "Detail Value = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MASTER_DETAIL[ii].MD_VALSTRING.ToString() + Environment.NewLine +
                                    //Refer to vPortal API documentation for list of MASTER_DETAIL_TYPE descriptions
                                    "Detail Type = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MASTER_DETAIL[ii].MASTER_DETAIL_TYPE_ID.ToString() + Environment.NewLine +
                                    "MASTER_ID = " + GetSpecificMasterRecord.ParseJSON<MASTER>()[i].MASTER_DETAIL[ii].MASTER_ID.ToString() + Environment.NewLine + Environment.NewLine);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region vPortalAPI_Data_Insert_Update
        private void InsertMasterDataButton_Click(object sender, EventArgs e)
        {
            //Set Gender value
            string gender = (GenderComboBox.Text == "Male") ? "M" : "F";

            //Assign your values to API MASTER class Model
            MASTER am = new MASTER
            {
                MASTER_ID = 0,
                MST_DISPLAYNAME = DisplayNameTextBox.Text,
                MST_TITLE = TitleComboBox.Text,
                MST_FIRSTNAME = FirstNameTextBox.Text,
                MST_MIDDLENAME = MiddleNameTextBox.Text,
                MST_LASTNAME = LastNameTextBox.Text,
                MST_IDNUMBER = IDNumberTextBox.Text,
                MST_GENDER = gender,
                MST_CURRENT = 1,

                //For MASTER_DETAIL data if you need to parse in your code whether you need this or not.
                //if there is no data for MASTER_DETAIL you can leave this part out completely.
                MASTER_DETAIL = new List<MASTERDETAIL>()
                {
                    //For each item (email, mobile, telephone etc.) add a comma then new MASTERDETAIL()...
                    new MASTERDETAIL(){ MD_VALBINARY = null, MASTER_DETAIL_ID = 0, MD_VALSTRING = EmailTextBox.Text, MASTER_DETAIL_TYPE_ID = 24, MASTER_ID = 0 },
                    new MASTERDETAIL(){ MD_VALBINARY = null, MASTER_DETAIL_ID = 0, MD_VALSTRING = CelltextBox.Text, MASTER_DETAIL_TYPE_ID = 23, MASTER_ID = 0 }
                }
            };

            string jsonreturn = vpApi.BuildMasterInsertUpdate(am);
            //Execute Insert data to vPortal API
            string result = vpApi.vPortalInsertMasterData(jsonreturn);

            InsertUpdateMasterDataResponseTextBox.Text = result;
        }

        private void UpdateMasterDataButton_Click(object sender, EventArgs e)
        {
            if (vPortalMASTER_IDtextBox.TextLength > 0)
            {
                //Set Gender value
                string gender = (GenderComboBox.Text == "Male") ? "M" : "F";

                //Assign your values to API MASTER class Model
                MASTER am = new MASTER
                {
                    MASTER_ID = Convert.ToInt32(vPortalMASTER_IDtextBox.Text),
                    MST_DISPLAYNAME = DisplayNameTextBox.Text,
                    MST_TITLE = TitleComboBox.Text,
                    MST_FIRSTNAME = FirstNameTextBox.Text,
                    MST_MIDDLENAME = MiddleNameTextBox.Text,
                    MST_LASTNAME = LastNameTextBox.Text,
                    MST_IDNUMBER = IDNumberTextBox.Text,
                    MST_GENDER = gender,
                    MST_CURRENT = 1,

                    //For MASTER_DETAIL data if you need to parse in your code whether you need this or not.
                    //if there is no data for MASTER_DETAIL you can leave this part out completely.
                    MASTER_DETAIL = new List<MASTERDETAIL>()
                {
                    //For each item (email, mobile, telephone etc.) add a comma then new MASTERDETAIL()...
                    new MASTERDETAIL(){ MD_VALBINARY = null, MASTER_DETAIL_ID = 0, MD_VALSTRING = EmailTextBox.Text, MASTER_DETAIL_TYPE_ID = 24, MASTER_ID = Convert.ToInt32(vPortalMASTER_IDtextBox.Text) },
                    new MASTERDETAIL(){ MD_VALBINARY = null, MASTER_DETAIL_ID = 0, MD_VALSTRING = CelltextBox.Text, MASTER_DETAIL_TYPE_ID = 23, MASTER_ID = Convert.ToInt32(vPortalMASTER_IDtextBox.Text) }
                }
                };

                string jsonreturn = vpApi.BuildMasterInsertUpdate(am);
                //Execute Update data to vPortal API
                string result = vpApi.vPortalUpdateMasterData(jsonreturn);

                InsertUpdateMasterDataResponseTextBox.Text = result;
            }
            else
            {
                MessageBox.Show("To do an update to Visitor Portal you need to know the MASTER_ID, use 'Get Specific Visitor Portal Master Record' tab to reference.");
            }
        }
        #endregion
    }
}

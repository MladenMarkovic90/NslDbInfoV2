using MyLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace NslDbInfoV2
{
    public partial class NslDbForm : Form
    {
        private ReadOnlyCollection<PropertyInfo> orderedPropertiesContacts = new ContactDO().GetOrderedProperties();

        public NslDbForm()
        {
            this.InitializeComponent();
        }

        private void NslDbFormLoad(object sender, EventArgs e)
        {
            this.ReloadContacts();
        }

        private void ReloadContacts()
        {
            this.flowLayoutPanel1.Controls.Clear();
            this.txtSearch.Text = string.Empty;

            string error = string.Empty;
            DataTable data = DataBaseHelper.SelectAll(Program.ConnectionString, ContactDO.TableName, ref error);

            if (string.IsNullOrEmpty(error))
            {
                List<ContactDO> contacts = DataBaseHelper.ConvertDataTable<ContactDO>(data, ref error);

                foreach (ContactDO contact in contacts)
                {
                    this.flowLayoutPanel1.Controls.Add(new ContactControl(contact, this));
                }

                Button newContact = new Button()
                {
                    Text = "New Contact",
                    Width = 300,
                    Height = 315
                };

                newContact.Click += NewContactClick;
                newContact.MouseEnter += FormMouseEnter;

                this.flowLayoutPanel1.Controls.Add(newContact);
            }
            else
            {
                MessageBox.Show("Error: " + error);
            }
        }

        private void NewContactClick(object sender, EventArgs e)
        {
            try
            {
                new ContactDetailView().ShowDialog();
                this.ReloadContacts();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ReloadMenuItemClick(object sender, EventArgs e)
        {
            this.ReloadContacts();
        }

        public void DeleteContactControl(ContactControl control, int recordId)
        {
            if (MessageBox.Show("Are you sure you want to delete this Contact?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string error = string.Empty;

                try
                {
                    DataBaseHelper.Delete<ContactDO>(Program.ConnectionString, ContactDO.TableName, recordId, ref error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while trying to delete a record: " + ex.Message);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    MessageBox.Show(error);
                    return;
                }

                control.Visible = false;
                this.flowLayoutPanel1.Controls.Remove(control);
            }
        }

        private void ImportClick(object sender, EventArgs e)
        {
            try
            {
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string file = this.openFileDialog1.FileName;

                    string error = string.Empty;

                    ReadOnlyCollection<PropertyInfo> collection = new ContactDO().GetOrderedProperties();

                    DataTable data = ExcelHelper.Import(file, collection, ref error);

                    this.ShowErrorIfNotEmpty(error);

                    error = string.Empty;
                    List<ContactDO> contacts = DomainObjectHelper.GetDomainObjectFromDataTable<ContactDO>(data, ref error);

                    this.ShowErrorIfNotEmpty(error);

                    foreach (ContactDO record in contacts)
                    {
                        error = string.Empty;
                        DataBaseHelper.Insert(Program.ConnectionString, ContactDO.TableName, record, ref error);
                        this.ShowErrorIfNotEmpty(error);
                    }

                    this.ReloadContacts();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error while trying to import data: " + ex.Message);
            }
        }

        private void ExportClick(object sender, EventArgs e)
        {
            try
            {
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    List<ContactDO> records = new List<ContactDO>();

                    string fileName = this.saveFileDialog1.FileName;

                    foreach (object control in flowLayoutPanel1.Controls)
                    {
                        if (control is ContactControl)
                        {
                            ContactControl contactControl = control as ContactControl;

                            if (contactControl.Visible)
                            {
                                records.Add(contactControl.GetContactCopy());
                            }
                        }
                    }

                    string error = string.Empty;
                    ExcelHelper.Export(records, fileName, ref error);
                    this.ShowErrorIfNotEmpty(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while trying to export data: " + ex.Message);
            }
        }

        private void SearchClick(object sender, EventArgs e)
        {
            string searchString = this.txtSearch.Text;

            foreach (object control in flowLayoutPanel1.Controls)
            {
                if (control is ContactControl)
                {
                    ContactControl contactControl = control as ContactControl;
                    contactControl.Visible = contactControl.Contains(searchString);
                }
            }
        }

        private void ShowErrorIfNotEmpty(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show(error);
            }
        }

        public void FormMouseEnter(object sender, EventArgs e)
        {
            foreach (object control in flowLayoutPanel1.Controls)
            {
                if (control is ContactControl)
                {
                    ContactControl contactControl = control as ContactControl;
                    contactControl.ContolMouseLeave();
                }
            }
        }
    }
}
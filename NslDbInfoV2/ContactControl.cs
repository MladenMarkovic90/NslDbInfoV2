using System;
using System.Windows.Forms;

namespace NslDbInfoV2
{
    public partial class ContactControl : UserControl
    {
        private ContactDO record;
        private NslDbForm parentForm;

        public ContactControl(ContactDO record, NslDbForm parentForm)
        {
            this.InitializeComponent();
            this.record = record;
            this.parentForm = parentForm;
        }

        public ContactDO GetContactCopy()
        {
            return this.record.Copy();
        }

        public bool Contains(string text)
        {
            return this.record.ToString().Contains(text);
        }

        private void ControlLoad(object sender, EventArgs e)
        {
            this.RefreshData();
        }

        private void RefreshData()
        {
            this.txtIdNumber.Text = this.record.IdNumber;
            this.txtName.Text = this.record.Name;
            this.txtDesignation.Text = this.record.Designation;
            this.datDateOfJoining.Text = this.record.DateOfJoining.ToString();
            this.datDateOfBirth.Text = this.record.DateOfBirth.ToString();
            this.txtBloodGroup.Text = this.record.BloodGroup;
            this.txtNidNumber.Text = this.record.NidNumber;
            this.txtPassPortNumber.Text = this.record.PassPortNumber;
            this.txtCurrentAllocation.Text = this.record.CurrentAllocation;
            this.txtPhoneNumber.Text = this.record.PhoneNumber;
            this.picPhoto.Image = this.record.Photo;
            this.picPhoto.Visible = (this.picPhoto.Image != null);
        }

        private void EditClick(object sender, EventArgs e)
        {
            try
            {
                new ContactDetailView(this.record).ShowDialog();
                this.RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error happened while editing contact: " + ex.Message);
            }
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            this.parentForm.DeleteContactControl(this, (int)this.record.Id);
        }

        private void ControlMouseEnter(object sender, EventArgs e)
        {
            this.parentForm.FormMouseEnter(sender, e);
            this.picPhoto.Visible = false;
        }

        public void ContolMouseLeave()
        {
            this.picPhoto.Visible = (this.picPhoto.Image != null);
        }
    }
}
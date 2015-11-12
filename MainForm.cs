using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;
using System.DirectoryServices;
using System.Threading;
using System.Configuration;

namespace PasswordJockey
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void OkButton_Click(object sender, EventArgs e)
        {
            string CurrentPassword = Password.Text;
            DomainPolicy dp = new DomainPolicy();
            if (dp.MinPasswordAge != 0)
            {
                MessageBox.Show("This program only works with a min password age of 0");
                return;
            }
                
            if (CurrentPassword.Length == 0)
            {
                MessageBox.Show("You need to suppy a password");
                return;
            }
            OkButton.Enabled = false;
            string user = WindowsIdentity.GetCurrent().Name.ToString().Replace("\\","/");
            DirectoryEntry entry;
            entry = new DirectoryEntry(@"WinNT://" + user + ",User");

            string previousPass = CurrentPassword;
            string newPass = null;
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            int loop_required = dp.PasswordHistoryLength+1;
            try
            {
                for (int i = 0; i < loop_required; i++)
                {

                    newPass = "a" + CurrentPassword + Convert.ToString(i);
                    changePassword(entry, previousPass, newPass);
                    
                    previousPass = newPass;
                    progressBar1.Value = (i + 1) * loop_required;
                }
                changePassword(entry, previousPass, CurrentPassword);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                
                Exception cause = ex.InnerException;
                MessageBox.Show("Opps " + cause.Message);
                progressBar1.Visible = false;
                OkButton.Enabled = true;
                
                return;
            }
            
            progressBar1.Value = 100;
            label1.Text = "You password has been changed";
        }

        private void changePassword(DirectoryEntry entry, string oldPass, string newPass)
        {
               //entry.Invoke("ChangePassword", oldPass, newPass);
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptForCredentials
{
    public partial class CredentialPromptForm : Form
    {
        #region Constructors

        public CredentialPromptForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        public string Username { get; private set; }

        public string Password { get; private set; }

        #endregion

        #region Public Methods

        public bool Get()
        {
            var results = (ShowDialog() == DialogResult.OK);

            if (results)
            {
                Username = UsernameText.Text.Trim();
                Password = PasswordText.Text.Trim();
            }

            return results;
        }

        #endregion

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

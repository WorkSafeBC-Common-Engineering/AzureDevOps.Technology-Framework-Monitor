using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using ScannerCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptForCredentials
{
    public class Credentials : ICredentials
    {
        #region ICredentials Implementation

        VssCredentials ICredentials.Get()
        {
            var form = new CredentialPromptForm();
            if (form.Get())
            {
                return new VssAadCredential(form.Username, form.Password);
            }
            else
            {
                throw new ApplicationException("Prompt For Credentials - user cancelled!");
            }
        }

        #endregion
    }
}

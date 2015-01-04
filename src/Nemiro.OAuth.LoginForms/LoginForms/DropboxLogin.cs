// ----------------------------------------------------------------------------
// Copyright (c) Aleksey Nemiro, 2015. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Nemiro.OAuth.Clients;

namespace Nemiro.OAuth.LoginForms
{

  public class DropboxLogin : Login, ILoginForm
  {

    public DropboxLogin(string clientId, string clientSecret) : this(clientId, clientSecret, null) { }

    public DropboxLogin(string clientId, string clientSecret, string scope) : this(new DropboxClient(clientId, clientSecret) { Scope = scope }) { }

    public DropboxLogin(DropboxClient client) : base(client) 
    {
      this.Width = 695;
      this.Height = 515;
      this.Icon = Properties.Resources.dropbox;
    }

    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      // waiting for results
      if (url.ToString().Equals("about:blank", StringComparison.OrdinalIgnoreCase))
      {
        // the user has refused to give permission 
        this.Close();
      }
      else
      {
        if (webBrowser.Document.GetElementById("auth-code") != null)
        {
          // found authorization code
          base.GetAccessToken(webBrowser.Document.GetElementById("auth-code").InnerText);
        }
      }
    }

  }

}
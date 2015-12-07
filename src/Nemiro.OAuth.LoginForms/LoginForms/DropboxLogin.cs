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
using Nemiro.OAuth.Clients;

namespace Nemiro.OAuth.LoginForms
{

  public class DropboxLogin : Login, ILoginForm
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>App key</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="clientSecret">The <b>App secret</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public DropboxLogin(string clientId, string clientSecret, bool autoLogout = false) : this(clientId, clientSecret, null, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>App key</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="clientSecret">The <b>App secret</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="scope">The access scope.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public DropboxLogin(string clientId, string clientSecret, string scope, bool autoLogout = false) : this(new DropboxClient(clientId, clientSecret) { Scope = scope }, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public DropboxLogin(DropboxClient client, bool autoLogout = false) : base(client, autoLogout) 
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
        if (webBrowser.Document.GetElementById("auth-code-input") != null)
        {
          // set authorization code
          base.AuthorizationCode = webBrowser.Document.GetElementById("auth-code-input").GetAttribute("value");
        }
      }
    }

  }

}
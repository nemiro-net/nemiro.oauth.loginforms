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
using Nemiro.OAuth.Clients;
using System.Collections.Specialized;

namespace Nemiro.OAuth.LoginForms
{

  public class GoogleLogin : Login, ILoginForm
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Client ID obtained from the <see href="https://console.developers.google.com/">Google Developers Console</see>.</param>
    /// <param name="clientSecret">The Client Secret obtained from the <see href="https://console.developers.google.com/">Google Developers Console</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public GoogleLogin(string clientId, string clientSecret, bool autoLogout = false) : this(clientId, clientSecret, null, autoLogout) { }
    
    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Client ID obtained from the <see href="https://console.developers.google.com/">Google Developers Console</see>.</param>
    /// <param name="clientSecret">The Client Secret obtained from the <see href="https://console.developers.google.com/">Google Developers Console</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    public GoogleLogin(string clientId, string clientSecret, string scope, bool autoLogout = false) : this(new GoogleClient(clientId, clientSecret) { Scope = scope }, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public GoogleLogin(GoogleClient client, bool autoLogout = false) : base(client, autoLogout) 
    {
      this.Width = 710;
      this.Height = 560;
      this.Icon = Properties.Resources.google;
    }

    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      // cancel button click handler
      if (webBrowser.Document.GetElementById("submit_deny_access") != null)
      {
        webBrowser.Document.GetElementById("submit_deny_access").Click += (sender, e) =>
        {
          this.Close();
        };
      }
      // has code
      if (webBrowser.Document.GetElementById("code") != null)
      {
        // found authorization code
        base.GetAccessToken(webBrowser.Document.GetElementById("code").GetAttribute("value"));
      }
    }

  }

}

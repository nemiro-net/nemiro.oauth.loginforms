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

  public class FacebookLogin : Login
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The App ID obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="clientSecret">The App Secret obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public FacebookLogin(string clientId, string clientSecret, bool autoLogout = false) : this(clientId, clientSecret, null, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The App ID obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="clientSecret">The App Secret obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    public FacebookLogin(string clientId, string clientSecret, string scope, bool autoLogout = false) : this(new FacebookClient(clientId, clientSecret) { Scope = scope, Parameters = new NameValueCollection { { "display", "popup" } } }, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public FacebookLogin(FacebookClient client, bool autoLogout = false) : base(client, autoLogout) 
    { 
      this.Icon = Properties.Resources.facebook;
    }

    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      this.Close();
    }
    
    public override void Logout()
    {
      base.SetUrl
      (
        "https://www.facebook.com/logout.php",
        (object sender, WebBrowserCallbackEventArgs e) =>
        {
          base.KillCookies();
          // goto auth
          if (this.CanLogin)
          {
            base.SetUrl(this.AuthorizationUrl);
          }
          else
          {
            base.GetAccessToken();
          }
        }
      );
    }

  }

}

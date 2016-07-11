// ----------------------------------------------------------------------------
// Copyright © Aleksey Nemiro, 2015. All rights reserved.
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
using Nemiro.OAuth.Clients;

namespace Nemiro.OAuth.LoginForms
{

  public class AmazonLogin : Login, ILoginForm
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The client ID obtained from the <see href="http://login.amazon.com/manageApps">App Console</see>.</param>
    /// <param name="clientSecret">The client secret obtained from the <see href="http://login.amazon.com/manageApps">App Console</see>.</param>
    /// <param name="returnUrl">The address to return.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public AmazonLogin(string clientId, string clientSecret, string returnUrl, bool autoLogout = false) : this(clientId, clientSecret, returnUrl, null, autoLogout) { }
    
    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The client ID obtained from the <see href="http://login.amazon.com/manageApps">App Console</see>.</param>
    /// <param name="clientSecret">The client secret obtained from the <see href="http://login.amazon.com/manageApps">App Console</see>.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="returnUrl">The address to return.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public AmazonLogin(string clientId, string clientSecret, string returnUrl, string scope, bool autoLogout = false) : this(new AmazonClient(clientId, clientSecret) { ReturnUrl = returnUrl, Scope = scope }, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public AmazonLogin(AmazonClient client, bool autoLogout = false) : base(client, autoLogout) 
    {
      this.Width = 785;
      this.Height = 575;
      this.Icon = Properties.Resources.amazon;
    }
    
    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      // cancel button click handler
      if (webBrowser.Document.GetElementsByTagName("button").GetElementsByName("acknowledgementDenied").Count > 0)
      {
        webBrowser.Document.GetElementsByTagName("button").GetElementsByName("acknowledgementDenied")[0].Click += (sender, e) =>
        {
          this.Close();
        };
      }
    }

  }

}

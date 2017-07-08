// ----------------------------------------------------------------------------
// Copyright © Aleksey Nemiro, 2015-2017. All rights reserved.
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

  /// <summary>
  /// Represents login form for Amazon.
  /// </summary>
  public class AmazonLogin : Login, ILoginForm
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The client ID obtained from the <see href="http://login.amazon.com/manageApps">App Console</see>.</param>
    /// <param name="clientSecret">The client secret obtained from the <see href="http://login.amazon.com/manageApps">App Console</see>.</param>
    /// <param name="returnUrl">The address to return.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public AmazonLogin(string clientId, string clientSecret, string returnUrl, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(clientId, clientSecret, returnUrl, null, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The client ID obtained from the <see href="http://login.amazon.com/manageApps">App Console</see>.</param>
    /// <param name="clientSecret">The client secret obtained from the <see href="http://login.amazon.com/manageApps">App Console</see>.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="returnUrl">The address to return.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public AmazonLogin(string clientId, string clientSecret, string returnUrl, string scope, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(new AmazonClient(clientId, clientSecret) { ReturnUrl = returnUrl, Scope = scope }, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public AmazonLogin(AmazonClient client, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : base(client, autoLogout, loadUserInfo, responseType) 
    {
      this.Width = 785;
      this.Height = 575;
      this.Icon = Properties.Resources.amazon;
    }

    /// <summary>
    /// Handler of event to receive notification when the document finishes loading. 
    /// </summary>
    /// <param name="webBrowser">The <see cref="System.Windows.Forms.WebBrowser"/> instance.</param>
    /// <param name="url">The loaded url.</param>
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

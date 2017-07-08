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
using System.Collections.Specialized;

namespace Nemiro.OAuth.LoginForms
{

  /// <summary>
  /// Represents login form for Facebook.
  /// </summary>
  public class FacebookLogin : Login
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The App ID obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="clientSecret">The App Secret obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public FacebookLogin(string clientId, string clientSecret, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(clientId, clientSecret, null, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The App ID obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="clientSecret">The App Secret obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public FacebookLogin(string clientId, string clientSecret, string scope, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(new FacebookClient(clientId, clientSecret) { Scope = scope, Parameters = new NameValueCollection { { "display", "popup" } } }, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The App ID obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="clientSecret">The App Secret obtained from the <see href="https://developers.facebook.com/apps/">Facebook Developers</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="returnUrl">The address to return. Default: <see href="https://www.facebook.com/connect/login_success.html"/>.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public FacebookLogin(string clientId, string clientSecret, string returnUrl, string scope, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(new FacebookClient(clientId, clientSecret) { ReturnUrl = returnUrl, Scope = scope, Parameters = new NameValueCollection { { "display", "popup" } } }, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public FacebookLogin(FacebookClient client, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : base(client, autoLogout, loadUserInfo, responseType) 
    { 
      this.Icon = Properties.Resources.facebook;
    }

    /// <summary>
    /// Handler of event to receive notification when the document finishes loading. 
    /// </summary>
    /// <param name="webBrowser">The <see cref="System.Windows.Forms.WebBrowser"/> instance.</param>
    /// <param name="url">The loaded url.</param>
    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      this.Close();
    }

    /// <summary>
    /// Logout.
    /// </summary>
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

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
using Nemiro.OAuth.Clients;

namespace Nemiro.OAuth.LoginForms
{

  /// <summary>
  /// Represents login form for Instagram.
  /// </summary>
  public class InstagramLogin : Login
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>Client ID</b> obtained from the <see href="http://instagram.com/developer/clients/manage/">Instagram Manage Clients</see>.</param>
    /// <param name="clientSecret">The <b>Client Secret</b> obtained from the <see href="http://instagram.com/developer/clients/manage/">Instagram Manage Clients</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="returnUrl">The address to return.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public InstagramLogin(string clientId, string clientSecret, string returnUrl, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(clientId, clientSecret, returnUrl, null, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>Client ID</b> obtained from the <see href="http://instagram.com/developer/clients/manage/">Instagram Manage Clients</see>.</param>
    /// <param name="clientSecret">The <b>Client Secret</b> obtained from the <see href="http://instagram.com/developer/clients/manage/">Instagram Manage Clients</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="returnUrl">The address to return.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public InstagramLogin(string clientId, string clientSecret, string returnUrl, string scope, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(new InstagramClient(clientId, clientSecret) { ReturnUrl = returnUrl, Scope = scope }, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public InstagramLogin(InstagramClient client, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : base(client, autoLogout, loadUserInfo, responseType) 
    {
      this.Icon = Properties.Resources.instagram;
    }

    /// <summary>
    /// Logout.
    /// </summary>
    public override void Logout()
    {
      base.SetUrl
      (
        "https://www.instagram.com/accounts/logout/",
        (object sender, WebBrowserCallbackEventArgs e) =>
        {
          this.CanLogout = false;

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
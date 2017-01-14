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
  /// Represents login form for Yahoo.
  /// </summary>
  public class YahooLogin : Login
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>Consumer Key</b> obtained from the <see href="https://developer.apps.yahoo.com/projects">Yahoo Developer Dashboard</see>.</param>
    /// <param name="clientSecret">The <b>Consumer Secret</b> obtained from the <see href="https://developer.apps.yahoo.com/projects">Yahoo Developer Dashboard</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="returnUrl">The address to return.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public YahooLogin(string clientId, string clientSecret, string returnUrl, bool autoLogout = false, bool loadUserInfo = false) : this(clientId, clientSecret, returnUrl, null, autoLogout, loadUserInfo) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>Consumer Key</b> obtained from the <see href="https://developer.apps.yahoo.com/projects">Yahoo Developer Dashboard</see>.</param>
    /// <param name="clientSecret">The <b>Consumer Secret</b> obtained from the <see href="https://developer.apps.yahoo.com/projects">Yahoo Developer Dashboard</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="returnUrl">The address to return.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public YahooLogin(string clientId, string clientSecret, string returnUrl, string scope, bool autoLogout = false, bool loadUserInfo = false) : this(new YahooClient(clientId, clientSecret) { ReturnUrl = returnUrl, Scope = scope }, autoLogout, loadUserInfo) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public YahooLogin(YahooClient client, bool autoLogout = false, bool loadUserInfo = false) : base(client, autoLogout, loadUserInfo) 
    {
      this.Width = 815;
      this.Height = 642;
      this.Icon = Properties.Resources.yahoo;
    }

  }

}

// ----------------------------------------------------------------------------
// Copyright © Aleksey Nemiro, 2015, 2017. All rights reserved.
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
  /// Represents login form for Dropbox.
  /// </summary>
  public class DropboxLogin : Login, ILoginForm
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>App key</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="clientSecret">The <b>App secret</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    [Obsolete("Please use an overloads with a return url.", true)]
    public DropboxLogin(string clientId, string clientSecret, bool autoLogout = false, bool loadUserInfo = false) : this(clientId, clientSecret, null, null, autoLogout, loadUserInfo) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>App key</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="clientSecret">The <b>App secret</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="scope">The access scope.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="obsolete">Do not use this parameter. It is added to avoid conflicts with overloading methods.</param>
    [Obsolete("Please use an overloads with a return url.", true)]
    public DropboxLogin(string clientId, string clientSecret, string scope, bool autoLogout = false, bool loadUserInfo = false, bool obsolete = true) : this(new DropboxClient(clientId, clientSecret) { Scope = scope }, autoLogout, loadUserInfo) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>App key</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="clientSecret">The <b>App secret</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="returnUrl">The address to return. You can use <see href="https://oauthproxy.nemiro.net"/>, but for reliability use your own server that you control.</param>
    public DropboxLogin(string clientId, string clientSecret, string returnUrl, bool autoLogout = false, bool loadUserInfo = false) : this(clientId, clientSecret, returnUrl, null, autoLogout, loadUserInfo, ResponseType.Token) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>App key</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="clientSecret">The <b>App secret</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="returnUrl">The address to return. You can use <see href="https://oauthproxy.nemiro.net"/>, but for reliability use your own server that you control.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public DropboxLogin(string clientId, string clientSecret, string returnUrl, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(clientId, clientSecret, returnUrl, null, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>App key</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="clientSecret">The <b>App secret</b> obtained from the <see href="https://www.dropbox.com/developers/apps">Dropbox App Console</see>.</param>
    /// <param name="scope">The access scope.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="returnUrl">The address to return. You can use <see href="https://oauthproxy.nemiro.net"/>, but for reliability use your own server that you control.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public DropboxLogin(string clientId, string clientSecret, string returnUrl, string scope, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this(new DropboxClient(clientId, clientSecret) { ReturnUrl = returnUrl, Scope = scope }, autoLogout, loadUserInfo, responseType) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>.</param>
    public DropboxLogin(DropboxClient client, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : base(client, autoLogout, loadUserInfo, responseType) 
    {
      this.Width = 695;
      this.Height = 515;
      this.Icon = Properties.Resources.dropbox;
    }
    
    /// <summary>
    /// Handler of event to receive notification when the document finishes loading. 
    /// </summary>
    /// <param name="webBrowser">The <see cref="System.Windows.Forms.WebBrowser"/> instance.</param>
    /// <param name="url">The loaded url.</param>
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

    private bool IsLogout = false;

    /// <summary>
    /// Logout.
    /// </summary>
    public override void Logout()
    {
      base.SetUrl
      (
        "https://www.dropbox.com/logout",
        (object sender, WebBrowserCallbackEventArgs e) =>
        {
          if (!this.IsLogout)
          {
            var webBrowser = (WebBrowser)sender;

            this.IsLogout = (webBrowser.Url.PathAndQuery.IndexOf("/login", 0, StringComparison.OrdinalIgnoreCase) != -1);
          
            if (!this.IsLogout)
            {
              return;
            }
          }

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
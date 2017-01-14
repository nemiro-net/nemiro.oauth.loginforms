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
using System.Windows.Forms;

namespace Nemiro.OAuth.LoginForms
{

  /// <summary>
  /// Represents login form for GitHub.
  /// </summary>
  public class GitHubLogin : Login
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Client ID obtained from the <see href="https://github.com/settings/applications">GitHub Applications</see>.</param>
    /// <param name="clientSecret">The Client Secret obtained from the <see href="https://github.com/settings/applications">GitHub Applications</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public GitHubLogin(string clientId, string clientSecret, bool autoLogout = false, bool loadUserInfo = false) : this(clientId, clientSecret, null, autoLogout, loadUserInfo) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Client ID obtained from the <see href="https://github.com/settings/applications">GitHub Applications</see>.</param>
    /// <param name="clientSecret">The Client Secret obtained from the <see href="https://github.com/settings/applications">GitHub Applications</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public GitHubLogin(string clientId, string clientSecret, string scope, bool autoLogout = false, bool loadUserInfo = false) : this(new GitHubClient(clientId, clientSecret) { Scope = scope }, autoLogout, loadUserInfo) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public GitHubLogin(GitHubClient client, bool autoLogout = false, bool loadUserInfo = false) : base(client, autoLogout, loadUserInfo) 
    {
      this.Width = 590;
      this.Height = 660;
      this.Icon = Properties.Resources.github;
    }

    private bool IsLogout = false;

    public override void Logout()
    {
      base.SetUrl
      (
        "https://github.com/",
        (object sender, WebBrowserCallbackEventArgs e) =>
        {
          if (!this.IsLogout)
          {
            var webBrowser = (WebBrowser)sender;
            if (webBrowser.Document != null && webBrowser.Document.Forms.Count > 0)
            {
              this.IsLogout = true;

              foreach (HtmlElement f in webBrowser.Document.Forms)
              {
                if (f.GetAttribute("class") == null)
                {
                  continue;
                }

                if (f.GetAttribute("action").Contains("/logout"))
                {
                  f.InvokeMember("submit");
                  return;
                }
              }
            }
          }

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

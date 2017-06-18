// ----------------------------------------------------------------------------
// Copyright © Aleksey Nemiro, 2015-2016. All rights reserved.
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
  /// Represents login form for Tumblr.
  /// </summary>
  public class TumblrLogin : Login, ILoginForm
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="consumerKey">The <b>Consumer Key</b> obtained from the <see href="https://www.tumblr.com/oauth/apps">Tumblr Dashboard</see>.</param>
    /// <param name="consumerSecret">The <b>Consumer Secret</b> obtained from the <see href="https://www.tumblr.com/oauth/apps">Tumblr Dashboard</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public TumblrLogin(string consumerKey, string consumerSecret, bool autoLogout = false, bool loadUserInfo = false) : this(new TumblrClient(consumerKey, consumerSecret), autoLogout, loadUserInfo) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public TumblrLogin(TumblrClient client, bool autoLogout = false, bool loadUserInfo = false) : base(client, autoLogout, loadUserInfo) 
    {
      this.Width = 845;
      this.Height = 540;
      this.Icon = Properties.Resources.tumblr;
    }
    
    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      // cancel button click handler
      if (webBrowser.Document.GetElementsByTagName("button").GetElementsByName("deny").Count > 0)
      {
        webBrowser.Document.GetElementsByTagName("button").GetElementsByName("deny")[0].Click += (sender, e) =>
        {
          this.Close();
        };
      }
    }

    /// <summary>
    /// Logout.
    /// </summary>
    public override void Logout()
    {
      base.SetUrl
      (
        "https://www.tumblr.com/logout",
        (object sender, WebBrowserCallbackEventArgs e) =>
        {
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

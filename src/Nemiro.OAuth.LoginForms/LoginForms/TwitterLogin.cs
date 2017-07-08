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
  /// Represents login form for Twitter.
  /// </summary>
  public class TwitterLogin : Login, ILoginForm
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="consumerKey">The API Key obtained from the <see href="https://apps.twitter.com">Twitter Application Management</see>.</param>
    /// <param name="consumerSecret">The API Secret obtained from the <see href="https://apps.twitter.com">Twitter Application Management</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public TwitterLogin(string consumerKey, string consumerSecret, bool autoLogout = false, bool loadUserInfo = false) : this(new TwitterClient(consumerKey, consumerSecret), autoLogout, loadUserInfo) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    public TwitterLogin(TwitterClient client, bool autoLogout = false, bool loadUserInfo = false) : base(client, autoLogout, loadUserInfo) 
    {
      this.Width = 730;
      this.Height = 570;
      this.Icon = Properties.Resources.twitter;
    }

    /// <summary>
    /// Handler of event to receive notification when the document finishes loading. 
    /// </summary>
    /// <param name="webBrowser">The <see cref="System.Windows.Forms.WebBrowser"/> instance.</param>
    /// <param name="url">The loaded url.</param>
    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      if (url.ToString().Equals("https://api.twitter.com/oauth/authorize", StringComparison.OrdinalIgnoreCase))
      {
        if (webBrowser.Document.GetElementsByTagName("code").Count > 0)
        {
          // found authorization code
          base.GetAccessToken(webBrowser.Document.GetElementsByTagName("code")[0].InnerText);
        }
        else
        {
          // the user has refused to give permission 
          this.Close();
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
        "https://twitter.com/",
        (object sender, WebBrowserCallbackEventArgs e) =>
        {
          if (!this.IsLogout)
          {
            this.IsLogout = true;
            var webBrowser = (WebBrowser)sender;
            if (webBrowser.Document != null && webBrowser.Document.GetElementById("signout-form") != null)
            {
              webBrowser.Document.GetElementById("signout-form").InvokeMember("submit");
              return;
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

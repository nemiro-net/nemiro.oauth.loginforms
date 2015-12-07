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
using System.Windows.Forms;

namespace Nemiro.OAuth.LoginForms
{

  public class YandexLogin : Login
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Application ID.</param>
    /// <param name="clientSecret">The Application Password.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public YandexLogin(string clientId, string clientSecret, bool autoLogout = false) : this(clientId, clientSecret, null, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Application ID.</param>
    /// <param name="clientSecret">The Application Password.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    public YandexLogin(string clientId, string clientSecret, string scope, bool autoLogout = false) : this(new YandexClient(clientId, clientSecret) { Scope = scope }, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public YandexLogin(YandexClient client, bool autoLogout = false) : base(client, autoLogout) 
    {
      this.Width = 1000;
      this.Height = 600;
      this.Icon = Properties.Resources.yandex;
    }

    private bool IsLogout = false;
    public override void Logout()
    {
      base.SetUrl
      (
        "https://www.yandex.ru/",
        (object sender, WebBrowserCallbackEventArgs e) =>
        {
          if (!this.IsLogout)
          {
            this.IsLogout = true;
            var webBrowser = (WebBrowser)sender;
            if (webBrowser.Document != null)
            {
              foreach (HtmlElement link in webBrowser.Document.Links)
              {
                if (!String.IsNullOrEmpty(link.GetAttribute("href")) && link.GetAttribute("href").Contains("mode=logout"))
                {
                  link.InvokeMember("click");
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

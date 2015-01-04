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

namespace Nemiro.OAuth.LoginForms
{

  public class TwitterLogin : Login, ILoginForm
  {

    public TwitterLogin(string consumerKey, string consumerSecret) : this(new TwitterClient(consumerKey, consumerSecret)) { }

    public TwitterLogin(TwitterClient client) : base(client) 
    {
      this.Width = 730;
      this.Height = 570;
      this.Icon = Properties.Resources.twitter;
    }

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

  }

}

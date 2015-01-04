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

namespace Nemiro.OAuth.LoginForms
{

  public class GoogleLogin : Login, ILoginForm
  {

    public GoogleLogin(string clientId, string clientSecret) : this(clientId, clientSecret, null) { }

    public GoogleLogin(string clientId, string clientSecret, string scope) : this(new GoogleClient(clientId, clientSecret) { Scope = scope }) { }

    public GoogleLogin(GoogleClient client) : base(client) 
    {
      this.Width = 710;
      this.Height = 560;
      this.Icon = Properties.Resources.google;
    }

    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      // cancel button click handler
      if (webBrowser.Document.GetElementById("submit_deny_access") != null)
      {
        webBrowser.Document.GetElementById("submit_deny_access").Click += (sender, e) =>
        {
          this.Close();
        };
      }
      // has code
      if (webBrowser.Document.GetElementById("code") != null)
      {
        // found authorization code
        base.GetAccessToken(webBrowser.Document.GetElementById("code").GetAttribute("value"));
      }
    }

  }

}

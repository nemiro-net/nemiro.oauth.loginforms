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

  public class LiveLogin : Login, ILoginForm
  {

    public LiveLogin(string clientId, string clientSecret, string returnUrl) : this(clientId, clientSecret, returnUrl, null) { }

    public LiveLogin(string clientId, string clientSecret, string returnUrl, string scope) : this(new LiveClient(clientId, clientSecret) { ReturnUrl = returnUrl, Scope = scope }) { }

    public LiveLogin(LiveClient client) : base(client) 
    {
      this.Width = 700;
      this.Height = 650;
      this.Icon = Properties.Resources.live;
    }

    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      // cancel button click handler
      if (webBrowser.Document.GetElementById("idBtn_Deny") != null)
      {
        webBrowser.Document.GetElementById("idBtn_Deny").Click += (sender, e) =>
        {
          this.Close();
        };
      }
    }

  }

}

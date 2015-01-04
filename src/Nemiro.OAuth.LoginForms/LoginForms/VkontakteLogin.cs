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

  public class VkontakteLogin : Login, ILoginForm
  {

    public VkontakteLogin(string clientId, string clientSecret) : this(clientId, clientSecret, null) { }

    public VkontakteLogin(string clientId, string clientSecret, string scope) : this(new VkontakteClient(clientId, clientSecret) { Scope = scope, Parameters = new NameValueCollection { { "display", "popup" } } }) { }

    public VkontakteLogin(VkontakteClient client) : base(client) 
    {
      this.Icon = Properties.Resources.vkontakte;
    }
    
    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      if (url.Fragment.IndexOf("code=") != -1)
      {
        // is result
        var v = UniValue.ParseParameters(url.Fragment.Substring(1));
        this.GetAccessToken(v["code"].ToString());
      }
    }

  }

}

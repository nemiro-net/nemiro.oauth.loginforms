// ----------------------------------------------------------------------------
// Copyright © Aleksey Nemiro, 2015. All rights reserved.
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
using System.Collections.Specialized;

namespace Nemiro.OAuth.LoginForms
{

  public class VkontakteLogin : Login, ILoginForm
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Application ID obtained from the <see href="http://vk.com/dev">VK App development</see>.</param>
    /// <param name="clientSecret">The Secure Key obtained from the <see href="http://vk.com/dev">VK App development</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public VkontakteLogin(string clientId, string clientSecret, bool autoLogout = false) : this(clientId, clientSecret, null, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Application ID obtained from the <see href="http://vk.com/dev">VK App development</see>.</param>
    /// <param name="clientSecret">The Secure Key obtained from the <see href="http://vk.com/dev">VK App development</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    public VkontakteLogin(string clientId, string clientSecret, string scope, bool autoLogout = false) : this(new VkontakteClient(clientId, clientSecret) { Scope = scope, Parameters = new NameValueCollection { { "display", "popup" } } }, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public VkontakteLogin(VkontakteClient client, bool autoLogout = false) : base(client, autoLogout) 
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

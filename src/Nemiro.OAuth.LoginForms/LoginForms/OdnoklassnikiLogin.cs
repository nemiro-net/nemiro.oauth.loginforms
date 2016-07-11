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
using Nemiro.OAuth.Clients;

namespace Nemiro.OAuth.LoginForms
{

  public class OdnoklassnikiLogin : Login
  {

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Client ID.</param>
    /// <param name="clientSecret">The Client Secret.</param>
    /// <param name="publickKey">The Public Key.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="returnUrl">The address to return.</param>
    public OdnoklassnikiLogin(string clientId, string clientSecret, string publickKey, string returnUrl, bool autoLogout = false) : this(clientId, clientSecret, publickKey, returnUrl, null, autoLogout) { }
    
    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The Client ID.</param>
    /// <param name="clientSecret">The Client Secret.</param>
    /// <param name="publickKey">The Public Key.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="returnUrl">The address to return.</param>
    public OdnoklassnikiLogin(string clientId, string clientSecret, string publickKey, string returnUrl, string scope, bool autoLogout = false) : this(new OdnoklassnikiClient(clientId, clientSecret, publickKey) { ReturnUrl = returnUrl, Scope = scope }, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public OdnoklassnikiLogin(OdnoklassnikiClient client, bool autoLogout = false) : base(client, autoLogout) 
    {
      this.Icon = Properties.Resources.odnoklassniki;
    }

  }

}

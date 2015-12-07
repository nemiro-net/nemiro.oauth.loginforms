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

  public class FoursquareLogin : Login
  {

    /// <summary>
    /// Gets logout url.
    /// </summary>
    public string LogoutUrl
    {
      get
      {
        return "https://foursquare.com/logout";
      }
    }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>Client ID</b> obtained from the <see href="https://foursquare.com/oauth">Foursquare Apps</see>.</param>
    /// <param name="clientSecret">The <b>Client Secret</b> obtained from the <see href="https://foursquare.com/oauth">Foursquare Apps</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="returnUrl">The address to return.</param>
    public FoursquareLogin(string clientId, string clientSecret, string returnUrl, bool autoLogout = false) : this(clientId, clientSecret, returnUrl, null, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified parameters.
    /// </summary>
    /// <param name="clientId">The <b>Client ID</b> obtained from the <see href="https://foursquare.com/oauth">Foursquare Apps</see>.</param>
    /// <param name="clientSecret">The <b>Client Secret</b> obtained from the <see href="https://foursquare.com/oauth">Foursquare Apps</see>.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    /// <param name="scope">The scope of the access request.</param>
    /// <param name="returnUrl">The address to return.</param>
    public FoursquareLogin(string clientId, string clientSecret, string returnUrl, string scope, bool autoLogout = false) : this(new FoursquareClient(clientId, clientSecret) { ReturnUrl = returnUrl, Scope = scope, Parameters = { { "display", "touch" } } }, autoLogout) { }

    /// <summary>
    /// Initializes a new instance of the login form with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public FoursquareLogin(FoursquareClient client, bool autoLogout = false) : base(client, autoLogout) 
    {
      this.Height = 555;
      this.Icon = Properties.Resources.foursquare;
    }

  }

}

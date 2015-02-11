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

  public class CodeProjectLogin : Login //, ILoginForm
  {

    public CodeProjectLogin(string clientId, string clientSecret, string returnUrl) : this(new CodeProjectClient(clientId, clientSecret) { ReturnUrl = returnUrl }) { }

    public CodeProjectLogin(CodeProjectClient client) : base(client) 
    {
      this.Icon = Properties.Resources.codeproject;
      this.Width = 720;
      this.Height = 550;
    }

    public void WebDocumentLoaded(System.Windows.Forms.WebBrowser webBrowser, Uri url)
    {
      if (!url.OriginalString.Equals("about:blank", StringComparison.InvariantCultureIgnoreCase))
      {
        webBrowser.DocumentText = webBrowser.DocumentText.Insert
        (
          webBrowser.DocumentText.IndexOf("</head>"), 
          String.Format
          (
            "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /><base href=\"{0}://{1}{2}\">", 
            url.Scheme,
            url.Host,
            ""//url.AbsolutePath
          )
        );
      }
    }

  }

}

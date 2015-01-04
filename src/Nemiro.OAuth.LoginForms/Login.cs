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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Nemiro.OAuth.LoginForms
{

  public partial class Login : Form
  {

    /// <summary>
    /// An instance of the OAuth client.
    /// </summary>
    protected OAuthBase Client { get; set; }

    /// <summary>
    /// Gets a value indicating whether the current authorization result is successful or not.
    /// </summary>
    public bool IsSuccessfully { get; protected set; }

    /// <summary>
    /// Gets an instance of the access token.
    /// </summary>
    public AccessToken AccessToken
    {
      get
      {
        if (!this.IsSuccessfully) { return null; }
        return (AccessToken)this.Client.AccessToken;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Login"/> class.
    /// </summary>
    protected Login()
    {
      InitializeComponent();
      this.SetProgressImage(global::Nemiro.OAuth.LoginForms.Properties.Resources.loader);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Login"/> class with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    public Login(OAuthBase client) : this()
    {
      this.Client = client;
      this.Text = String.Format(this.Text, this.Client.ProviderName);
      this.ShowProgress();
      webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
      Task.Factory.StartNew(() => this.SetUrl(this.Client.AuthorizationUrl));
    }

    private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      // waiting for results
      if (e.Url.Query.IndexOf("code=") != -1 || e.Url.Query.IndexOf("oauth_verifier=") != -1)
      {
        // is result
        var v = UniValue.ParseParameters(e.Url.Query.Substring(1));
        if (v.ContainsKey("code"))
        {
          this.GetAccessToken(v["code"].ToString());
        }
        else
        {
          this.GetAccessToken(v["oauth_verifier"].ToString());
        }
        return;
      }
      // access denied
      if (!String.IsNullOrEmpty(e.Url.Query) && e.Url.Query.IndexOf("error=access_denied") != -1)
      {
        this.Close();
      }
      // custom handler
      if (typeof(ILoginForm).IsAssignableFrom(this.GetType()))
      {
        ((ILoginForm)this).WebDocumentLoaded(this.webBrowser1, e.Url);
      }
    }

    protected internal void GetAccessToken(string authorizationCode)
    {
      try
      {
        // verify code
        this.Client.AuthorizationCode = authorizationCode;
        // show progress
        this.ShowProgress();
        // save access token to application settings
        var t = Task.Factory.StartNew(() =>
        {
          this.IsSuccessfully = this.Client.AccessToken.IsSuccessfully;
          this.Close();
        });
      }
      catch (Exception ex)
      {
        // show error message
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.Close();
      }
    }

    protected internal void ShowProgress()
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action(ShowProgress));
        return;
      }
      this.webBrowser1.Visible = false;
      this.pictureBox1.Visible = true;
    }

    protected internal void HideProgress()
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action(HideProgress));
        return;
      }
      this.webBrowser1.Visible = true;
      this.pictureBox1.Visible = false;
    }

    /// <summary>
    /// Sets new image of the progress.
    /// </summary>
    /// <param name="image">The image to set.</param>
    public void SetProgressImage(Image image)
    {
      this.pictureBox1.Image = image;
    }

    public new void Close()
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action(Close));
        return;
      }
      // set dialog result
      if (this.IsSuccessfully)
      {
        this.DialogResult = System.Windows.Forms.DialogResult.OK;
      }
      else
      {
        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      }
      // close form
      base.Close();
    }

    private void SetUrl(string url)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<string>(SetUrl), url);
        return;
      }
      webBrowser1.Navigate(url);
      this.HideProgress();
    }

  }

}

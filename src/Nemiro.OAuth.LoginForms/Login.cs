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
using Microsoft.Win32;
using System.Security;
using System.IO;

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
    /// Gets the access token value.
    /// </summary>
    public string AccessTokenValue
    {
      get
      {
        if (!this.IsSuccessfully) { return null; }
        return this.AccessToken.Value;
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
      this.webBrowser1.ScriptErrorsSuppressed = true;
      webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
      webBrowser1.ProgressChanged += webBrowser1_ProgressChanged;
      Task.Factory.StartNew(() => this.SetUrl(this.Client.AuthorizationUrl));
    }

    private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
    {
      if (e.CurrentProgress == e.MaximumProgress)
      {
        
      }
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

    private bool AccessTokenProcessing = false; 

    protected internal void GetAccessToken(string authorizationCode)
    {
      if (this.AccessTokenProcessing) { return; }
      this.AccessTokenProcessing = true;

      // verify code
      this.Client.AuthorizationCode = authorizationCode;
      // show progress
      this.ShowProgress();
      // save access token to application settings
      Task.Factory.StartNew(() =>
      {
        try
        {
          this.IsSuccessfully = this.Client.AccessToken.IsSuccessfully;
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // this.AccessTokenProcessing = false;
        this.Close();
      });
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

    private void Login_Load(object sender, EventArgs e)
    {
      try
      {
        var programName = Path.GetFileName(Environment.GetCommandLineArgs().First());

        // get current version of IE emulation
        var currentEmulationVersion = BrowserEmulationVersion.Default;
        var emulationKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);

        if (emulationKey != null)
        {
          object value = emulationKey.GetValue(programName, null);

          if (value != null)
          {
            Enum.TryParse<BrowserEmulationVersion>(value.ToString().Split('.').First(), out currentEmulationVersion);
          }
        }

        // get current IE version
        int ieVersion = 0;
        var ieKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer");

        if (ieKey != null)
        {
          object value = ieKey.GetValue("svcVersion", null) ?? ieKey.GetValue("Version", null);

          if (value != null)
          {
            int.TryParse(value.ToString().Split('.').First(), out ieVersion);
          }
        }

        // check versions
        if (ieVersion > 0 && currentEmulationVersion == BrowserEmulationVersion.Default)
        {
          // set IE emulation version
          if (ieVersion >= 11)
          {
            emulationKey.SetValue(programName, (int)BrowserEmulationVersion.IE11, RegistryValueKind.DWord);
          }
          else
          {
            var v = BrowserEmulationVersion.IE7; 
            switch (ieVersion)
            {
              case 10:
                v = BrowserEmulationVersion.IE10;
                break;
              case 9:
                v = BrowserEmulationVersion.IE9;
                break;
              case 8:
                v = BrowserEmulationVersion.IE8;
                break;
            }
            emulationKey.SetValue(programName, (int)v, RegistryValueKind.DWord);
          }
        }
      }
      /*catch (SecurityException)
      {
      }
      catch (UnauthorizedAccessException)
      {
      }*/
      catch { }
    }

  }

}

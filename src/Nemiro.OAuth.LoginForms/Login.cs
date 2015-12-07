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
using Microsoft.Win32;
using System.Security;
using System.IO;
using System.Threading;
using System.Diagnostics;

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
    /// Gets or sets autrhorization url.
    /// </summary>
    protected internal string AuthorizationUrl { get; protected set; }

    /// <summary>
    /// Indicates can sign or not.
    /// </summary>
    protected internal bool CanLogin { get; protected set; }

    /// <summary>
    /// Indicates can logout or not.
    /// </summary>
    protected internal bool CanLogout { get; set; }

    private string _AuthorizationCode = "";

    /// <summary>
    /// Authorization code.
    /// </summary>
    protected internal string AuthorizationCode
    {
      get
      {
        return _AuthorizationCode;
      }
      set
      {
        Debug.WriteLine(String.Format("Set AuthorizationCode {0}", value), "LoginForm");

        // bad solution...
        _AuthorizationCode = value;
        this.CanLogin = false;

        if (this.AutoLogout && this.CanLogout)
        {
          // clear browser cookies
          this.Logout();
        }
        else
        {
          // get access token by auth code
          this.GetAccessToken();
        }
      }
    }

    /// <summary>
    /// Gets or sets auto logout mode.
    /// </summary>
    private bool AutoLogout { get; set; }

    private WebBrowserCallback Callback = null;

    private bool AccessTokenProcessing = false;

    private bool Timeout = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Login"/> class.
    /// </summary>
    protected Login()
    {
      InitializeComponent();
      this.SetIEVersion();
      this.SetProgressImage(global::Nemiro.OAuth.LoginForms.Properties.Resources.loader2);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Login"/> class with a specified OAuth client.
    /// </summary>
    /// <param name="client">Instance of the OAuth client.</param>
    /// <param name="autoLogout">Disables saving and restoring authorization cookies in WebBrowser. Default: false.</param>
    public Login(OAuthBase client, bool autoLogout = false)
      : this()
    {
      this.Client = client;
      this.AutoLogout = autoLogout;
      this.Text = String.Format(this.Text, this.Client.ProviderName);
      this.CanLogin = true;
      this.CanLogout = true;

      this.webBrowser1.ScriptErrorsSuppressed = true;
      this.webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;

      this.Controls.SetChildIndex(this.webBrowser1, 1);
      this.Controls.SetChildIndex(this.pictureBox1, 0);

      this.AuthorizationUrl = this.Client.AuthorizationUrl;

      Thread t = null;

      if (this.AutoLogout)
      {
        t = new Thread(() => this.Logout());
      }
      else
      {
        t = new Thread(() => this.SetUrl(this.AuthorizationUrl));
      }

      t.IsBackground = true;
      t.Start();
    }

    private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      Debug.WriteLine(String.Format("Document {0}", this.webBrowser1.ReadyState), "LoginForm");
      Debug.WriteLine(e.Url.ToString(), "LoginForm");

      if (!this.Timeout)
      {
        if (this.webBrowser1.ReadyState != WebBrowserReadyState.Complete && this.webBrowser1.ReadyState != WebBrowserReadyState.Interactive) // || this.webBrowser1.IsBusy
        {
          //if (this.webBrowser1.ReadyState == WebBrowserReadyState.Interactive)
          //{
          //  this.StartWaiting();
          //}
          return;
        }

        /*try
        {
          if (this.webBrowser1.Document.Window.Frames.Count > 0 && this.webBrowser1.Document.Window.Frames[0].Url.Equals(e.Url))
          {
            this.StartWaiting();
            return;
          }
        }
        catch { }*/
      }

      if (this.Callback != null)
      {
        Debug.WriteLine("Custom Callback", "LoginForm");
        this.Callback(sender, new WebBrowserCallbackEventArgs(e.Url));
      }
      else
      {
        this.DefaultCallback(sender, new WebBrowserCallbackEventArgs(e.Url));
      }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      Debug.WriteLine("Timeout", "LoginForm");
      this.Timeout = true;
      this.Enabled = false;
      this.webBrowser1_DocumentCompleted(this.webBrowser1, new WebBrowserDocumentCompletedEventArgs(this.webBrowser1.Url));
    }

    protected internal void DefaultCallback(object sender, WebBrowserCallbackEventArgs e)
    {
      Debug.WriteLine("Default Callback", "LoginForm");

      // waiting for results
      if (e.Url.Query.IndexOf("code=") != -1 || e.Url.Query.IndexOf("oauth_verifier=") != -1)
      {
        this.CanLogin = false;

        // is result
        var v = UniValue.ParseParameters(e.Url.Query.Substring(1));

        if (v.ContainsKey("code"))
        {
          this.AuthorizationCode = v["code"].ToString();
        }
        else
        {
          this.AuthorizationCode = v["oauth_verifier"].ToString();
        }

        return;
      }

      // access denied
      if (!String.IsNullOrEmpty(e.Url.Query) && e.Url.Query.IndexOf("error=access_denied") != -1)
      {
        this.Close();
      }

      // hide progress
      if (!this.AccessTokenProcessing) // is impossible to determine the exact address
      {
        this.HideProgress();
      }

      // additional custom handler
      if (typeof(ILoginForm).IsAssignableFrom(this.GetType()))
      {
        this.CanLogin = false;
        Debug.WriteLine("ILoginForm", "LoginForm");
        ((ILoginForm)this).WebDocumentLoaded(this.webBrowser1, e.Url);
      }
    }

    /// <summary>
    /// Gets access token.
    /// </summary>
    /// <param name="authorizationCode">The authorization code. Default: <see cref="AuthorizationCode"/>.</param>
    protected internal void GetAccessToken(string authorizationCode = "")
    {
      if (this.AccessTokenProcessing) { return; }

      if (String.IsNullOrEmpty(authorizationCode))
      {
        authorizationCode = this.AuthorizationCode;
      }

      this.AccessTokenProcessing = true;
      this.SetProgressImage(global::Nemiro.OAuth.LoginForms.Properties.Resources.loader);
      this.ShowProgress();

      var t = new Thread(GetAccessTokenThread);
      t.IsBackground = true;
      t.Start(authorizationCode);
    }

    private void GetAccessTokenThread(object args)
    {
      Debug.WriteLine(String.Format("GetAccessTokenThread {0}", args), "LoginForm");

      // verify code
      this.Client.AuthorizationCode = args.ToString();

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
    }

    /// <summary>
    /// Shows progess image.
    /// </summary>
    protected internal void ShowProgress()
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action(ShowProgress));
        return;
      }

      Debug.WriteLine("ShowProgress", "LoginForm");

      // because document is not loaded
      //this.webBrowser1.Visible = false;
      this.pictureBox1.Visible = true;
    }

    /// <summary>
    /// Hides progess image.
    /// </summary>
    protected internal void HideProgress()
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action(HideProgress));
        return;
      }

      Debug.WriteLine("HideProgress", "LoginForm");

      //this.webBrowser1.Visible = true;
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

    /// <summary>
    /// Closes the form.
    /// </summary>
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

    /// <summary>
    /// Sets and opens new url.
    /// </summary>
    /// <param name="url">The new url.</param>
    /// <param name="callback">The callback function when load completed.</param>
    protected internal void SetUrl(string url, WebBrowserCallback callback = null)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<string, WebBrowserCallback>(SetUrl), url, callback);
        return;
      }

      Debug.WriteLine(String.Format("SetUrl {0}", url), "LoginForm");

      this.Timeout = false;
      this.timer1.Enabled = false;

      this.ShowProgress();
      this.Callback = callback;
      this.webBrowser1.Navigate(url);
    }

    private void StartWaiting()
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action(StartWaiting));
        return;
      }

      this.timer1.Enabled = true;
    }

    /// <summary>
    /// Sets latest version of IE emulation for current application.
    /// </summary>
    private void SetIEVersion()
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
            try
            {
              currentEmulationVersion = (BrowserEmulationVersion)Enum.Parse(typeof(BrowserEmulationVersion), value.ToString().Split('.').First());
            }
            catch { }
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
            emulationKey.SetValue(programName, (int)BrowserEmulationVersion.IE11Edge, RegistryValueKind.DWord);
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

    /// <summary>
    /// Removes all cookies.
    /// </summary>
    protected internal void KillCookies()
    {
      if (this.webBrowser1.Document != null && !String.IsNullOrEmpty(this.webBrowser1.Document.Cookie))
      {
        var cookies = webBrowser1.Document.Cookie.Split(';').ToList();
        foreach (var c in cookies)
        {
          var domains = this.webBrowser1.Url.Host.Split('.').ToList();
          while (domains.Count > 1)
          {
            this.webBrowser1.Document.Cookie = String.Format("{0}=; Thu, 01-Jan-1970 00:00:01 GMT; domain={1};", c.Split('=').First().Trim(), String.Join(".", domains));
            // path=
            domains.RemoveAt(0);
          }
        }
      }
    }

    /// <summary>
    /// Logout.
    /// </summary>
    public virtual void Logout()
    {
      Debug.WriteLine("Logout", "LoginForm");

      // goto home page
      var u = new Uri(this.AuthorizationUrl);

      this.SetUrl
      (
        String.Format("{0}://{1}", u.Scheme, u.Host),
        (object sender, WebBrowserCallbackEventArgs e) =>
        {
          // remove cookies
          this.KillCookies();
          // next action
          if (this.CanLogin)
          {
            // goto login
            this.SetUrl(this.AuthorizationUrl);
          }
          else
          {
            // can not login, get access token
            this.GetAccessToken();
          }
        }
      );
    }

  }

}
// ----------------------------------------------------------------------------
// Copyright © Aleksey Nemiro, 2015-2017. All rights reserved.
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Nemiro.OAuth.LoginForms
{

  /// <summary>
  /// The base class for the authorization forms.
  /// </summary>
  public partial class Login : Form
  {

    #region ..imports..

    [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

    #endregion
    #region ..constants, fields and properties..

    private const int INTERNET_SUPPRESS_COOKIE_PERSIST = 3;
    private const int INTERNET_OPTION_SUPPRESS_BEHAVIOR = 81;
    private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

    /// <summary>
    /// An instance of the OAuth client.
    /// </summary>
    public OAuthBase Client { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the current authorization result is successful or not.
    /// </summary>
    public bool IsSuccessfully { get; protected set; }

    /// <summary>
    /// Error message if an error occurred during the authorization process.
    /// </summary>
    public string ErrorMessage { get; protected set; }

    /// <summary>
    /// Gets user profile.
    /// </summary>
    public UserInfo UserInfo { get; protected set; }

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
        Debug.WriteLine(string.Format("Set AuthorizationCode {0}", value), "LoginForm");

        // bad solution...
        _AuthorizationCode = value;
        this.CanLogin = false;

        if (this.AutoLogout && this.CanLogout)
        {
          // clear browser cookies
          this.WinInetSetOption(INTERNET_OPTION_END_BROWSER_SESSION, null);
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

    /// <summary>
    /// Gets or sets a value that indicates the need to load user profile information.
    /// </summary>
    private bool LoadUserInfo { get; set; }

    private WebBrowserCallback Callback = null;

    private bool AccessTokenProcessing = false;

    private bool Timeout = false;

    private Uri LastUri = new Uri("about:blank");
    
    #endregion
    #region ..constructors..

    /// <summary>
    /// Initializes a new instance of the <see cref="Login"/>.
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
    /// <param name="loadUserInfo">Indicates the need to make a request for recive the user profile or not. Default: false.</param>
    /// <param name="responseType">Allows to set the type of response that is expected from the server. Default: <see cref="ResponseType.Token"/>. Only for OAuth v2.0.</param>
    public Login(OAuthBase client, bool autoLogout = false, bool loadUserInfo = false, string responseType = "token") : this()
    {
      if (client.Version.Major == 2)
      {
        client.Get<OAuth2Client>().ResponseType = responseType;
      }

      this.Client = client;
      this.AutoLogout = autoLogout;
      this.LoadUserInfo = loadUserInfo;
      this.Text = string.Format(this.Text, this.Client.ProviderName);
      this.CanLogin = true;
      this.CanLogout = true;

      this.webBrowser1.ScriptErrorsSuppressed = true;
      this.webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
      this.webBrowser1.DocumentTitleChanged += webBrowser1_DocumentTitleChanged;

#if DEBUG
      this.webBrowser1.Navigating += webBrowser1_Navigating;
      this.webBrowser1.Navigated += webBrowser1_Navigated;
#endif

      this.Controls.SetChildIndex(this.webBrowser1, 1);
      this.Controls.SetChildIndex(this.pictureBox1, 0);

      this.AuthorizationUrl = this.Client.AuthorizationUrl;

      Thread t = null;

      if (this.AutoLogout)
      {
        this.WinInetSetOption(INTERNET_OPTION_SUPPRESS_BEHAVIOR, INTERNET_SUPPRESS_COOKIE_PERSIST);

        t = new Thread(() => this.Logout());
      }
      else
      {
        t = new Thread(() => this.SetUrl(this.AuthorizationUrl));
      }

      t.IsBackground = true;
      t.Start();
    }

    #endregion
    #region ..handlers and methods..

    private void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
    {
      Debug.WriteLine(string.Format("{0} {1}, title <{2}>, state <{3}>", "DocumentTitleChanged", webBrowser1.Url, webBrowser1.DocumentTitle, webBrowser1.ReadyState), "LoginForm");

      if (!this.LastUri.Equals(webBrowser1.Url) && (this.webBrowser1.ReadyState == WebBrowserReadyState.Complete || this.webBrowser1.ReadyState == WebBrowserReadyState.Interactive))
      {
        this.LastUri = webBrowser1.Url;
        this.webBrowser1_DocumentCompleted(sender, new WebBrowserDocumentCompletedEventArgs(webBrowser1.Url));
      }
    }

#if DEBUG

    private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
      Debug.WriteLine(string.Format("{0} {1}", "Navigating", e.Url), "LoginForm");
    }

    private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      Debug.WriteLine(string.Format("{0} {1}, state <{2}>", "Navigated", e.Url, this.webBrowser1.ReadyState), "LoginForm");
    }

#endif

    private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      Debug.WriteLine(string.Format("{0} {1}, state <{2}>", "DocumentCompleted", e.Url, this.webBrowser1.ReadyState), "LoginForm");

      if (!this.Timeout)
      {
        if (this.webBrowser1.ReadyState != WebBrowserReadyState.Complete && this.webBrowser1.ReadyState != WebBrowserReadyState.Interactive) // || this.webBrowser1.IsBusy
        {
          return;
        }
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

    /// <summary>
    /// Default callback.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event arguments.</param>
    protected internal void DefaultCallback(object sender, WebBrowserCallbackEventArgs e)
    {
      Debug.WriteLine("Default Callback", "LoginForm");

      // waiting for results
      if (this.Client.Version.Major == 2 && ((OAuth2Client)this.Client).ResponseType == ResponseType.Token)
      {
        // OAuth v2.0 with token request
        int tokenStartIndex = e.Url.Fragment.IndexOf("access_token");

        if (tokenStartIndex != -1)
        {
          this.CanLogin = false;

          var token = AccessToken.Parse(e.Url.Fragment.Substring(1));

          this.Client.SetAccessToken(token);

          // completion to set 
          this.GetAccessTokenThread("");

          return;
        }
      }

      // OAuth v1.x/2.0 with authorization code request or token request is not support
      if (e.Url.Query.IndexOf("code=") != -1 || e.Url.Query.IndexOf("oauth_verifier=") != -1)
      {
        // is result
        var code = UniValue.ParseParameters(e.Url.Query.Substring(1));

        if (code.ContainsKey("code"))
        {
          this.AuthorizationCode = code["code"].ToString();
        }
        else
        {
          this.AuthorizationCode = code["oauth_verifier"].ToString();
        }

        return;
      }

      string errorMessage = null;

      if (!string.IsNullOrEmpty(e.Url.Query))
      {
        errorMessage = this.GetErrorMessage(UniValue.ParseParameters(e.Url.Query.Substring(1)));
      }
      
      if (string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(e.Url.Fragment))
      {
        errorMessage = this.GetErrorMessage(UniValue.ParseParameters(e.Url.Fragment.Substring(1)));
      }

      if (!string.IsNullOrEmpty(errorMessage))
      {
        // MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.ErrorMessage = errorMessage;
        this.Close();
        return;
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

    private string GetErrorMessage(UniValue value)
    {
      if (value["error_message"].HasValue)
      {
        return value["error_message"].ToString();
      }
      else if (value["error_description"].HasValue)
      {
        return value["error_description"].ToString();
      }
      else if (value["error"].HasValue)
      {
        return value["error"].ToString();
      }

      return null;
    }

    /// <summary>
    /// Gets access token.
    /// </summary>
    /// <param name="authorizationCode">The authorization code. Default: <see cref="AuthorizationCode"/>.</param>
    protected internal void GetAccessToken(string authorizationCode = "")
    {
      if (this.AccessTokenProcessing) { return; }

      if (this.Client.Version.Major == 2 && this.Client.Get<OAuth2Client>().ResponseType.IsToken && this.AccessToken != null)
      {
        this.GetAccessTokenThread(string.Empty);

        return;
      }

      if (string.IsNullOrEmpty(authorizationCode))
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
      Debug.WriteLine(string.Format("GetAccessTokenThread {0}", args), "LoginForm");

      try
      {
        // verify code
        this.Client.AuthorizationCode = args.ToString();

        this.IsSuccessfully = this.Client.AccessToken.IsSuccessfully;

        if (this.LoadUserInfo)
        {
          this.UserInfo = this.Client.GetUserInfo();

          Debug.WriteLine(string.Format("UserInfo {0}", this.UserInfo.Items), "LoginForm");
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage = ex.Message;
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

      Debug.WriteLine(string.Format("SetUrl {0}", url), "LoginForm");

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
      if (this.webBrowser1.Document != null && !string.IsNullOrEmpty(this.webBrowser1.Document.Cookie))
      {
        var cookies = webBrowser1.Document.Cookie.Split(';').ToList();

        foreach (var c in cookies)
        {
          var domains = this.webBrowser1.Url.Host.Split('.').ToList();

          while (domains.Count > 1)
          {
            this.webBrowser1.Document.Cookie = string.Format("{0}=; Thu, 01-Jan-1970 00:00:01 GMT; domain={1};", c.Split('=').First().Trim(), String.Join(".", domains.ToArray()));
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
        string.Format("{0}://{1}", u.Scheme, u.Host),
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

    protected bool WinInetSetOption(int settingCode, int? option)
    {
      IntPtr optionPtr = IntPtr.Zero;
      int size = 0;

      if (option.HasValue)
      {
        size = sizeof(int);
        optionPtr = Marshal.AllocCoTaskMem(size);
        Marshal.WriteInt32(optionPtr, option.Value);
      }

      bool success = InternetSetOption(0, settingCode, optionPtr, size);

      if (optionPtr != IntPtr.Zero)
      {
        Marshal.Release(optionPtr);
      }

      return success;
    }

    #endregion

  }

}
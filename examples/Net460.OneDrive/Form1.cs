using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;

namespace Net46.OneDrive
{

  public partial class Form1 : Form
  {

    private string LastFolderId = null;
    private string CurrentFolderId = null;
    private long CurrentFileLength = 0;

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(Properties.Settings.Default.AccessToken))
      {
        this.GetAccessToken();
      }
      else
      {
        this.GetFiles();
      }
    }

    private void GetAccessToken()
    {
      var login = new LiveLogin
      (
        "000000004C1337C9",
        "7N3IwTyTGoGGimndiJAQiG2GBspOLyFZ",
        "http://oauthproxy.nemiro.net/",
        // scope: https://dev.onedrive.com/auth/msa_oauth.htm#authentication-scopes
        "onedrive.readwrite", // ,onedrive.appfolder
        loadUserInfo: true
      );

      login.Owner = this;
      login.ShowDialog();

      if (login.IsSuccessfully)
      {
        Properties.Settings.Default.AccessToken = login.AccessToken.Value;
        Properties.Settings.Default.Save();

        this.Text = String.Format("{0} (OneDrive)", login.UserInfo.DisplayName ?? login.UserInfo.UserName);

        this.GetFiles();
      }
      else
      {
        MessageBox.Show("error...");
      }
    }

    private void GetFiles()
    {
      // https://dev.onedrive.com/items/list.htm

      this.Cursor = Cursors.WaitCursor;

      string url = "https://api.onedrive.com/v1.0/drive/root/children";

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        url = String.Format("https://api.onedrive.com/v1.0/drive/items/{0}/children", this.CurrentFolderId);
      }

      OAuthUtility.GetAsync
      (
        url,
        authorization: new HttpAuthorization(AuthorizationType.Bearer, Properties.Settings.Default.AccessToken),
        callback: GetFiles_Result
      );
    }

    private void GetFiles_Result(RequestResult result)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<RequestResult>(GetFiles_Result), result);
        return;
      }

      this.Cursor = Cursors.Default;

      if (Enumerable.Range(200, 299).Contains(result.StatusCode)) // result.StatusCode == 200
      {

        listBox1.Items.Clear();

        listBox1.DisplayMember = "name";

        foreach (UniValue file in result["value"])
        {
          listBox1.Items.Add(file);
        }

        if (!String.IsNullOrEmpty(this.CurrentFolderId))
        {
          listBox1.Items.Insert(0, UniValue.Create(new { name = "..", toup = true }));
        }

      }
      else
      {
        this.ShowError(result);
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      // https://dev.onedrive.com/items/create.htm

      string url = "https://api.onedrive.com/v1.0/drive/root/children";

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        url = String.Format("https://api.onedrive.com/v1.0/drive/items/{0}/children", this.CurrentFolderId);
      }

      OAuthUtility.PostAsync
      (
        url,
        authorization: new HttpAuthorization(AuthorizationType.Bearer, Properties.Settings.Default.AccessToken),
        contentType: "application/json",
        parameters: new HttpParameterCollection
        {
          new
          {
            name = textBox1.Text,
            folder = new { }
          }
        },
        callback: CreateFolder_Result
      );
    }

    private void CreateFolder_Result(RequestResult result)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<RequestResult>(CreateFolder_Result), result);
        return;
      }

      if (Enumerable.Range(200, 299).Contains(result.StatusCode))
      {
        this.GetFiles();
      }
      else
      {
        this.ShowError(result);
      }
    }

    private void listBox1_DoubleClick(object sender, EventArgs e)
    {
      if (listBox1.SelectedItem == null) { return; }
      UniValue file = (UniValue)listBox1.SelectedItem;

      if (!UniValue.IsNullOrEmpty(file["toup"]))
      {
        this.CurrentFolderId = this.LastFolderId;
        this.LastFolderId = "";
      }
      else
      {
        if (file["folder"].HasValue)
        {
          this.LastFolderId = this.CurrentFolderId;
          this.CurrentFolderId = file["id"].ToString();
        }
        else
        {
          // https://dev.onedrive.com/items/download.htm

          saveFileDialog1.FileName = Path.GetFileName(file["name"].ToString());
          if (saveFileDialog1.ShowDialog() != DialogResult.OK)
          {
            return;
          }
          var web = new WebClient();
          web.DownloadProgressChanged += DownloadProgressChanged;
          web.Headers.Add("Authorization", String.Format("Bearer {0}", Properties.Settings.Default.AccessToken));
          web.DownloadFileAsync(new Uri(String.Format("https://api.onedrive.com/v1.0/drive/items/{0}/content", file["id"])), saveFileDialog1.FileName);
        }
      }

      this.GetFiles();
    }

    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      progressBar1.Value = e.ProgressPercentage;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      // https://dev.onedrive.com/items/upload_put.htm

      progressBar1.Value = 0;

      if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }

      string fileName = Path.GetFileName(openFileDialog1.FileName);

      string url = String.Format("https://api.onedrive.com/v1.0/drive/root/children/{0}/content", fileName);

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        url = String.Format("https://api.onedrive.com/v1.0/drive/items/{0}/children/{1}/content", this.CurrentFolderId, fileName);
      }

      var file = openFileDialog1.OpenFile();
      this.CurrentFileLength = file.Length;

      OAuthUtility.PutAsync
      (
        url,
        authorization: new HttpAuthorization(AuthorizationType.Bearer, Properties.Settings.Default.AccessToken),
        contentType: "application/octet-stream",
        parameters: file,
        // handler of result
        callback: Upload_Result,
        // handler of uploading
        streamWriteCallback: Upload_Processing
      );
    }

    private void Upload_Processing(object sender, StreamWriteEventArgs e)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<object, StreamWriteEventArgs>(this.Upload_Processing), sender, e);
        return;
      }

      progressBar1.Value = Math.Min(Convert.ToInt32(Math.Round((e.TotalBytesWritten * 100.0) / this.CurrentFileLength)), 100);
    }

    private void Upload_Result(RequestResult result)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<RequestResult>(Upload_Result), result);
        return;
      }

      if (Enumerable.Range(200, 299).Contains(result.StatusCode))
      {
        this.GetFiles();
      }
      else
      {
        this.ShowError(result);
      }
    }

    private void ShowError(RequestResult result)
    {
      if (result["error"].HasValue && result["error"]["message"].HasValue)
      {
        MessageBox.Show
        (
          result["error"]["message"].ToString(),
          result["error"]["code"].HasValue ? result["error"]["code"].ToString() : "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
        );
      }
      else
      {
        MessageBox.Show(result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

  }

}
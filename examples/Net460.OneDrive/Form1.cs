using System;
using System.Collections.Specialized;
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

    private const string API_BASE_URL = "https://api.onedrive.com/v1.0";
    private const int FRAGMENT_SIZE = 5 * 1024 * 1024;

    private string LastFolderId = null;
    private string CurrentFolderId = null;
    private long CurrentFileLength = 0;
    private long CurrentFileTotalBytesSended = 0;

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

      string url = String.Format("{0}/drive/root/children", API_BASE_URL);

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        url = String.Format("{0}/drive/items/{1}/children", API_BASE_URL, this.CurrentFolderId);
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

      if (Enumerable.Range(200, 100).Contains(result.StatusCode)) // result.StatusCode == 200
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

      string url = String.Format("{0}/drive/root/children", API_BASE_URL);

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        url = String.Format("{0}/drive/items/{1}/children", API_BASE_URL, this.CurrentFolderId);
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

      if (Enumerable.Range(200, 100).Contains(result.StatusCode))
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
          web.DownloadFileAsync(new Uri(String.Format("{0}/drive/items/{1}/content", API_BASE_URL, file["id"])), saveFileDialog1.FileName);
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

      progressBar1.Value = 0;

      if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }

      var file = openFileDialog1.OpenFile();
      this.CurrentFileLength = file.Length;
      this.CurrentFileTotalBytesSended = 0;

      if (this.CurrentFileLength < 10 * 1024 * 1024)
      {
        this.UploadSmallFile(file);
      }
      else
      {
        this.UploadLargeFile(file);
      }
    }

    private void UploadSmallFile(Stream file)
    {
      // https://dev.onedrive.com/items/upload_put.htm

      string fileName = Path.GetFileName(openFileDialog1.FileName);

      string url = String.Format("{0}/drive/root/children/{1}/content", API_BASE_URL, fileName);

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        url = String.Format("{0}/drive/items/{1}/children/{2}/content", API_BASE_URL, this.CurrentFolderId, fileName);
      }

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

    private void UploadLargeFile(Stream file)
    {
      // https://dev.onedrive.com/items/upload_large_files.htm

      // 1. Create an upload session
      string fileName = Path.GetFileName(openFileDialog1.FileName);

      string url = String.Format("{0}/drive/root:/{1}:/upload.createSession", API_BASE_URL, fileName);

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        url = String.Format("{0}/drive/items/{1}:/{2}:/upload.createSession", API_BASE_URL, this.CurrentFolderId, fileName);
      }

      OAuthUtility.PostAsync
      (
        url,
        authorization: new HttpAuthorization(AuthorizationType.Bearer, Properties.Settings.Default.AccessToken),
        callback: (RequestResult result) =>
        {
          // https://dev.onedrive.com/items/upload_large_files.htm#upload-fragments

          if (Enumerable.Range(200, 100).Contains(result.StatusCode))
          {
            string uploadUrl = result["uploadUrl"].ToString();
            this.UploadLargeFileFragment(uploadUrl, new BinaryReader(file), 0, FRAGMENT_SIZE, file.Length);
          }
          else
          {
            this.ShowError(result);
          }
        }
      );
    }

    private void UploadLargeFileFragment(string uploadUrl, BinaryReader stream, long start, long end, long total)
    {
      end = Math.Min(end, total);

      byte[] buffer = stream.ReadBytes(Convert.ToInt32(end - start));

      // Invoke(new Action(() => progressBar1.Value = Math.Min(Convert.ToInt32(Math.Round((start * 100.0) / total)), 100)));

      Console.WriteLine("bytes {0}-{1}/{2}", start, end - 1, total);

      OAuthUtility.PutAsync
      (
        uploadUrl,
        authorization: new HttpAuthorization(AuthorizationType.Bearer, Properties.Settings.Default.AccessToken),
        parameters: buffer,
        headers: new NameValueCollection { { "Content-Range", String.Format("bytes {0}-{1}/{2}", start, end - 1, total) } },
        streamWriteCallback: Upload_Processing2,
        callback: (RequestResult result) =>
        {
          if (result.StatusCode == 202)
          {
            // var next = result["nextExpectedRanges"][0].ToString().Split('-').Select(itm => Convert.ToInt64(itm)).ToArray();
            // next part
            this.UploadLargeFileFragment(uploadUrl, stream, end, end + FRAGMENT_SIZE, total);
          }
          else if (result.StatusCode == 201)
          {
            stream.Close();
            this.Upload_Result(result);
          }
          else
          {
            stream.Close();
            this.ShowError(result);
          }
        }
      );
    }

    private void Upload_Processing2(object sender, StreamWriteEventArgs e)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<object, StreamWriteEventArgs>(this.Upload_Processing2), sender, e);
        return;
      }

      if (e.IsCompleted)
      {
        this.CurrentFileTotalBytesSended += e.TotalBytesWritten;
      }

      progressBar1.Value = Math.Min(Convert.ToInt32(Math.Round((this.CurrentFileTotalBytesSended * 100.0) / this.CurrentFileLength)), 100);
    }

    private void Upload_Result(RequestResult result)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<RequestResult>(Upload_Result), result);
        return;
      }

      if (Enumerable.Range(200, 100).Contains(result.StatusCode))
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
      if (result.StatusCode == 401)
      {
        this.GetAccessToken();
        return;
      }

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
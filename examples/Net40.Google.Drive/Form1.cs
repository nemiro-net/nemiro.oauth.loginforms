// ----------------------------------------------------------------------------
// Copyright © Aleksey Nemiro, 2015-2016. All rights reserved.
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
using System.Text;
using System.Windows.Forms;
using Nemiro.OAuth.LoginForms;
using Nemiro.OAuth;
using System.IO;
using System.Net;

namespace Google.Drive.Net40
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
      var login = new GoogleLogin("934704666049-129jsvmelksmcmf250ir90aqn8pk4nak.apps.googleusercontent.com", "OS7HZ1cfJnhdIFZ6fUsgamH-", "https://www.googleapis.com/auth/drive", loadUserInfo: true);
      login.Owner = this;
      login.ShowDialog();

      if (login.IsSuccessfully)
      {
        Properties.Settings.Default.AccessToken = login.AccessToken.Value;
        Properties.Settings.Default.Save();

        this.Text = String.Format("{0} (Google Drive)", login.UserInfo.DisplayName ?? login.UserInfo.UserName);

        this.GetFiles();
      }
      else
      {
        MessageBox.Show("error...");
      }
    }

    private void GetFiles()
    {
      // help: https://developers.google.com/drive/v2/reference/files/list

      this.Cursor = Cursors.WaitCursor;

      string q = "trashed=false";

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        q = String.Format("'{0}' in parents and {1}", this.CurrentFolderId, q);
      }
      else
      {
        q = String.Format("'{0}' in parents and {1}", "root", q);
      }

      OAuthUtility.GetAsync
      (
        "https://www.googleapis.com/drive/v2/files",
        parameters: new HttpParameterCollection
        {
          { "maxResults", "1000" },
          { "q", q }
        },
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

      if (result.StatusCode == 200)
      {

        listBox1.Items.Clear();

        listBox1.DisplayMember = "title";

        foreach (UniValue file in result["items"])
        {
          listBox1.Items.Add(file);
        }

        if (!String.IsNullOrEmpty(this.CurrentFolderId))
        {
          listBox1.Items.Insert(0, UniValue.Create(new { title = "..", toup = true }));
        }

      }
      else
      {
        if (result["error"]["errors"].Count > 0)
        {
          if (result["error"]["errors"][0]["reason"].Equals("authError", StringComparison.OrdinalIgnoreCase))
          {
            this.GetAccessToken();
          }
          else
          {
            MessageBox.Show(result["error"]["errors"][0]["message"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
        else
        {
          MessageBox.Show(result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      // help: https://developers.google.com/drive/v2/reference/files/insert

      object parents = null;

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        parents = new object[] { new { id = this.CurrentFolderId } };
      }

      UniValue content = UniValue.Create
      (
        new 
        { 
          mimeType = "application/vnd.google-apps.folder", 
          title = textBox1.Text, 
          parents = parents
        }
      );

      var parameters = new HttpParameterCollection();
      parameters.Encoding = Encoding.UTF8;
      parameters.Add("uploadType", "multipart");
      parameters.AddContent("application/json", content.ToString());
      
      OAuthUtility.PostAsync
      (
        "https://www.googleapis.com/upload/drive/v2/files",
        parameters: parameters,
        authorization: new HttpAuthorization(AuthorizationType.Bearer, Properties.Settings.Default.AccessToken),
        contentType: "multipart/related",
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

      if (result.StatusCode == 200)
      {
        this.GetFiles();
      }
      else
      {
        if (result["error"].HasValue)
        {
          MessageBox.Show(result["error"].ToString());
        }
        else
        {
          MessageBox.Show(result.ToString());
        }
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
        if (file["mimeType"] == "application/vnd.google-apps.folder")
        {
          this.LastFolderId = this.CurrentFolderId;
          this.CurrentFolderId = file["id"].ToString();
        }
        else
        {
          saveFileDialog1.FileName = Path.GetFileName(file["title"].ToString());
          if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
          {
            return;
          }
          var web = new WebClient();
          web.DownloadProgressChanged += DownloadProgressChanged;
          web.Headers.Add("Authorization", String.Format("Bearer {0}", Properties.Settings.Default.AccessToken));
          web.DownloadFileAsync(new Uri(String.Format("https://www.googleapis.com/drive/v2/files/{0}?alt=media", file["id"])), saveFileDialog1.FileName);
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

      if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }

      object parents = null;

      if (!String.IsNullOrEmpty(this.CurrentFolderId))
      {
        parents = new object[] { new { id = this.CurrentFolderId } };
      }

      UniValue properties = UniValue.Create
      (
        new 
        {
          title = Path.GetFileName(openFileDialog1.FileName), 
          parents = parents
        }
      );

      var file = openFileDialog1.OpenFile();
      this.CurrentFileLength = file.Length;

      var parameters = new HttpParameterCollection();
      parameters.Add("uploadType", "multipart");
      parameters.AddContent("application/json", properties.ToString());
      parameters.AddContent("application/octet-stream", file);

      OAuthUtility.PostAsync
      (
        "https://www.googleapis.com/upload/drive/v2/files",
        parameters,
        authorization: new HttpAuthorization(AuthorizationType.Bearer, Properties.Settings.Default.AccessToken),
        contentType: "multipart/related",
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

      if (result.StatusCode == 200)
      {
        this.GetFiles();
      }
      else
      {
        MessageBox.Show(result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

  }

}
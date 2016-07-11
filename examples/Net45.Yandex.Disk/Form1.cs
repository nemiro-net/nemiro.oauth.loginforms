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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;

namespace Yandex.Disk.Net45
{

  public partial class Form1 : Form
  {

    private bool AlwaysNewToken = true;
    private long CurrentFileLength = 0;
    private string CurrentPath = "/";
    private HttpAuthorization Authorization = null;

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(Properties.Settings.Default.AccessToken) || this.AlwaysNewToken)
      {
        this.GetAccessToken();
      }
      else
      {
        this.Authorization = new HttpAuthorization(AuthorizationType.OAuth, Properties.Settings.Default.AccessToken);
        this.GetFiles();
      }
    }

    private void GetAccessToken()
    {
      var login = new YandexLogin("80bbde206ef74606bf239039bce82ed0", "123c7a7a69614d18a5d5e23397009bac", this.AlwaysNewToken);
      login.Owner = this;
      login.ShowDialog();

      if (login.IsSuccessfully)
      {
        Properties.Settings.Default.AccessToken = login.AccessToken.Value;
        Properties.Settings.Default.Save();
        this.Authorization = new HttpAuthorization(AuthorizationType.OAuth, Properties.Settings.Default.AccessToken);
        this.GetFiles();
      }
      else
      {
        MessageBox.Show("error...");
      }
    }

    private void GetFiles()
    {
      this.Cursor = Cursors.WaitCursor;

      OAuthUtility.GetAsync
      (
        "https://cloud-api.yandex.net/v1/disk/resources",
        new HttpParameterCollection
        {
          { "path", this.CurrentPath }
        },
        authorization: this.Authorization,
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

        listBox1.DisplayMember = "name";

        foreach (UniValue file in result["_embedded"]["items"])
        {
          listBox1.Items.Add(file);
        }

        if (this.CurrentPath != "/")
        {
          listBox1.Items.Insert(0, UniValue.ParseJson("{name: '..'}"));
        }

      }
      else
      {
        this.ShowErrorMessage(result);
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      OAuthUtility.PutAsync
      (
        endpoint: "https://cloud-api.yandex.net/v1/disk/resources/",
        parameters: new HttpParameterCollection 
        {
          new HttpUrlParameter("path", Path.Combine(this.CurrentPath, textBox1.Text).Replace("\\", "/")) 
        },
        authorization: this.Authorization,
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

      if (result.StatusCode == 201)
      {
        this.GetFiles();
      }
      else
      {
        this.ShowErrorMessage(result);
      }
    }

    private void listBox1_DoubleClick(object sender, EventArgs e)
    {
      if (listBox1.SelectedItem == null) { return; }
      UniValue file = (UniValue)listBox1.SelectedItem;

      if (file["name"] == "..")
      {
        if (this.CurrentPath != "/")
        {
          this.CurrentPath = Path.GetDirectoryName(this.CurrentPath).Replace("\\", "/");
        }
      }
      else
      {
        if (file["type"] == "dir")
        {
          this.CurrentPath = file["path"].ToString();
        }
        else
        {
          saveFileDialog1.FileName = Path.GetFileName(file["name"].ToString());
          if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
          {
            return;
          }

          OAuthUtility.GetAsync
          (
            endpoint: "https://cloud-api.yandex.net/v1/disk/resources/download",
            parameters: new HttpParameterCollection 
            {
              { "path", file["path"] } 
            },
            authorization: this.Authorization,
            callback: (result) =>
            {
              if (result.StatusCode == 200)
              {
                var web = new WebClient();
                web.DownloadProgressChanged += DownloadProgressChanged;
                web.Headers.Add("Authorization", this.Authorization.ToString());
                web.DownloadFileAsync(new Uri(result["href"].ToString()), saveFileDialog1.FileName);
              }
              else
              {
                this.ShowErrorMessage(result);
              }
            }
          );
        }
      }

      if (this.CurrentPath.StartsWith("disk:", StringComparison.OrdinalIgnoreCase))
      {
        this.CurrentPath = this.CurrentPath.Substring("disk:".Length);
      }

      this.GetFiles();
    }

    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<object, DownloadProgressChangedEventArgs>(DownloadProgressChanged), sender, e);
        return;
      }

      progressBar1.Value = e.ProgressPercentage;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            
      var fileStream = openFileDialog1.OpenFile();

      this.progressBar1.Value = 0;
      this.CurrentFileLength = fileStream.Length;
      this.Cursor = Cursors.WaitCursor;

      OAuthUtility.GetAsync
      (
        endpoint: "https://cloud-api.yandex.net/v1/disk/resources/upload",
        parameters: new HttpParameterCollection 
        {
          { "path", Path.Combine(this.CurrentPath, Path.GetFileName(openFileDialog1.FileName)).Replace("\\", "/") } 
        },
        authorization: this.Authorization,
        callback: (result) =>
        {
          if (result.StatusCode == 200)
          {
            OAuthUtility.PutAsync
            (
              result["href"].ToString(),
              new HttpParameterCollection { {fileStream} },
              // handler of result
              callback: Upload_Result,
              // handler of uploading
              streamWriteCallback: Upload_Processing
            );
          }
          else
          {
            this.ShowErrorMessage(result);
          }
        }
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

      this.Cursor = Cursors.Default;

      if (result.StatusCode == 201)
      {
        this.GetFiles();
      }
      else
      {
        this.ShowErrorMessage(result);
      }
    }

    private void ShowErrorMessage(RequestResult result)
    {
      if (result["message"].HasValue)
      {
        MessageBox.Show(result["message"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      else
      {
        MessageBox.Show(result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

  }

}
// ----------------------------------------------------------------------------
// Copyright © Aleksey Nemiro, 2015. All rights reserved.
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
using Nemiro.OAuth.LoginForms;
using Nemiro.OAuth;

namespace Facebook.Net35
{

  public partial class Form1 : Form
  {

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
        this.GetLikes();
      }
    }

    private void GetAccessToken()
    {
      var login = new FacebookLogin("1435890426686808", "c6057dfae399beee9e8dc46a4182e8fd");
      login.Owner = this;
      login.ShowDialog();

      if (login.IsSuccessfully)
      {
        Properties.Settings.Default.AccessToken = login.AccessTokenValue;
        Properties.Settings.Default.Save();
        this.GetLikes();
      }
      else
      {
        MessageBox.Show("error...");
      }
    }

    private void GetLikes()
    {
      OAuthUtility.GetAsync
      (
        "https://graph.facebook.com/v2.2/me/likes",
        new HttpParameterCollection
        {
          { "access_token", Properties.Settings.Default.AccessToken }
        },
        callback: GetLikes_Result
      );
    }

    private void GetLikes_Result(RequestResult result)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<RequestResult>(GetLikes_Result), result);
        return;
      }

      if (result.StatusCode == 200)
      {

        listView1.Items.Clear();

        foreach (UniValue item in result["data"])
        {
          listView1.Items.Add(new ListViewItem(item["name"].ToString()));
        }

      }
      else
      {
        MessageBox.Show("Error...");
      }
    }

  }

}
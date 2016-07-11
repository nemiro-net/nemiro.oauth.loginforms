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
using System.Threading.Tasks;
using System.Windows.Forms;
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;

namespace Twitter.Net451
{

  public partial class Form1 : Form
  {

    private string ConsumerKey = "cXzSHLUy57C4gTBgMGRDuqQtr";
    private string ConsumerSecret = "3SSldiSb5H4XeEMOIIF4osPWxOy19jrveDcPHaWtHDQqgDYP9P";

    private string LastTweetId = null;
    
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
        this.GetTweets();
      }
    }

    private void GetAccessToken()
    {
      var login = new TwitterLogin(this.ConsumerKey, this.ConsumerSecret);
      login.Owner = this;
      login.ShowDialog();

      if (login.IsSuccessfully)
      {
        Properties.Settings.Default.AccessToken = login.AccessTokenValue;
        Properties.Settings.Default.TokenSecret = ((OAuthAccessToken)login.AccessToken).TokenSecret;
        Properties.Settings.Default.Save();
        this.GetTweets();
      }
      else
      {
        MessageBox.Show("error...");
      }
    }

    private void GetTweets()
    {
      this.Cursor = Cursors.WaitCursor;

      var parameters = new HttpParameterCollection();

      if (!String.IsNullOrEmpty(this.LastTweetId))
      {
        parameters.AddUrlParameter("max_id", this.LastTweetId);
      }

      OAuthUtility.GetAsync
      (
        "https://api.twitter.com/1.1/statuses/user_timeline.json",
        parameters: parameters,
        authorization: this.GetAuth(),
        callback: GetTweets_Result
      );
    }

    private void GetTweets_Result(RequestResult result)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<RequestResult>(GetTweets_Result), result);
        return;
      }

      this.Cursor = Cursors.Default;

      if (result.IsSuccessfully)
      {
        foreach (UniValue tweet in result)
        {
          if (!String.IsNullOrEmpty(textBox1.Text))
          {
            textBox1.Text += "\r\n\r\n--------------------------------------\r\n\r\n";
          }

          // tweet["user"]["name"], 
          textBox1.Text += String.Format("@{0} / {1}\r\n\r\n", tweet["user"]["screen_name"], DateTime.ParseExact(tweet["created_at"].ToString(), "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString());
          textBox1.Text += tweet["text"].ToString();

          this.LastTweetId = tweet["id_str"].ToString();
        }
      }
      else
      {
        MessageBox.Show(result.ToString());
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.GetTweets();
    }

    private OAuthAuthorization GetAuth()
    {
      var auth = new OAuthAuthorization();
      auth.ConsumerKey = this.ConsumerKey;
      auth.ConsumerSecret = this.ConsumerSecret;
      auth.SignatureMethod = SignatureMethods.HMACSHA1;
      auth.Token = Properties.Settings.Default.AccessToken;
      auth.TokenSecret = Properties.Settings.Default.TokenSecret;
      return auth;
    }


  }

}
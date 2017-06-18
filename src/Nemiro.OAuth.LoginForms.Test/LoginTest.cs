using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;

namespace Nemiro.OAuth.LoginForms
{
  
  public class LoginTest
  {

    private const string RETURN_URL = "https://oauthproxy.nemiro.net/";
    private const string RETURN_URL_HTTP = "http://oauthproxy.nemiro.net/";

    private const bool AUTO_LOGOUT = true;
    private const bool LOAD_USER_INFO = true;

    [Fact]
    public async void Amazon()
    {
      await this.RunSTATask(() => this.TestForm(new AmazonLogin("amzn1.application-oa2-client.f0ffe4edc256488dae00dcaf96d75d1b", "764dcefe49b441c8c6244c93e5d5d04de54fda6dfdc83da9693bf346f4dc4515", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void CodeProject()
    {
      await this.RunSTATask(() => this.TestForm(new CodeProjectLogin("92mWWELc2DjcL-6tu7L1Py6yllleqSCt", "YJXrk_Vzz4Ps02GqmaUY-aSLucxh4kfLq6oq0CtiukPfvbzb9yQG69NeDr2yiV9M", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Dropbox()
    {
      await this.RunSTATask(() => this.TestForm(new DropboxLogin("5nkunr8uscwfoba", "n7x9icfwoe6dehq", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Facebook()
    {
      await this.RunSTATask(() => this.TestForm(new FacebookLogin("1435890426686808", "c6057dfae399beee9e8dc46a4182e8fd", AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Foursquare()
    {
      await this.RunSTATask(() => this.TestForm(new FoursquareLogin("SNFCHOFGBEEJOSWU0TNPR2Q24VMHX3PIEVDBVJXLJURXJA5U", "XTTUIV5ZBGI14P3YHXXM3XWDGHSITPBGBGRUBAXE3G5C13D4", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void GitHub()
    {
      await this.RunSTATask(() => this.TestForm(new GitHubLogin("e14122695d88f5c95bce", "cde23ec001c5180e01e865f4efb57cb0bc848c16", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Google()
    {
      // NOTE: Keys only for web applications.
      // Other types is not supported.
      // this.TestForm(new GoogleLogin("1058655871432-83b9micke7cll89jfmcno5nftha3e95o.apps.googleusercontent.com", "AeEbEGQqoKgOZb41JUVLvEJL", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO));
      await this.RunSTATask(() => this.TestForm(new GoogleLogin("1058655871432-fscjqht7ou30a75gjkde1eu1brsvbqkn.apps.googleusercontent.com", "SI5bIZkrSB5rO03YF-CdsCJC", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Instagram()
    {
      await this.RunSTATask(() => this.TestForm(new InstagramLogin("9fcad1f7740b4b66ba9a0357eb9b7dda", "3f04cbf48f194739a10d4911c93dcece", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void LinkedIn()
    {
      await this.RunSTATask(() => this.TestForm(new LinkedInLogin("7890g34o1xof2y", "OJ7zIMPZtjbGQLg5", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void MicrosoftLive()
    {
      await this.RunSTATask(() => this.TestForm(new LiveLogin("000000004C1337C9", "7N3IwTyTGoGGimndiJAQiG2GBspOLyFZ", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void MailRu()
    {
      await this.RunSTATask(() => this.TestForm(new MailRuLogin("722701", "d0622d3d9c9efc69e4ca42aa173b938a", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Odnoklassniki()
    {
      await this.RunSTATask(() => this.TestForm(new OdnoklassnikiLogin("1094959360", "E45991423E8C5AE249B44E84", "CBACMEECEBABABABA", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void SoundCloud()
    {
      await this.RunSTATask(() => this.TestForm(new SoundCloudLogin("039ff9ae960f6654d0fc73f1035e5d03", "14b498d3fc0fd4d1dadf1a4169d47e21", RETURN_URL_HTTP, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Tumblr()
    {
      await this.RunSTATask(() => this.TestForm(new TumblrLogin("2EZbsj2oF8OAouPlDWSVnESetAchImzPLV4q0IcQH7DGKECuzJ", "4WZ3HBDwNuz5ZDZY8qyK1qA5QFHEJY7gkPK6ooYFCN4yw6crKd", AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Twitter()
    {
      await this.RunSTATask(() => this.TestForm(new TwitterLogin("cXzSHLUy57C4gTBgMGRDuqQtr", "3SSldiSb5H4XeEMOIIF4osPWxOy19jrveDcPHaWtHDQqgDYP9P", AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Vkontakte()
    {
      await this.RunSTATask(() => this.TestForm(new VkontakteLogin("4457505", "wW5lFMVbsw0XwYFgCGG0", AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Yahoo()
    {
      await this.RunSTATask(() => this.TestForm(new YahooLogin("dj0yJmk9WUlxZmpUcTg4YXZZJmQ9WVdrOVIxaHJiRU5YTmpJbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD02Mg--", "0dca4c079cc277df6e9b3fd2b7b940d34fa0023f", RETURN_URL, AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    [Fact]
    public async void Yandex()
    {
      await this.RunSTATask(() => this.TestForm(new YandexLogin("0ee5f0bf2cd141a1b194a2b71b0332ce", "59d76f7c09b54ad38e6b15f792da7a9a", AUTO_LOGOUT, LOAD_USER_INFO)));
    }

    private void TestForm(Login form)
    {
      form.Load += (sender, e) => (sender as Login).Visible = true;

      var r = form.ShowDialog();

      Assert.True(r == DialogResult.OK);
      Assert.True(form.IsSuccessfully);

      if (form.IsSuccessfully)
      {
        Assert.True(!string.IsNullOrEmpty(form.AccessTokenValue));
        
        if (LOAD_USER_INFO)
        {
          Assert.True(form.UserInfo != null);
        }
      }
    }

    private Task RunSTATask(Action action)
    {
      var tcs = new TaskCompletionSource<object>();
      var thread = new Thread(() =>
      {
        try
        {
          action();
          tcs.SetResult(new object());
        }
        catch (Exception e)
        {
          tcs.SetException(e);
        }
      });

      thread.SetApartmentState(ApartmentState.STA);

      thread.Start();

      return tcs.Task;
    }

  }

}
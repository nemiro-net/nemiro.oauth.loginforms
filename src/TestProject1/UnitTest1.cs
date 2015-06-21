using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;
using System.Windows.Forms;
using Nemiro.OAuth.Clients;

namespace TestProject1
{

  [TestClass]
  public class UnitTest1
  {

    public UnitTest1()
    {
    }

    private TestContext testContextInstance;

    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    [TestMethod]
    public void Amazon()
    {
      this.TestForm(new AmazonLogin("amzn1.application-oa2-client.f0ffe4edc256488dae00dcaf96d75d1b", "764dcefe49b441c8c6244c93e5d5d04de54fda6dfdc83da9693bf346f4dc4515", "https://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void CodeProject()
    {
      this.TestForm(new CodeProjectLogin("92mWWELc2DjcL-6tu7L1Py6yllleqSCt", "YJXrk_Vzz4Ps02GqmaUY-aSLucxh4kfLq6oq0CtiukPfvbzb9yQG69NeDr2yiV9M", "https://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void Dropbox()
    {
      this.TestForm(new DropboxLogin("5nkunr8uscwfoba", "n7x9icfwoe6dehq"));
    }
    
    [TestMethod]
    public void Facebook()
    {
      this.TestForm(new FacebookLogin("1435890426686808", "c6057dfae399beee9e8dc46a4182e8fd"));
    }
    
    [TestMethod]
    public void Foursquare()
    {
      this.TestForm(new FoursquareLogin("SNFCHOFGBEEJOSWU0TNPR2Q24VMHX3PIEVDBVJXLJURXJA5U", "XTTUIV5ZBGI14P3YHXXM3XWDGHSITPBGBGRUBAXE3G5C13D4", "http://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void GitHub()
    {
      this.TestForm(new GitHubLogin("e14122695d88f5c95bce", "cde23ec001c5180e01e865f4efb57cb0bc848c16"));
    }

    [TestMethod]
    public void Google()
    {
      this.TestForm(new GoogleLogin("1058655871432-83b9micke7cll89jfmcno5nftha3e95o.apps.googleusercontent.com", "AeEbEGQqoKgOZb41JUVLvEJL"));
    }

    [TestMethod]
    public void Instagram()
    {
      this.TestForm(new InstagramLogin("9fcad1f7740b4b66ba9a0357eb9b7dda", "3f04cbf48f194739a10d4911c93dcece", "http://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void LinkedIn()
    {
      this.TestForm(new LinkedInLogin("7890g34o1xof2y", "OJ7zIMPZtjbGQLg5", "http://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void MicrosoftLive()
    {
      this.TestForm(new LiveLogin("000000004C1337C9", "7N3IwTyTGoGGimndiJAQiG2GBspOLyFZ", "http://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void MailRu()
    {
      this.TestForm(new MailRuLogin("722701", "d0622d3d9c9efc69e4ca42aa173b938a"));
    }

    [TestMethod]
    public void Odnoklassniki()
    {
      this.TestForm(new OdnoklassnikiLogin("1094959360", "E45991423E8C5AE249B44E84", "CBACMEECEBABABABA", "http://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void SoundCloud()
    {
      this.TestForm(new SoundCloudLogin("039ff9ae960f6654d0fc73f1035e5d03", "14b498d3fc0fd4d1dadf1a4169d47e21", "http://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void Tumblr()
    {
      this.TestForm(new TumblrLogin("2EZbsj2oF8OAouPlDWSVnESetAchImzPLV4q0IcQH7DGKECuzJ", "4WZ3HBDwNuz5ZDZY8qyK1qA5QFHEJY7gkPK6ooYFCN4yw6crKd"));
    }

    [TestMethod]
    public void Twitter()
    {
      this.TestForm(new TwitterLogin("cXzSHLUy57C4gTBgMGRDuqQtr", "3SSldiSb5H4XeEMOIIF4osPWxOy19jrveDcPHaWtHDQqgDYP9P"));
    }

    [TestMethod]
    public void Vkontakte()
    {
      this.TestForm(new VkontakteLogin("4457505", "wW5lFMVbsw0XwYFgCGG0"));
    }

    [TestMethod]
    public void Yahoo()
    {
      this.TestForm(new YahooLogin("dj0yJmk9WUlxZmpUcTg4YXZZJmQ9WVdrOVIxaHJiRU5YTmpJbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD02Mg--", "0dca4c079cc277df6e9b3fd2b7b940d34fa0023f", "http://oauthproxy.nemiro.net/"));
    }

    [TestMethod]
    public void Yandex()
    {
      this.TestForm(new YandexLogin("0ee5f0bf2cd141a1b194a2b71b0332ce", "59d76f7c09b54ad38e6b15f792da7a9a"));
    }

    private void TestForm(Login form)
    {
      var r = form.ShowDialog();
      Console.WriteLine(r);
      Console.WriteLine(form.IsSuccessfully);
      if (form.IsSuccessfully)
      {
        Console.WriteLine(form.AccessTokenValue);
        if (String.IsNullOrEmpty(form.AccessTokenValue))
        {
          Assert.Fail();
        }
      }
      else
      {
        Assert.Fail();
      }
    }

  }

}
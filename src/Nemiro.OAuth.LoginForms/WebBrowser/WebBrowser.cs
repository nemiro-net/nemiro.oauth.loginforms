using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Nemiro.OAuth.LoginForms
{

  internal class WebBrowser : System.Windows.Forms.WebBrowser
  {

    public static Guid IID_IHttpSecurity = new Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b");
    public static Guid IID_IWindowForBindingUI = new Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b");

    public const int S_OK = 0;
    public const int S_FALSE = 1;
    public const int E_NOINTERFACE = unchecked((int)0x80004002);
    public const int RPC_E_RETRY = unchecked((int)0x80010109);

    protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
    {
      return new WebBrowserSite(this);
    }
    
    new class WebBrowserSite : System.Windows.Forms.WebBrowser.WebBrowserSite, UCOMIServiceProvider, IHttpSecurity, IWindowForBindingUI
    {

      private Nemiro.OAuth.LoginForms.WebBrowser _WebBrowser = null;

      public WebBrowserSite(Nemiro.OAuth.LoginForms.WebBrowser myWebBrowser)
        : base(myWebBrowser)
      {
        this._WebBrowser = myWebBrowser;
      }

      public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
      {

        if (riid == IID_IHttpSecurity)
        {
          ppvObject = Marshal.GetComInterfaceForObject(this, typeof(IHttpSecurity));
          return S_OK;
        }

        if (riid == IID_IWindowForBindingUI)
        {
          ppvObject = Marshal.GetComInterfaceForObject(this, typeof(IWindowForBindingUI));
          return S_OK;
        }

        /*if (riid == IID_IInternetProtocol)
        {
          ppvObject = Marshal.GetComInterfaceForObject(this, typeof(IInternetProtocol));
          return S_OK;
        }*/

        ppvObject = IntPtr.Zero;

        return E_NOINTERFACE;
      }

      public int GetWindow(ref Guid rguidReason, ref IntPtr phwnd)
      {
        if (rguidReason == IID_IHttpSecurity || rguidReason == IID_IWindowForBindingUI)
        {
          phwnd = _WebBrowser.Handle;
          return S_OK;
        }
        else
        {
          phwnd = IntPtr.Zero;
          return S_FALSE;
        }
      }

      public int OnSecurityProblem(uint dwProblem)
      {
        // ignore ssl errors
        return S_OK;
      }

    }

    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface UCOMIServiceProvider
    {

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int QueryService([In] ref Guid guidService, [In] ref Guid riid, [Out] out IntPtr ppvObject);

    }

    [ComImport()]
    [ComVisible(true)]
    [Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWindowForBindingUI
    {

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int GetWindow([In] ref Guid rguidReason, [In, Out] ref IntPtr phwnd);

    }

    [ComImport()]
    [ComVisible(true)]
    [Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IHttpSecurity
    {

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int GetWindow([In] ref Guid rguidReason, [In, Out] ref IntPtr phwnd);

      [PreserveSig]
      int OnSecurityProblem([In, MarshalAs(UnmanagedType.U4)] uint dwProblem);

    }

  }

}
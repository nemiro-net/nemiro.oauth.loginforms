using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nemiro.OAuth.LoginForms
{

  /// <summary>
  /// Browser Emulation
  /// </summary>
  /// <remarks>
  /// <para><see href="https://msdn.microsoft.com/en-us/library/ee330730(v=vs.85).aspx"/></para>
  /// </remarks>
  internal enum BrowserEmulationVersion
  {
    Default = 0,
    /// <summary>
    /// Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.
    /// </summary>
    IE7 = 0x1B58,
    /// <summary>
    /// Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8.
    /// </summary>
    IE8 = 0x1F40,
    /// <summary>
    /// Webpages are displayed in IE8 Standards mode, regardless of the declared !DOCTYPE directive. Failing to declare a !DOCTYPE directive causes the page to load in Quirks.
    /// </summary>
    IE8Standards = 0x22B8,
    /// <summary>
    /// Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
    /// </summary>
    IE9 = 0x2328,
    /// <summary>
    /// Windows Internet Explorer 9. Webpages are displayed in IE9 Standards mode, regardless of the declared !DOCTYPE directive. Failing to declare a !DOCTYPE directive causes the page to load in Quirks.
    /// </summary>
    IE9Standards = 0x270F,
    /// <summary>
    /// Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode. Default value for Internet Explorer 10.
    /// </summary>
    IE10 = 0x2710,
    /// <summary>
    /// Internet Explorer 10. Webpages are displayed in IE10 Standards mode, regardless of the !DOCTYPE directive.
    /// </summary>
    IE10Standards = 0x2711,
    /// <summary>
    /// IE11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 edge mode. Default value for IE11.
    /// </summary>
    IE11 = 0x2AF8,
    /// <summary>
    /// Internet Explorer 11. Webpages are displayed in IE11 edge mode, regardless of the declared !DOCTYPE directive. Failing to declare a !DOCTYPE directive causes the page to load in Quirks.
    /// </summary>
    IE11Edge = 0x2AF9
  }

}
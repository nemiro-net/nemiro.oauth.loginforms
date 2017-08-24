# Change Log

All notable changes to **Nemiro.OAuth.LoginForms** will be documented in this file.

## [v1.7] - 2017-08-25

In this release, the authentication logic has changed. 

<details>
  <summary>Now the access token requests are executed, not the authorization code.</summary>
  <p>For most forms, the <code>returnUrl</code> parameter in the constructor is now required.</p>
  <p>You may need to obtain new keys to perform authentication.<br />
  For example, for Google, you need to create a key for web applications, instead of standalone.</p>
  <p>The old behavior can be returned by specifying a <code>responseType</code> in the designer with the value <code>ResponseType.Code</code>:</p>
  <div class="highlight highlight-source-cs"><pre>
  var login = new GoogleLogin
  (
    "934704666049-129jsvmelksmcmf250ir90aqn8pk4nak.apps.googleusercontent.com", 
    "OS7HZ1cfJnhdIFZ6fUsgamH-",
    returnUrl: null, 
    scope: "https://www.googleapis.com/auth/drive", 
    loadUserInfo: true, 
    responseType: ResponseType.Code
  );
  </pre></div>
</details>

### Added

* Support `ResponseType.Token`.

### Changed

* Now `ReturnUrl` is required for all forms.

### Thanks

* [Visio70](https://github.com/Visio70)
* [santoshpasi](https://github.com/santoshpasi)

## [v1.6] - 2017-01-15

### Added

* Additional method to Cookie cleaning.

### Changed

* Size of forms: Foursquare, GitHub, Google, VK, Yahoo and Yandex;
* `ILoginForm` to public  ([#6](https://github.com/alekseynemiro/Nemiro.OAuth.LoginForms/issues/6));
* Version number: excluded version of .NET Framework ([#19](https://github.com/alekseynemiro/nemiro.oauth.dll/issues/19)).

### Fixed

* Dropbox logout ([#7](https://github.com/alekseynemiro/Nemiro.OAuth.LoginForms/issues/7)).

### Thanks

* [ed-miller](https://github.com/ed-miller)
* [austin-----](https://github.com/austin-----)
* [Michael Collins](https://github.com/mfcollins3)

## [v1.5] - 2016-09-12

### Added

* Added ability to get detailed information about users.

### Changed

* Changed modifier of the `Client` property. Now it is a public property;
* Updated references to [Nemiro.OAuth](https://github.com/alekseynemiro/nemiro.oauth.dll).

## [v1.4.2483] - 2016-08-08

### Changed

* Updated references to [Nemiro.OAuth](https://github.com/alekseynemiro/nemiro.oauth.dll).

## [v1.4.2423] - 2016-07-11

### Added

* Strong name.

### Changed

* Updated references to [Nemiro.OAuth](https://github.com/alekseynemiro/nemiro.oauth.dll).

## [v1.4] - 2015-12-07

### Added

* Logout.

### Fixed

* Dropbox.

## [v1.3] - 2015-06-25

### Added

* Support for **.NET 3.5**.

### Changed

* Updated references to [Nemiro.OAuth](https://github.com/alekseynemiro/nemiro.oauth.dll).

## [v1.2] - 2015-06-21

### Added

* Added lock on re-obtain an access token (fixed problem with **Dropbox** form);
* Automatic switching **WebBrowser** to latest version **IE**.

## [v1.1] - 2015-02-11

### Added

* `AccessTokenValue` property;
* Form for **CodeProject**.

## [v1.0] - 2015-01-04

* First version released.
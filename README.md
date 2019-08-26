# Global Microsoft 365 Developer Bootcamp 2019 Japan

[Global Microsoft 365 Developer Bootcamp 2019 Japan](https://connpass.com/event/144707) の Adaptive Cards のハンズオンの資料です。

## ハンズオン資料

[ダウンロード](https://1drv.ms/p/s!Ao3RAkrDcoDMhKc1csONcxZdYfNUXQ?e=GEzpUg) はこちらから。

## 事前準備

ハンズオンを受講するにあたり以下のソフトウェアをインストールする必要があります。

- [.NET Core 2.2 SDK](https://www.microsoft.com/net/download)
- [Visual Studio Code](https://code.visualstudio.com)
  - 拡張機能: C#
  - 拡張機能: Azure Functions
- [Git](https://git-scm.com/downloads)

コンピューターは Windows または Mac をご利用いただけます。

## プロジェクト

### AdaptiveCardsSend

Adaptive Cards を Outlook に送信するためのサンプル プロジェクト (ASP.NET Core MVC アプリケーション) です。画面から受け取ったペイロード (JSON) を Microsoft Graph を使って送信します。

### AdaptiveCardsAction

Outlook Actionable Messages のアクションを受け取るためのサンプル プロジェクト (Azure Functions アプリケーション) です。Adaptive Cards から受け取ったコンテンツを Microsoft Graph を使って OneDrive for Bussiness に置かれた Excel ファイルに書き込みます。

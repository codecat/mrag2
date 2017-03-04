using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Mrag2
{
  /// <summary>
  /// Language information, get this from DetectLanguage.
  /// </summary>
  public class Language
  {
    /// <summary>
    /// Whether it was found.
    /// </summary>
    public bool Found = false;
    /// <summary>
    /// Whether it was reliable enough.
    /// </summary>
    public bool Reliable = false;
    /// <summary>
    /// Language code.
    /// </summary>
    public string Code = "en";
  }

  /// <summary>
  /// Language detection methods, uses DetectLanguage.com's API. You need an API key for this.
  /// </summary>
  public class DetectLanguage
  {
    /// <summary>
    /// Your API key.
    /// </summary>
    public string dl_strAPIKey;
    /// <summary>
    /// The URL to query.
    /// </summary>
    public string dl_strURL = "http://" + "ws.detectlanguage.com/0.2/detect?q={0}&key={1}";

    /// <summary>
    /// Create a new DetectLanguage object for detectlanguage.com API interaction.
    /// </summary>
    /// <param name="strAPIKey"></param>
    public DetectLanguage(string strAPIKey)
    {
      dl_strAPIKey = strAPIKey;
    }

    /// <summary>
    /// Detect the language based on the given text. It's recommended to only give sentences with multiple words, not single words.
    /// </summary>
    /// <param name="strText">The text to test the language for.</param>
    /// <returns>A Language object with language and reliability info.</returns>
    public Language Detect(string strText)
    {
      WebClient wc = new WebClient();
      wc.Proxy = null;
      string strRequest = String.Format(dl_strURL, Uri.EscapeDataString(strText), dl_strAPIKey);
      dynamic res = JSON.JsonDecode(wc.DownloadString(strRequest));

      // if there are detections
      if (res["data"]["detections"].Count >= 1) {
        dynamic l = res["data"]["detections"][0];

        // construct a proper response
        Language ret = new Language();
        ret.Found = true;
        ret.Code = l["language"];
        ret.Reliable = l["isReliable"];

        // return language
        return ret;
      }

      return new Language();
    }
  }
}

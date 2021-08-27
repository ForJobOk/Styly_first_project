using System.Collections.Generic;
using System;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// 不正なURLにリクエストしたときに投げる例外
    /// </summary>
    public class UrlValidationException : Exception
    {
        public UrlValidationException(Uri uri) : base("Cannot Post to Url:" + uri.ToString())
        {

        }
    }

    /// <summary>
    /// UrlValidatorでリクエスト先を制限したHttpRequester実装
    /// </summary>
    public class RestrictHttpRequester : IHttpRequester
    {
        readonly IHttpRequester httpRequester;
        readonly IUrlValidator urlValidator;

        public RestrictHttpRequester(IHttpRequester httpRequester, IUrlValidator urlValidator)
        {
            this.httpRequester = httpRequester;
            this.urlValidator = urlValidator;
        }

        public void Get(Uri uri, Dictionary<string, string> headers, Action<HttpResponse> action)
        {
            if (!this.urlValidator.isValid(uri))
            {
                throw new UrlValidationException(uri);
            }

            this.httpRequester.Get(uri, headers, action);
        }

        public void Post(Uri uri, Dictionary<string, string> headers, string body, Action<HttpResponse> action)
        {
            if (!this.urlValidator.isValid(uri))
            {
                throw new UrlValidationException(uri);
            }

            this.httpRequester.Post(uri, headers, body, action);
        }

        public void Put(Uri uri, Dictionary<string, string> headers, string body, Action<HttpResponse> action)
        {
            if (!this.urlValidator.isValid(uri))
            {
                throw new UrlValidationException(uri);
            }

            this.httpRequester.Put(uri, headers, body, action);
        }

        public void Delete(Uri uri, Dictionary<string, string> headers, string body, Action<HttpResponse> action)
        {
            if (!this.urlValidator.isValid(uri))
            {
                throw new UrlValidationException(uri);
            }
            this.httpRequester.Delete(uri, headers, body, action);
        }

        public void Dispose()
        {
            httpRequester.Dispose();
        }
    }
}
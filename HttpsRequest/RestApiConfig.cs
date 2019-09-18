using System;
using System.Collections.Generic;
using System.Text;

namespace HttpsRequest
{
    public class RestApiConfig
    {
        public const string Accpet = "application/json";
        public const string ContentType = "application/json; charset=utf-8"; //application/x-www-form-urlencoded, application/png
        public const string Referer = ""; //전송 전 참조하던 리소스 혹은 주소
        public const string CacheControl = "";

        //https://developer.mozilla.org/ko/docs/Web/HTTP/Methods
        public enum MethodTypes
        {
            NONE = 0,
            GET = 1,
            POST = 2, //검증 없이 게시
            PUT = 3, //전체 수정
            PATHCH = 4,  //부분 수정
            DELETE = 5,
        }

        //https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
        public enum ResponseTypes
        {
            NONE = 0,
            CONVERT_FAILED = 10,
            CONTINUE = 100,
            SWITCHING_PROTOCOLS = 101,
            SUCCESS = 200,
            BAD_REQUEST = 400,
            UNAUTHORIZED = 401,
            NOT_FOUND = 404,
        } 
    }
}

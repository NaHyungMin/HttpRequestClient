using System;
using System.Net;

namespace HttpsRequest
{
    class Program
    {
        static void Main(string[] args)
        {
            WebRequester requester = new WebRequester();
            requester.Request("https://www.naver.com", string.Empty, RestApiConfig.MethodTypes.POST, Response, ErrorResponse);
        }

        public static void Response(string send, string receive)
        {

        }

        public static void ErrorResponse(int errorCode, string message)
        {

        }
    }
}
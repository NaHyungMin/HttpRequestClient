using System;
using System.IO;
using System.Net;
using System.Text;
using static HttpsRequest.WebRequester;

namespace HttpsRequest
{
    public class RequestState
    {
        // This class stores the State of the request.
        public string SendString { get; set; }
        public HttpWebRequest Request { get; set; }
        public ResponseCallback ResponseCallback { get; set; }
        public ErrorCallback ErrorCallback { get; set; }
        public HttpWebResponse Response { get; set; }
        public Stream StreamResponse { get; set; }

        public RequestState()
        {
        }
    }
}

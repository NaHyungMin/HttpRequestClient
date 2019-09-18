using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using HttpsRequest.SharpZipLib;

namespace HttpsRequest
{
    public class WebRequester
    {
        private object lockObject = new object();
        private HttpWebRequest request;
        private bool isWorking;
        private bool isSuccess;
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private readonly int timeOut = 0; //1 * 60 * 1000; //default 1 minutes

        public delegate void ResponseCallback(string reqeustString, string responseString);
        public delegate void ErrorCallback(int errorCode, string errorString);

        public WebRequester(int timeOut = (1 * 60 * 1000))
        {
            this.timeOut = timeOut;
        }

        private void TimeOutCallback(object state, bool timeOut)
        {
            lock(lockObject)
            {
                if (timeOut == true)
                {
                    if (state is HttpWebRequest request)
                        request.Abort();
                }
            }
        }

        public bool Request(string url, string sendString, RestApiConfig.MethodTypes sendType, ResponseCallback responseCallback, ErrorCallback errorCallback)
        {
            lock(lockObject)
            {
                if (sendType == RestApiConfig.MethodTypes.NONE)
                    return false; //Error 처리.

                if (isWorking == true)
                    return false;
                
                try
                {
                    request = WebRequest.CreateHttp(url);
                    request.Timeout = timeOut;
                    request.ProtocolVersion = HttpVersion.Version11;
                    request.Method = sendType.ToString();
                    request.Headers.Add(HttpRequestHeader.ContentType, RestApiConfig.ContentType);
                    request.Headers.Add(HttpRequestHeader.Accept, RestApiConfig.Accpet);
                    //암호화 정책은 따로 넣어야 한다.
                    //request.Headers.Add(HttpRequestHeader.Allow, )

                    RequestState requestState = new RequestState
                    {
                        Request = request,
                        SendString = sendString,
                        ResponseCallback = responseCallback,
                        ErrorCallback = errorCallback,
                    };

                    request.BeginGetRequestStream(new AsyncCallback(RequestStreamCallback), requestState);
                  
                    isWorking = true;
                    allDone.WaitOne();
                }
                catch (WebException exception)
                {
                    isWorking = false;
                    errorCallback(Convert.ToInt32(exception.Status), exception.Message);
                }
                catch (Exception exception)
                {
                    isWorking = false;
                    //error message를 따로 찍는다.
                    //클라이언트가 이걸 어떻게 쓸지는 결정해야 함.
                }
            }

            return true;
        }
        
        private void RequestStreamCallback(IAsyncResult asyncResult)
        {
            RequestState requestState = (RequestState)asyncResult.AsyncState;

            using (Stream stream = requestState.Request.EndGetRequestStream(asyncResult))
            {
                string postData = SharpZip.StringToStringZip(requestState.SendString);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                stream.Write(byteArray, 0, postData.Length);
                stream.Close();
                requestState.Request.BeginGetResponse(new AsyncCallback(GetResponseCallback), requestState);
            }
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            RequestState requestState = (RequestState)asynchronousResult.AsyncState;
            HttpWebRequest request = requestState.Request;//(HttpWebRequest)asynchronousResult.AsyncState;
            
            using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult))
            using (Stream stream = response.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            {
                string responseString = streamReader.ReadToEnd();
                string responseData = "";
                try
                {
                    responseData = SharpZip.StringZipToString(responseString);
                }
                catch (Exception exception)
                {
                    //Convert Error
                    requestState.ErrorCallback(Convert.ToInt32(RestApiConfig.ResponseTypes.CONVERT_FAILED), exception.Message);
                }
                
                // Close the stream object
                stream.Close();
                streamReader.Close();

                // Release the HttpWebResponse
                response.Close();
                allDone.Set();
                isSuccess = true;
                Abort();

                //Callback
                requestState.ResponseCallback(requestState.SendString, responseData);
            }
        }

        private bool Abort()
        {
            lock(lockObject)
            {
                if(isWorking == true)
                {
                    isWorking = false;
                    request.Abort();
                    return true;
                }

                return false;
            }
        }
    }
}

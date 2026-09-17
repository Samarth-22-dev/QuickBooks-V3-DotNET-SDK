using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Intuit.Ipp.Core.Rest;
using Intuit.Ipp.Data;
using Intuit.Ipp.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Intuit.Ipp.Core.Test;
using Intuit.Ipp.Exception;
//using Intuit.Ipp.Retry;
using Intuit.Ipp.Core.Test.Common;
using Intuit.Ipp.Utility;

namespace Intuit.Ipp.Core.Test
{
    /// <summary>
    ///This is a test class for SyncRestHandlerTest and is intended
    ///to contain all SyncRestHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SyncRestHandlerTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize]
        public static void SyncRestHandlerInitialize(TestContext testContext)
        {
        }

        ///// <summary>
        ///// Private constructor test
        ///// </summary>
        //[TestMethod]
        //public void SyncRestHandlerEmptyConstructorTest()
        //{
        //    SyncRestHandler_Accessor actual = new SyncRestHandler_Accessor();
        //}

        

        /// <summary>
        /// Prepare request with updated QBO BaseURL.
        /// </summary>
        [TestMethod]
        public void SyncRestHandlerPrepareRequestUpdateBaseURLQBOTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            SyncRestHandler handler = new SyncRestHandler(serviceContext);
            string resourceUri = string.Format("v3/company/{0}/customer", serviceContext.RealmId);
            serviceContext.IppConfiguration.BaseUrl.Qbo = "http://www.intuit.com/";
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.POST, CoreConstants.CONTENTTYPE_APPLICATIONJSON);
            HttpWebRequest request = handler.PrepareRequest(parameters, new Intuit.Ipp.Data.Customer());
            string endpointUri = string.Format("{0}{1}", "http://www.intuit.com/", resourceUri);

            Assert.AreEqual(endpointUri.Trim(), request.RequestUri.ToString().Replace(request.RequestUri.Query, "").Trim());
            Assert.AreEqual(parameters.Verb.ToString(), request.Method.ToString());
            Assert.AreEqual(parameters.ContentType, request.ContentType);
        }

        /// <summary>
        /// Test case for get response
        /// </summary>
        [TestMethod]
        public void GetResponseSuccessTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            IRestHandler handler = new SyncRestHandler(serviceContext);
            string AccountId = "1";
            string resourceUri = string.Format("v3/company/{0}/account/{1}", serviceContext.RealmId, AccountId);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_TEXTXML);
            HttpWebRequest request = handler.PrepareRequest(parameters, null);
            string response = handler.GetResponse(request);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response));
        }

        /// <summary>
        /// Test case for Get response with Retry policy
        /// </summary>
        [TestMethod]
        public void GetResponseWithRetrySuccessTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            serviceContext.IppConfiguration.RetryPolicy = new IntuitRetryPolicy(3, TimeSpan.FromSeconds(2));
            IRestHandler handler = new SyncRestHandler(serviceContext);
            string AccountId = "1";
            string resourceUri = string.Format("v3/company/{0}/account/{1}", serviceContext.RealmId, AccountId);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_APPLICATIONXML);
            HttpWebRequest request = handler.PrepareRequest(parameters, null);
            string response = handler.GetResponse(request);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response));
        }

       
        /// <summary>
        /// Generates status code 401
        /// </summary>
        [TestMethod][Ignore]//no exception for oauth2 as we try to refresh token everytime
        [ExpectedException(typeof(InvalidTokenException))]
        public void GetResponseInvalidTokenExceptionTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            IRestHandler handler = new SyncRestHandler(serviceContext);
            string resourceUri = string.Format("v3/company/{0}/customer", AuthorizationKeysQBO.realmIdIAQBO);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.POST, CoreConstants.CONTENTTYPE_APPLICATIONXML);
            Intuit.Ipp.Data.Customer customer = new Data.Customer();
            HttpWebRequest request = handler.PrepareRequest(parameters, customer);
            string response = handler.GetResponse(request);
        }

        

        /// <summary>
        /// Test case for SyncRestHandler Timeout=10 expected Timeout 
        /// </summary>
        [TestMethod]
        public void GetResponseTimeoutTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            //Set timeout 10 milliseconds
            serviceContext.Timeout = 10;
            IRestHandler handler = new SyncRestHandler(serviceContext);
            string AccountId = "1";
            string resourceUri = string.Format("v3/company/{0}/account/{1}", serviceContext.RealmId, AccountId);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_TEXTXML);
            HttpWebRequest request = handler.PrepareRequest(parameters, null);
            string response = handler.GetResponse(request);
            Assert.IsTrue(string.IsNullOrWhiteSpace(response));
        }

        /// <summary>
        /// Test case for SyncRestHandler Timeout=200 seconds. No timeout
        /// </summary>
        [TestMethod]
        public void GetResponseTimeoutNoTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            //Set timeout 200 seconds
            serviceContext.Timeout = 200000; 
            IRestHandler handler = new SyncRestHandler(serviceContext);
            string AccountId = "1";
            string resourceUri = string.Format("v3/company/{0}/account/{1}", serviceContext.RealmId, AccountId);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_TEXTXML);
            HttpWebRequest request = handler.PrepareRequest(parameters, null);
            string response = handler.GetResponse(request);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response));
        }

        [TestMethod]
        public void GetResponseStreamSuccessTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            IRestHandler handler = new SyncRestHandler(serviceContext);
            List<SalesReceipt> salesReceipts = Helper.FindAll<SalesReceipt>(serviceContext, new SalesReceipt());
            Assert.IsTrue(salesReceipts.Count > 0);
            string resourceUri = string.Format("v3/company/{0}/salesreceipt/{1}/pdf", serviceContext.RealmId, salesReceipts[0].Id);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_APPLICATIONXML);
            HttpWebRequest request = handler.PrepareRequest(parameters, null,includeRequestId: false);
            request.Accept = CoreConstants.CONTENTTYPE_APPLICATIONPDF;
            byte[] response = handler.GetResponseStream(request);
            Assert.IsTrue(response.Length > 0);
        }

        [TestMethod]
        public void GetResponseStreamCompressedSuccessTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            serviceContext.IppConfiguration.Message.Response.CompressionFormat = Intuit.Ipp.Core.Configuration.CompressionFormat.GZip;
            IRestHandler handler = new SyncRestHandler(serviceContext);
            List<SalesReceipt> salesReceipts = Helper.FindAll<SalesReceipt>(serviceContext, new SalesReceipt());
            Assert.IsTrue(salesReceipts.Count > 0);
            string resourceUri = string.Format("v3/company/{0}/salesreceipt/{1}/pdf", serviceContext.RealmId, salesReceipts[0].Id);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_APPLICATIONXML);
            HttpWebRequest request = handler.PrepareRequest(parameters, null, includeRequestId: false);
            request.Accept = CoreConstants.CONTENTTYPE_APPLICATIONPDF;
            byte[] response = handler.GetResponseStream(request);
            Assert.IsTrue(response.Length > 0);
        }

        [TestMethod]
        public void GetResponseStreamRetrySuccessTest()
        {
            ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
            serviceContext.IppConfiguration.RetryPolicy = new IntuitRetryPolicy(3, new TimeSpan(0, 0, 5));
            IRestHandler handler = new SyncRestHandler(serviceContext);
            List<SalesReceipt> salesReceipts = Helper.FindAll<SalesReceipt>(serviceContext, new SalesReceipt());
            Assert.IsTrue(salesReceipts.Count > 0);
            string resourceUri = string.Format("v3/company/{0}/salesreceipt/{1}/pdf", serviceContext.RealmId, salesReceipts[0].Id);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_APPLICATIONXML);
            HttpWebRequest request = handler.PrepareRequest(parameters, null, includeRequestId: false);
            request.Accept = CoreConstants.CONTENTTYPE_APPLICATIONPDF;
            byte[] response = handler.GetResponseStream(request);
            Assert.IsTrue(response.Length > 0);
        }

        [TestMethod]
        public void GetResponseStreamFailureTest()
        {
            try
            {
                ServiceContext serviceContext = Initializer.InitializeServiceContextQbo();
                IRestHandler handler = new SyncRestHandler(serviceContext);
                string AccountId = "1";
                string resourceUri = string.Format("v3/company/{0}/salesreceipt/3/", serviceContext.RealmId);
                RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_APPLICATIONXML);
                HttpWebRequest request = handler.PrepareRequest(parameters, null, includeRequestId: false);
                request.Accept = CoreConstants.CONTENTTYPE_APPLICATIONPDF;
                byte[] response = handler.GetResponseStream(request);
                Assert.IsTrue(response.Length > 0);
            }
            catch (IdsException idsEx)
            {
                Assert.IsNotNull(idsEx);
                
            }
            
        }

        /// <summary>
        /// A fault in the body of an HTTP 200 response throws an exception carrying the intuit_tid response header.
        /// </summary>
        [TestMethod]
        public void GetResponseFaultIntuitTidTest()
        {
            string intuitTid = "1-5f3c9a2b-0e1d4c8a9b7f6e5d";
            string faultResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><IntuitResponse xmlns=\"http://schema.intuit.com/finance/v3\" time=\"2016-06-01T10:03:08+00:00\"><Fault type=\"ValidationFault\"><Error code=\"2050\" element=\"firstname\"><Message>Length exceeds limit</Message><Detail>Length of the field exceeds 21 chars</Detail></Error></Fault></IntuitResponse>";
            string baseUrl = string.Format("http://localhost:{0}/", GetFreePort());

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(baseUrl);
            listener.Start();
            try
            {
                // The first call is answered with the intuit_tid header, the second one without it.
                System.Threading.Tasks.Task.Run(() =>
                {
                    RespondWithFault(listener, faultResponse, intuitTid);
                    RespondWithFault(listener, faultResponse, null);
                });

                ServiceContext serviceContext = new ServiceContext("1234567890", IntuitServicesType.QBO, new OAuth2RequestValidator("bearertoken"));
                serviceContext.IppConfiguration.BaseUrl.Qbo = baseUrl;
                serviceContext.IppConfiguration.Message.Response.SerializationFormat = Intuit.Ipp.Core.Configuration.SerializationFormat.Xml;
                SyncRestHandler handler = new SyncRestHandler(serviceContext);

                IdsException firstException = GetResponseExpectingException(handler, serviceContext);
                Assert.AreEqual(intuitTid, firstException.Intuit_Tid);

                // The tid of the first call must not leak into the second one.
                IdsException secondException = GetResponseExpectingException(handler, serviceContext);
                Assert.AreNotEqual(intuitTid, secondException.Intuit_Tid);
            }
            finally
            {
                listener.Stop();
                ((IDisposable)listener).Dispose();
            }
        }

        /// <summary>
        /// Gets a free port on the loopback interface.
        /// </summary>
        private static int GetFreePort()
        {
            TcpListener portProbe = new TcpListener(IPAddress.Loopback, 0);
            portProbe.Start();
            int port = ((IPEndPoint)portProbe.LocalEndpoint).Port;
            portProbe.Stop();
            return port;
        }

        /// <summary>
        /// Answers one request with an HTTP 200 whose body contains a fault.
        /// </summary>
        private static void RespondWithFault(HttpListener listener, string faultResponse, string intuitTid)
        {
            HttpListenerContext context = listener.GetContext();
            if (intuitTid != null)
            {
                context.Response.Headers.Add("intuit_tid", intuitTid);
            }

            byte[] body = Encoding.UTF8.GetBytes(faultResponse);
            context.Response.StatusCode = 200;
            context.Response.ContentType = CoreConstants.CONTENTTYPE_APPLICATIONXML;
            context.Response.ContentLength64 = body.Length;
            context.Response.OutputStream.Write(body, 0, body.Length);
            context.Response.OutputStream.Close();
        }

        /// <summary>
        /// Calls the handler and returns the exception thrown for the fault in the response body.
        /// </summary>
        private static IdsException GetResponseExpectingException(SyncRestHandler handler, ServiceContext serviceContext)
        {
            string resourceUri = string.Format("v3/company/{0}/customer/1", serviceContext.RealmId);
            RequestParameters parameters = new RequestParameters(resourceUri, HttpVerbType.GET, CoreConstants.CONTENTTYPE_APPLICATIONXML);
            HttpWebRequest request = handler.PrepareRequest(parameters, null);
            try
            {
                handler.GetResponse(request);
            }
            catch (IdsException idsException)
            {
                return idsException;
            }

            Assert.Fail("Expected an IdsException for the fault in the response body.");
            return null;
        }
    }
}

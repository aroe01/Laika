//
// This file contains the sample code to use System.Net.WebRequest
// on the iPhone to communicate with HTTP and HTTPS servers
//
// Author:
//   Miguel de Icaza
//

using System;
using System.Net;
using System.Text;
using System.IO;

//using Foundation;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using Yousful.iOS;

namespace Yousful.Transactions {

	public abstract class DotNet {
		public static DotNet Instance;

		AppDelegate ad;
	public DotNet (AppDelegate ad)
		{
			this.ad = ad;
		}

		//
		// Asynchronous HTTP request
		//
		public void HttpSample ()
		{
			//Application.Busy ();
			string url = "http://localhost:3000/events";
			var request = WebRequest.Create (url);
			request.Method = "POST";


			string postData = "myFirstData=testData1";
			ASCIIEncoding encoding = new ASCIIEncoding ();
			byte[] byte1 = encoding.GetBytes (postData);

			// Set the content type of the data being posted.
			request.ContentType = "application/x-www-form-urlencoded";

			// Set the content length of the string being posted.
			request.ContentLength = byte1.Length;

			Stream newStream = request.GetRequestStream ();

			newStream.Write (byte1, 0, byte1.Length);


			request.BeginGetResponse (FeedDownloaded, request);
		}
		void FeedDownloaded (IAsyncResult result)
		{
			//Application.Done ();
			var request = result.AsyncState as HttpWebRequest;

			try {
				var response = request.EndGetResponse (result);
				ad.RenderStream (response.GetResponseStream ());
			} catch (Exception e) {
				Debug.WriteLine (e);
			}
		}
		public abstract void PhotoUploadTxn(string eventId, string filepath);
		public abstract void GetPhotoTxn ();
			
		/*void OnPhotoUpload (IAsyncResult result)
		{
			Console.WriteLine ("OnPhotoUpload");
			var request = result.AsyncState as HttpWebRequest;

			try {
				var response = request.EndGetResponse (result);
				ad.RenderStream (response.GetResponseStream ());
			} catch (Exception e) {
				Debug.WriteLine (e);
			}
		}*/

		public void EventCreateTxn (double latitude, double longitude)
		{
			//Application.Busy ();
			string url = "http://localhost:3000/events";
			var request = WebRequest.Create (url);
			request.Method = "POST";


			string postData = "latitude=" + latitude.ToString () + "&longitude=" + longitude.ToString ();
			//string postData = "event={latitude:" + latitude.ToString () + ",longitude:" + longitude.ToString () + "}";
			ASCIIEncoding encoding = new ASCIIEncoding ();
			byte[] byte1 = encoding.GetBytes (postData);

			// Set the content type of the data being posted.
			request.ContentType = "application/x-www-form-urlencoded";

			// Set the content length of the string being posted.
			request.ContentLength = byte1.Length;

			Stream newStream = request.GetRequestStream ();

			newStream.Write (byte1, 0, byte1.Length);


			request.BeginGetResponse (OnEventCreate, request);
		}


		void OnEventCreate (IAsyncResult result)
		{
			//Application.Done ();
			var request = result.AsyncState as HttpWebRequest;

			try {
				var response = request.EndGetResponse (result);
				ad.RenderStream (response.GetResponseStream ());
			} catch (Exception e) {
				Debug.WriteLine (e);
			}
		}


		public void GetEventsTxn(){
			string url = "http://localhost:3000/events";
			var request = WebRequest.Create (url);
			request.Method = "GET";

			//string postData = "latitude=" + latitude.ToString () + "&longitude=" + longitude.ToString ();
			//ASCIIEncoding encoding = new ASCIIEncoding ();
			//byte[] byte1 = encoding.GetBytes (postData);

			// Set the content type of the data being posted.
			request.ContentType = "application/x-www-form-urlencoded";

			// Set the content length of the string being posted.
			//request.ContentLength = byte1.Length;

			//Stream newStream = request.GetRequestStream ();

			//newStream.Write (byte1, 0, byte1.Length);


			request.BeginGetResponse (OnGetEvents, request);
		}
		void OnGetEvents (IAsyncResult result)
		{
			//Application.Done ();
			var request = result.AsyncState as HttpWebRequest;

			try {
				var response = request.EndGetResponse (result);
				ad.ParseEventsFromServer (response.GetResponseStream ());
			} catch (Exception e) {
				Debug.WriteLine (e);
			}
		}

		//
		// Asynchornous HTTPS request
		//
		public void HttpSecureSample ()
		{
			var https = (HttpWebRequest) WebRequest.Create ("https://gmail.com");
			//
			// To not depend on the root certficates, we will
			// accept any certificates:
			//
			ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, ssl) =>  true;

			https.BeginGetResponse (GmailDownloaded, https);
		}

		//
		// This sample just gets the result from calling
		// https://gmail.com, an HTTPS secure connection,
		// we do not attempt to parse the output, but merely
		// dump it as text
		//
		void GmailDownloaded (IAsyncResult result)
		{
			//Application.Done ();
			var request = result.AsyncState as HttpWebRequest;

			try {
				var response = request.EndGetResponse (result);
			//	ad.RenderStream (response.GetResponseStream ());
			} catch {
				// Error
			}
		}

		//
		// For an explanation of this AcceptingPolicy class, see
		// http://mono-project.com/UsingTrustedRootsRespectfully
		//
		// This will not be needed in the future, when MonoTouch
		// pulls the certificates from the iPhone directly
		//
		class AcceptingPolicy : ICertificatePolicy {
			public bool CheckValidationResult (ServicePoint sp, X509Certificate cert, WebRequest req, int error)
			{
				// Trust everything
				return true;
			}
		}
	}
}
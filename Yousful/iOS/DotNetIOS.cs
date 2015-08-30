using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using Yousful.Transactions;

namespace Yousful.iOS
{
	public class DotNetIOS : DotNet
	{

		public DotNetIOS (AppDelegate ad) : base(ad)
		{
		}

		public override async void PhotoUploadTxn(string eventId, string filepath){
			string url = "http://localhost:3000/events/photos/"+eventId;
			Console.WriteLine ("PhotoUploadTxn");
			var httpClient = new HttpClient();

			//httpClient.DefaultRequestHeaders.TransferEncodingChunked = true;		

			var content = new MultipartFormDataContent ();

			var imageContent = new StreamContent (new FileStream (filepath, FileMode.Open, FileAccess.Read, FileShare.Read));
			imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse ("image/jpeg");
			content.Add(imageContent, "image", "image.jpg");
			Console.WriteLine ("About to!");
			await httpClient.PostAsync(url, content);
			Console.WriteLine ("FINISH");
		}
		public override async void GetPhotoTxn(){
			string uri = "http://mythicspoiler.com/bfz/cards/dominatordrone.jpg";
			try
			{
				var httpClient = new HttpClient(); 
				byte[] dlFile = await httpClient.GetByteArrayAsync(uri);

				UIImage image = null;
				try {

					image = new UIImage(NSData.FromArray(dlFile));
					dlFile = null;
				} catch (Exception ) {
					return;// null;
				}

				var documentsDirectory = Environment.GetFolderPath
					(Environment.SpecialFolder.Personal);
				Console.WriteLine ("Saving to: " + documentsDirectory);
				string jpgFilename = System.IO.Path.Combine (documentsDirectory, "from_server_photo.jpg");
				//NSData imgData = originalImage.AsJPEG();
				NSError err = null;
				NSData imgData = image.AsJPEG();

				if (imgData.Save(jpgFilename, false, out err))
				{
					Console.WriteLine("saved as " + jpgFilename);
				} else {
					Console.WriteLine("NOT saved as" + jpgFilename + " because" + err.LocalizedDescription);
				}

			}
			catch (OperationCanceledException)
			{
				//Debug.LogWarning("File download got canceled.", "FileDownloader");
				//return false;
			}

			//return true;

		}
	}
}


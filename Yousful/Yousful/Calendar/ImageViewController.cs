using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yousful.Calendar;
using Yousful.Transactions;

namespace Yousful.Calendar{

	public class ImageViewController : UIViewController {

		UIImagePickerController imagePicker;
		UIButton choosePhotoButton;
		UIImageView imageView;
		UIScrollView scrollView;

		RootViewController parentController;
		int eventId;
		int imgCount = 0;

		public ImageViewController (RootViewController p, int eId)
		{
			this.parentController = p;
			this.eventId = eId;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Party Lifeline";

			scrollView = new UIScrollView (
				new RectangleF (0, 0, this.View.Frame.Width, this.View.Frame.Height - this.NavigationController.NavigationBar.Frame.Height));
			this.View.AddSubview (scrollView);

			// create our image view
			//imageView = new UIImageView (UIImage.FromFile ("Images/Icons/512_icon.png"));
			//scrollView.ContentSize = imageView.Image.Size;
			scrollView.MaximumZoomScale = 3f;
			scrollView.MinimumZoomScale = .1f;
			//scrollView.BackgroundColor = UIColor.Blue;
			//scrollView.AddSubview (imageView);

			//scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => { return imageView; };

			View.BackgroundColor = UIColor.White;

			//imageView = new UIImageView(new RectangleF(10, 150, 300, 300));
			//Add (imageView);

			choosePhotoButton = UIButton.FromType (UIButtonType.RoundedRect);
			choosePhotoButton.Frame = new RectangleF(10, 80, 100,40);
			choosePhotoButton.BackgroundColor = UIColor.Blue;
			choosePhotoButton.SetTitle ("Add Photo", UIControlState.Normal);
			choosePhotoButton.TouchUpInside += (s, e) => {
				// create a new picker controller
				imagePicker = new UIImagePickerController ();

				// set our source to the photo library
				imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;

				// set what media types
				imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary);

				imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
				imagePicker.Canceled += Handle_Canceled;

				// show the picker
				NavigationController.PresentModalViewController(imagePicker, true);
				//UIPopoverController picc = new UIPopoverController(imagePicker);

			};
			View.Add (choosePhotoButton);

			this.AddPhotos ();
		}
		void AddPhotos(){
			BL.Event currentEvent = BL.Managers.EventManager.GetEvent (this.eventId);
			string[] photoUrls = JsonConvert.DeserializeObject<string[]> (currentEvent.PhotoUrls);
			List<string> stringList = photoUrls.ToList();
			foreach (string imgName in stringList) {
				Console.WriteLine ("imgname: " + imgName);
				//FileStream imageContent = new FileStream (imgName, FileMode.Open, FileAccess.Read, FileShare.Read);
				NSData imageContent = NSData.FromFile (imgName);
				this.AddPhoto (UIImage.LoadFromData (imageContent));
			}
		}
		void AddPhoto(UIImage originalImage){
			//imageView.Image.Size
			Console.WriteLine("Height: " +originalImage.Size.Height.ToString());
			//UIImageView newView = new UIImageView(new RectangleF(10, (150*this.imgCount)+150, originalImage.Size.Width, originalImage.Size.Height));
			UIImageView newView = new UIImageView(new RectangleF(10, (300*this.imgCount)+100,300,300));
			newView.Image = originalImage;
			//Add (newView);
			this.imgCount++;
			this.scrollView.AddSubview(newView);
			this.scrollView.ContentSize = new SizeF(300,(300*this.imgCount)+100);

		}

		// Do something when the 
		void Handle_Canceled (object sender, EventArgs e) {
			Console.WriteLine ("picker cancelled");
			imagePicker.DismissModalViewControllerAnimated (true);
			//imagePicker.DismissModalViewController(true);
		}

		// This is a sample method that handles the FinishedPickingMediaEvent
		protected void Handle_FinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// determine what was selected, video or image
			bool isImage = false;
			//string jpgFilename = "";
			switch(e.Info[UIImagePickerController.MediaType].ToString())
			{
			case "public.image":
				Console.WriteLine("Image selected");
				isImage = true;
				break;

			case "public.video":
				Console.WriteLine("Video selected");
				break;
			}

			Console.WriteLine("Reference URL: [" + UIImagePickerController.ReferenceUrl + "]");

			// get common info (shared between images and video)
			NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;

			Console.WriteLine ("referenceURL 1:  " + referenceURL);
			Console.WriteLine ("e.toSTring: " + e.ToString ());

			if (referenceURL != null) 
				Console.WriteLine ("referenceURL 2:  " + referenceURL.ToString());		

			// if it was an image, get the other image info
			if(isImage) {

				// get the original image
				Console.WriteLine("OriginalImage: "+UIImagePickerController.OriginalImage);
				UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if(originalImage != null) {
					// do something with the image
					Console.WriteLine ("got the original image");
					//imageView.Image = originalImage;
					//imageView.AddSubview (originalImage);
					this.AddPhoto(originalImage);
					this.parentController.AddImageToEvent (eventId, originalImage);
					//UIImageView newView = new UIImageView(new RectangleF(10, 150, 300, 300));
					//newView.Image = originalImage;
					//Add (newView);
					//this.parentController.AddImageToEvent (eventId, originalImage);

				}

				// get the edited image
				UIImage editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
				if(editedImage != null) {
					// do something with the image
					Console.WriteLine ("got the edited image");
					imageView.Image = editedImage;
				}

				//- get the image metadata
				NSDictionary imageMetadata = e.Info[UIImagePickerController.MediaMetadata] as NSDictionary;
				if(imageMetadata != null) {
					// do something with the metadata
					Console.WriteLine ("got image metadata");
				}

			}
			// if it's a video
			else {
				// get video url
				NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
				if(mediaURL != null) {
					//
					Console.WriteLine(mediaURL.ToString());
				}
			}

			//string filepath = "Assets/myprofile.jpg";


			// dismiss the picker
			imagePicker.DismissModalViewControllerAnimated (true);
			//imagePicker.DismissModalViewController (true);
		}
	}
}
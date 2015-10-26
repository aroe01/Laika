using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.MapKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yousful.Map;
using Yousful.Calendar;
using Yousful.BL;
using Yousful.Transactions;

namespace Yousful
{
	[Register("RootViewController")]
	public partial class RootViewController : UINavigationController
	{
		[Outlet]
		UIBarButtonItem customButton { get; set; }

		MapPage CurrentMapPage;

		public RootViewController ()
		{
			//this.SetNavigationBarHidden (true, false);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Console.WriteLine ("VIEW DID LOAD");
			/*(customButton = new UIBarButtonItem (
				UIImage.FromFile ("Assets/monkey.png"),
				UIBarButtonItemStyle.Plain,
				(s, e) => {
					Console.WriteLine ("button tapped"); }
			);
			NavigationItem.SetRightBarButtonItem (customButton, false);*/
		}
		public void ShowMapPage(MapPage.Mode viewType){
			this.CurrentMapPage = new MapPage (this, viewType);
			this.PushViewController ( this.CurrentMapPage, false);
		}
		public void ShowEventCreation(int eventId){
			var chatContent = new RootElement ("New Event") {
				new Section("Event "+eventId.ToString()){}
			};
			this.PushViewController( new EventCreatePage (this,chatContent,eventId),true);
		}

		public void ShowImageView(int eventId){
			/*var chatContent = new RootElement ("Image View") {
				new Section("Img: "){}
			};*/
			this.PushViewController( new EventRoomViewController (this, eventId),true);

		}

		public void ShowCalendarList(){
			List<BL.Event> events = BL.Managers.EventManager.GetEvents ().ToList ();
			Section section = new Section ();
			foreach (BL.Event savedEvent in events) {
				section.Add (new CalendarEventEntry (savedEvent));
				//BL.Managers.EventManager.DeleteEvent(savedEvent.ID);
			}
			var chatContent = new RootElement ("Calendar") {
				section
			};
			this.PushViewController(new CalendarPage (chatContent),false);
		}

		public void UpdateView(){
			Console.WriteLine ("Updating View");
			if (this.CurrentMapPage != null) {
				this.CurrentMapPage.ShowActiveEvents ();
			}
		}
		public void AddEvent(BL.Event newEvent){
			Console.WriteLine ("AddEvent");
			if (this.CurrentMapPage != null) {
				this.CurrentMapPage.ShowEventNotation (newEvent);
			}
		}
		public void AddImageToEvent(int eventId,  UIImage originalImage){
			Console.WriteLine ("Adding Image");

			BL.Event currentEvent = BL.Managers.EventManager.GetEvent (eventId);

			string[] photoUrls = JsonConvert.DeserializeObject<string[]> (currentEvent.PhotoUrls);
			List<string> stringList = photoUrls.ToList();

			///////////////
			//var meta =  e.Info[UIImagePickerController.MediaMetadata] as NSDictionary;
			var documentsDirectory = Environment.GetFolderPath
				(Environment.SpecialFolder.Personal);
			Console.WriteLine ("Saving to: " + documentsDirectory);
			string jpgFilename = System.IO.Path.Combine (documentsDirectory, eventId.ToString()+"_"+stringList.Count.ToString()+".jpg");
			NSData imgData = originalImage.AsJPEG();
			NSError err = null;
			if (imgData.Save(jpgFilename, false, out err))
			{
				Console.WriteLine("saved as " + jpgFilename);
			} else {
				Console.WriteLine("NOT saved as" + jpgFilename + " because" + err.LocalizedDescription);
			}
			///////////////	
			//DotNet.Instance.PhotoUploadTxn (eventId.ToString(),jpgFilename);
					
			stringList.Add (jpgFilename);
			string urlsString = JsonConvert.SerializeObject (stringList.ToArray());
			currentEvent.PhotoUrls = urlsString;
			Console.WriteLine ("photourls: " + urlsString);
			BL.Managers.EventManager.SaveEvent (currentEvent);

		}
		public void AddUserToEvent(int eventId, User userToAdd){

			Console.WriteLine ("Adding User");

			BL.Event currentEvent = BL.Managers.EventManager.GetEvent (eventId);

			string[] userIds = JsonConvert.DeserializeObject<string[]> (currentEvent.UserIds);
			List<string> stringList = userIds.ToList();

			stringList.Add (userToAdd.ID.ToString());
			string urlsString = JsonConvert.SerializeObject (stringList.ToArray());
			currentEvent.UserIds = urlsString;

			Console.WriteLine ("UserIds: " + urlsString);
			BL.Managers.EventManager.SaveEvent (currentEvent);

		}
	}
}


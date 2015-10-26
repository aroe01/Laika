//
// Sample shows how to use MonoTouch.Dialog to create an iPhone SMS-like 
// display of conversations
//
// Author:
//   Miguel de Icaza 
//
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
using Yousful.BL.Managers;
using Yousful.Transactions;

namespace Yousful.iOS {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		RootViewController rootControl;



		UIViewController MakeMap(){
			/*MapPage viewController;
			var chatContent = new RootElement (title) {
				new Section("Map Page"){}
			};*/
			return new MapPage (rootControl,MapPage.Mode.VIEW);
		}


		UIViewController MakeCalendar (string title)
		{
			//var loginButton = new StringElement ("Login", delegate {
				//login.FetchValue ();
				//pass.FetchValue ();
				//if (login.Value == "Root" && pass.Value == "Root"){
					//NSUserDefaults.StandardUserDefaults.SetBool (true, "loggedIn");

			//	window.RootViewController.PresentViewController (MakeEventCreatePage ("Home"), true, delegate {});
				//}
			//});
			List<BL.Event> events = BL.Managers.EventManager.GetEvents ().ToList ();
			Section section = new Section ();
			foreach(BL.Event savedEvent in events){
				section.Add (new CalendarEventEntry (savedEvent));
				//BL.Managers.EventManager.DeleteEvent(savedEvent.ID);
			}
			var chatContent = new RootElement (title) {
				section
			};
			return new CalendarPage (chatContent);
		}
		/*UIViewController MakeEventCreatePage (string title)
		{
			//var nameEntry = new EntryElement ("Event Name", "Enter name", "");
			//var descEntry = new EntryElement ("Description", "Describe it!", "");
			//var loginButton = new StringElement ("Login", delegate {
				//login.FetchValue ();
				//pass.FetchValue ();
				//if (login.Value == "Root" && pass.Value == "Root"){
				//	NSUserDefaults.StandardUserDefaults.SetBool (true, "loggedIn");

					//window.RootViewController.PresentViewController (MakeCalendar ("Home"), true, delegate {});
				//}
			//});
			var chatContent = new RootElement (title) {
				new Section("Create Event"){}
			};
			return new EventCreatePage (this.rootControl,chatContent);
		}*/

		 /*UIViewController MakeHome ()
		{
			var options = new DialogViewController (new RootElement ("Yousful") {
				new Section ("Home"){
					(Element)new RootElement ("Check your Calender", x=> MakeCalendar ("Calender")),
					//(Element)new RootElement ("Make an event", x=> MakeEventCreatePage ("Event Create")),
					(Element)new RootElement ("Show Map", x=> MakeMap ()),
				}
			});
			return new UINavigationController (options);
		}*/
		UIViewController MakeHome(){
			rootControl = new RootViewController ();
			rootControl.ShowMapPage (MapPage.Mode.VIEW);
			//rootControl.PushViewController (MakeMap(), false);
			//rootControl.PushViewController (MakeCalendar ("Calender"),false);

			return rootControl;
		}

		/*UIViewController MakeLogin ()
		{
			var login = new EntryElement ("Event Name", "Enter name", "");
			var pass = new EntryElement ("Description", "Describe it!", "");

			var loginButton = new StringElement ("Login", delegate {
				login.FetchValue ();
				pass.FetchValue ();
				if (login.Value == "Root" && pass.Value == "Root"){
					NSUserDefaults.StandardUserDefaults.SetBool (true, "loggedIn");

					window.RootViewController.PresentViewController (MakeCalendar ("Home"), true, delegate {});
				}
			});

			return new DialogViewController (new RootElement ("Login"){
				new Section ("Enter login and password"){
					login, pass,
				},
				new Section (){
					loginButton
				}
			});
		}*/
		public void RenderStream (Stream stream)
		{
			var reader = new System.IO.StreamReader (stream);
			Console.WriteLine ("Response: " + reader.ReadToEnd ());
			/*InvokeOnMainThread (delegate {
				var view = new UIViewController ();
				var label = new UILabel (new CGRect (20, 20, 300, 80)){
					Text = "The HTML returned by the server:"
				};
				var tv = new UITextView (new CGRect (20, 100, 300, 400)){
					Text = reader.ReadToEnd ()
				};
				view.Add (label);
				view.Add (tv);

				if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
					view.EdgesForExtendedLayout = UIRectEdge.None;
				}

				navigationController.PushViewController (view, true);
			});*/
		}
		public void ParseEventsFromServer(Stream stream){
			var reader = new System.IO.StreamReader (stream);		
			var responseString = reader.ReadToEnd ();
			Console.WriteLine ("Response: " + responseString);
			var fullResponseJson = JObject.Parse (responseString);
			var eventArray = fullResponseJson ["events"];
			foreach (var eventObj in eventArray) {
				Console.WriteLine ("Event: " + eventObj.ToString ());
				BL.Event currentEvent = new BL.Event();
				double eventId = currentEvent.ID;

				BL.Event eventData = JsonConvert.DeserializeObject<Event> (eventObj.ToString ());
				Console.WriteLine ("ID: " + eventData.ID.ToString ());
				//Console.WriteLine ("Lat: " + eventData.Lat.ToString ());
				currentEvent.ID = eventData.ID;
				currentEvent.Lat = eventData.Lat;
				currentEvent.Long = eventData.Long;
				currentEvent.PhotoUrls = eventData.PhotoUrls;

				//var result = BL.Managers.EventManager.SaveEvent (eventData);
				var result = BL.Managers.EventManager.SaveEvent (currentEvent);

				Console.WriteLine ("Result1: " + result.ToString ());
			}
			List<BL.Event> events = BL.Managers.EventManager.GetEvents ().ToList ();
			Console.WriteLine ("Count: " + events.Count.ToString ());
			InvokeOnMainThread (delegate {
				this.rootControl.UpdateView ();
			});
		}

		void ClearAllEvents(){
			List<BL.Event> events = BL.Managers.EventManager.GetEvents ().ToList ();
			foreach(BL.Event savedEvent in events){
				BL.Managers.EventManager.DeleteEvent(savedEvent.ID);
			}
		}
		void InitUser(){
			UserManager.CurrentUser = new BL.User ();
		}
		void ShowMap(){
			UIViewController main = MakeHome ();
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			//window.AddSubview (main.View);
			window.RootViewController = main;
			window.MakeKeyAndVisible ();

		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			InitUser ();

			ClearAllEvents ();

			//DotNet.Instance = new DotNet (this);
			//DotNet.Instance = new DotNetIOS(this);

			//UIViewController main =MakeCalendar ("Home");
			//UIViewController main =MakeEventCreatePage ("Home");
			//UIViewController main = MakeLogin();

			// If you have defined a view, add it here:
			//window.AddSubview (navigationController.View);

			//UIViewController main = MakeHome ();

			//disabled for offline 
			//DotNet.Instance.GetEventsTxn ();
			//DotNet.Instance.GetPhotoTxn ();
			this.ShowMap ();

			//window = new UIWindow (UIScreen.MainScreen.Bounds);
			//window.RootViewController = main;
			//window.MakeKeyAndVisible ();

			return true;
		}
	}
}


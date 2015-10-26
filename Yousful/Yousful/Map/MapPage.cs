using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using System.Linq;
using System.Collections.Generic;
using Yousful.Transactions;
using Yousful.Calendar;

namespace Yousful.Map
{
	[Register("MapPage")]
	public partial class MapPage : UIViewController
	{
		public enum Mode{VIEW, CREATE_NEW}
		[Outlet]
		MonoTouch.MapKit.MKMapView map{ get; set; }

		//UITabBarController tabController{get; set;}

		[Outlet]
		UIView bottomBar {get; set;}

		[Outlet]
		UIButton turnUp {get; set;}

		MyMapDelegate mapDel;
		LocationDelegate locationDel;
		UISearchBar searchBar;
		UISearchDisplayController searchController;
		RootViewController parentController;
		Mode currentMode = Mode.VIEW;
		UIBarButtonItem cancelButton,createEventButton;
		MKPointAnnotation eventCreationPin;
		CLLocationManager locationManager;
		List<int> currentEventIds;
		//BL.Event currentEvent;

		//CLLocationCoordinate2D MapCenter;

		public MapPage (RootViewController p, MapPage.Mode viewType) : base ("MapPage", null)
		{
			this.parentController = p;
			this.currentMode = viewType;
			this.currentEventIds = new List<int> ();
			//this.currentEvent = curEv;

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SetupLocationManager ();

			if (CLLocationManager.LocationServicesEnabled) {
				Console.WriteLine ("Enabled");
				Setup ();
			}				
		}

		void SetupLocationManager ()
		{
			if (locationManager != null)
				return;

			locationDel = new LocationDelegate (this.map);
			locationManager = new CLLocationManager ();
			locationManager.Delegate = locationDel;
			//locationManager.AuthorizationChanged += OnAuthorizationChanged;
			locationManager.RequestWhenInUseAuthorization();
		}

		void OnAuthorizationChanged (object sender, CLAuthorizationChangedEventArgs e)
		{
			Console.WriteLine ("Changed");
			switch (e.Status) {
			case CLAuthorizationStatus.AuthorizedAlways:
				Console.WriteLine ("AAA");
				//locationManager.StartUpdatingLocation ();
				Setup();
				break;

			case CLAuthorizationStatus.NotDetermined:
				Console.WriteLine ("ND");
				locationManager.RequestAlwaysAuthorization ();
				break;

			default:
				break;
			}
		}

		protected void Setup(){			

			// set the map delegate
			Console.WriteLine("Setup");
			mapDel = new MyMapDelegate (this);//.parentController);
			map.Delegate = mapDel;
			map.ShowsUserLocation = true;
			//locationManager.StartMonitoringSignificantLocationChanges ();
			locationManager.StartUpdatingLocation ();
			NavigationItem.Title = "Laika";

			//nav bar
			createEventButton = new UIBarButtonItem (
				UIImage.FromFile ("Assets/Image.png"),
				UIBarButtonItemStyle.Plain,
				(s, e) => {
					Console.WriteLine ("button tapped"); 
					//this.parentController.ShowEventCreation();\
					this.EnableCreationMode();
				}
			);
			cancelButton = new UIBarButtonItem (
				UIImage.FromFile ("Assets/Image.png"),
				UIBarButtonItemStyle.Plain,
				(s, e) => {
					Console.WriteLine ("button tapped"); 
					//this.parentController.ShowEventCreation();\
					this.CancelCreationMode();
				}
			);	
			turnUp.TouchUpInside +=  (sender, ea) => {
				//new UIAlertView("Touch2", "TouchUpInside handled", null, "OK", null).Show();
				if(this.currentMode == Mode.CREATE_NEW){
					this.CancelCreationMode();
				}else{
					this.EnableCreationMode();
				}
				
			};
			UIImage logoImage = UIImage.FromFile ("Assets/rsz_11space_dog.jpg");
			logoImage = logoImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
			UIBarButtonItem logo = new UIBarButtonItem(
				logoImage,
				UIBarButtonItemStyle.Plain,
				(s,e) => {
				}
			);
			NavigationItem.SetLeftBarButtonItem (logo, false);

			// set map type and show user location
			map.MapType = MKMapType.Standard;

			// set map center and region
			double lat = 37.784622;
			double lon =  -122.413090;

			/*map.DidUpdateUserLocation += (sender, e) => {

				Console.WriteLine ("Test 1");
				if (map.UserLocation.Location != null && !this.MapCenter.Equals(map.UserLocation)) {
					Console.WriteLine ("userloc:"+map.UserLocation.Coordinate.Latitude + "," + map.UserLocation.Coordinate.Longitude);
					//CLLocationCoordinate2D coords = map.UserLocation.Coordinate;

					this.MapCenter = map.UserLocation.Coordinate;
					var mapRegion = MKCoordinateRegion.FromDistance (this.MapCenter, 2000, 2000);
					map.CenterCoordinate = this.MapCenter;
					map.Region = mapRegion;

				}else{
					//this.MapCenter = new CLLocationCoordinate2D (lat, lon);
				}

			};
			/*
			map.WillStartLocatingUser += (sender, e) => {
				Console.WriteLine ("Test 2");
				if (map.UserLocation.Location != null && !this.MapCenter.Equals(map.UserLocation)) {
					Console.WriteLine ("userloc:"+map.UserLocation.Coordinate.Latitude + "," + map.UserLocation.Coordinate.Longitude);
					//CLLocationCoordinate2D coords = map.UserLocation.Coordinate;

					this.MapCenter = map.UserLocation.Coordinate;
					var mapRegion = MKCoordinateRegion.FromDistance (this.MapCenter, 2000, 2000);
					map.CenterCoordinate = this.MapCenter;
					map.Region = mapRegion;
				}else{
					this.MapCenter = new CLLocationCoordinate2D (lat, lon);
					var mapRegion = MKCoordinateRegion.FromDistance (this.MapCenter, 2000, 2000);
					map.CenterCoordinate = this.MapCenter;
					map.Region = mapRegion;
				}
				/*var mapRegion = MKCoordinateRegion.FromDistance (this.MapCenter, 2000, 2000);
				map.CenterCoordinate = this.MapCenter;
				map.Region = mapRegion;*/
			//};


			//map.DidChangeValue
			if (!map.UserLocationVisible) {
				// user denied permission, or device doesn't have GPS/location ability
				Console.WriteLine ("userloc not visible, show sf");

				/*this.MapCenter = new CLLocationCoordinate2D (lat, lon);

				var mapRegion = MKCoordinateRegion.FromDistance (this.MapCenter, 2000, 2000);
				map.CenterCoordinate = this.MapCenter;
				map.Region = mapRegion;*/
			}


			if (this.currentMode == Mode.VIEW) {
				this.ShowActiveEvents ();
				//NavigationItem.SetRightBarButtonItem (createEventButton, false);
			} else {

				//NavigationItem.SetLeftBarButtonItem (cancelButton, false);
				this.ShowPotentialLocationPin ();
			}


			var tapRecogniser = new UITapGestureRecognizer(this, new MonoTouch.ObjCRuntime.Selector("MapTapSelector:"));
			map.AddGestureRecognizer(tapRecogniser);
			//map.AddGestureRecognizer = new UItouch

			// create search controller
			/*searchBar = new UISearchBar (new RectangleF ( 0,View.Frame.Height - 20, View.Frame.Width, 50)) {
				Placeholder = "Enter a search query"
			};
			searchController = new UISearchDisplayController (searchBar, this);
			searchController.Delegate = new SearchDelegate (map);
			searchController.SearchResultsSource = new SearchSource (searchController, map);
			View.AddSubview (searchBar);*/
		}

		public void OnEventCreation(int eventId){
			this.CancelCreationMode ();
			this.parentController.UpdateView ();
			this.ShowEventPage(eventId);

		}
		public void ShowEventPage(int eventId){
			

			this.parentController.ShowImageView (eventId);
			//this.parentController.ShowEventCreation (eventId);
		}
			
		protected void CancelCreationMode(){
			this.currentMode = Mode.VIEW;
			//NavigationItem.LeftBarButtonItem = null;
			//NavigationItem.SetRightBarButtonItem (createEventButton, false);
			this.HidePotentialLocationPin ();
		}
		protected void EnableCreationMode(){
			this.currentMode = Mode.CREATE_NEW;
			//NavigationItem.SetLeftBarButtonItem (cancelButton, false);
			//NavigationItem.RightBarButtonItem = null;
			this.ShowPotentialLocationPin ();
		}

		public void ShowActiveEvents(){
			Console.WriteLine ("ShowActiveEvents");
			List<BL.Event> events = BL.Managers.EventManager.GetEvents ().ToList ();
			Console.WriteLine ("Count: " + events.Count.ToString ());
			foreach (BL.Event savedEvent in events) {
				Console.WriteLine ("Eventtttt");
				if (this.currentEventIds.Contains (savedEvent.ID)) {
					continue;
				}
				this.currentEventIds.Add (savedEvent.ID);
				this.ShowEventNotation (savedEvent);
			}
		}
		protected void ShowPotentialLocationPin(){
			// add an annotation
			Console.WriteLine("Showpotential");
			if (eventCreationPin == null) {
				Console.WriteLine ("Set pin");//nameEntry.Value);

				eventCreationPin = new MKPointAnnotation () {
					Title = "Here?", 
					Coordinate = this.map.CenterCoordinate
				};
			} else {
				eventCreationPin.Coordinate = this.map.CenterCoordinate;
			}
			map.AddAnnotation (eventCreationPin);
		}
		protected void HidePotentialLocationPin(){
			map.RemoveAnnotation (eventCreationPin);
		}

		public void ShowEventNotation(BL.Event mapEvent){ //CLLocationCoordinate2D mapLoc){
			MonkeyAnnotation annot = new MonkeyAnnotation (mapEvent);
			map.AddAnnotation (annot);
			CLLocationCoordinate2D mapLoc = new CLLocationCoordinate2D (mapEvent.Lat, mapEvent.Long);

			MKCircle circleOverlay = MKCircle.Circle (mapLoc, 500);
			map.AddOverlay (circleOverlay);

			//CreateCalenderEvent (mapLoc);

		}
		void CreateCalenderEvent(CLLocationCoordinate2D mapLoc){
			Console.WriteLine ("New Event");//nameEntry.Value);
			BL.Event currentEvent = new BL.Event();
			currentEvent.Name = "Name: " + currentEvent.ID;//nameEntry.Value;
			currentEvent.Date = "1/1/15";//dateEntry.Value;
			currentEvent.Notes = "Test Event";//descEntry.Value;
			currentEvent.Lat = mapLoc.Latitude;
			currentEvent.Long = mapLoc.Longitude;
			BL.Managers.EventManager.SaveEvent(currentEvent);

			//map.AddAnnotation (new MonkeyAnnotation (currentEvent));//.ID,currentEvent.Name, mapLoc));
			this.ShowEventNotation (currentEvent);
			//this.GoToCalendarPage ();
		}


		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		[Export("MapTapSelector:")]
		protected void OnMapTapped(UIGestureRecognizer sender)
		{
			Console.WriteLine ("Tappedy");		
			CLLocationCoordinate2D tappedLocationCoord =  map.ConvertPoint(sender.LocationInView(map), map);
			//this.CreateCalenderEvent (tappedLocationCoord);
			//... add code here to add MKAnnotation to map using 'tappedLocationCoord' as annotation co-ordinate ...
		}

		class MyMapDelegate : MKMapViewDelegate
		{
			string pId = "PinAnnotation";
			string mId = "MonkeyAnnotation";
			UIButton detailButton;
			MapPage parentController;

			public MyMapDelegate(MapPage p){
				this.parentController = p;
			}

			public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, NSObject annotation)
			{
				MKAnnotationView anView;

				Console.WriteLine ("Check 1");

				if (annotation is MKUserLocation) {
					Console.WriteLine ("Check 2");
					return null; 
				}

				if (annotation is MonkeyAnnotation) {
					Console.WriteLine ("Check 3");
					// show monkey annotation
					anView = mapView.DequeueReusableAnnotation (mId);

					if (anView == null)
						anView = new MKAnnotationView (annotation, mId);


					anView.Image = UIImage.FromFile ("Assets/myprofile.jpg").Scale (new SizeF(50, 50));
						Console.WriteLine ("anView.Image: "+anView.Image.Size);

					anView.CanShowCallout = true;
					//anView.Draggable = true;
					detailButton =UIButton.FromType (UIButtonType.DetailDisclosure);
					anView.RightCalloutAccessoryView = detailButton;
					//anView.RightCalloutAccessoryView = UIButton.FromType (UIButtonType.DetailDisclosure);

				} else {
					Console.WriteLine ("Check 4");
					// show pin annotation
					anView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation (pId);

					if (anView == null)
						anView = new MKPinAnnotationView (annotation, pId);
					Console.WriteLine ("PIN");

					((MKPinAnnotationView)anView).PinColor = MKPinAnnotationColor.Green;
					//((MKPinAnnotationView)anView).AnimatesDrop = false;
					anView.CanShowCallout = true;
					anView.Draggable = true;
					detailButton =UIButton.FromType (UIButtonType.InfoLight);
					anView.RightCalloutAccessoryView = detailButton;

				}

				return anView;
			}

			public override void DidSelectAnnotationView (MKMapView mapView, MKAnnotationView view)
			{
				Console.WriteLine ("DidSelectAnnotationView");//
			}

			public override void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control)
			{
				var monkeyAn = view.Annotation as MonkeyAnnotation;

				if (monkeyAn != null) {
					//var alert = new UIAlertView ("Event", monkeyAn.Title, null, "OK");
					//alert.Show ();
					Console.WriteLine("EventiD:  "+(monkeyAn as MonkeyAnnotation).EventId.ToString());
					this.parentController.ShowEventPage((monkeyAn as MonkeyAnnotation).EventId);
					//ShowEventCreation
				} else {
					var pinAn = view.Annotation as MKPointAnnotation;
					if (pinAn != null ) {
						Console.WriteLine ("Confirmed");

						//DotNet.Instance.EventCreateTxn (pinAn.Coordinate.Latitude,pinAn.Coordinate.Longitude);

						////////testing, should be on server return
						BL.Event currentEvent = new BL.Event();
						IList<BL.Event> eventList =  BL.Managers.EventManager.GetEvents();
						currentEvent.ID = eventList.Count;
						Console.WriteLine ("Making: " + currentEvent.ID.ToString ());
						currentEvent.Lat = pinAn.Coordinate.Latitude;
						currentEvent.Long = pinAn.Coordinate.Longitude;

						var result = BL.Managers.EventManager.SaveEvent (currentEvent);
						Console.WriteLine ("Result: "+result.ToString());
						//this.ShowEventNotation (savedEvent);

						/////////////////
						//InvokeOnMainThread (delegate {
						this.parentController.OnEventCreation(currentEvent.ID);
							
						//});

						//this.parentController.PopToRootViewController (true);
					}

				}
			}

			public override MKOverlayView GetViewForOverlay (MKMapView mapView, NSObject overlay)
			{
				var circleOverlay = overlay as MKCircle;
				var circleView = new MKCircleView (circleOverlay);
				circleView.FillColor = UIColor.Red;
				circleView.Alpha = 0.3f;
				return circleView;
			}
			public override void DidUpdateUserLocation (MKMapView mapView, MKUserLocation userLocation)
			{
				return;
				Console.WriteLine ("DidUpdateUserLocation 1");
				if (mapView.UserLocation.Location != null && ! mapView.UserLocation.Coordinate.Equals(mapView.UserLocation)) {
					Console.WriteLine ("userloc:"+mapView.UserLocation.Coordinate.Latitude + "," + mapView.UserLocation.Coordinate.Longitude);
					//CLLocationCoordinate2D coords = map.UserLocation.Coordinate;

					//this.MapCenter = map.UserLocation.Coordinate;
					var mapRegion = MKCoordinateRegion.FromDistance ( mapView.UserLocation.Coordinate, 2000, 2000);
					mapView.CenterCoordinate =  mapView.UserLocation.Coordinate;
					mapView.Region = mapRegion;

				}else{
					//this.MapCenter = new CLLocationCoordinate2D (lat, lon);
				}

			}
			public override void WillStartLocatingUser (MKMapView mapView)
			{				
				Console.WriteLine ("WillStartLocatingUser 2");
				if (mapView.UserLocation.Location != null && !mapView.Equals(mapView.UserLocation)) {
					Console.WriteLine ("userloc:"+mapView.UserLocation.Coordinate.Latitude + "," + mapView.UserLocation.Coordinate.Longitude);
					//CLLocationCoordinate2D coords = map.UserLocation.Coordinate;

					//this.MapCenter = map.UserLocation.Coordinate;
					var mapRegion = MKCoordinateRegion.FromDistance (mapView.UserLocation.Coordinate, 2000, 2000);
					mapView.CenterCoordinate = mapView.UserLocation.Coordinate;
					mapView.Region = mapRegion;
				}else{
					//this.MapCenter = new CLLocationCoordinate2D (lat, lon);
					CLLocationCoordinate2D fakeLocation = new CLLocationCoordinate2D( 37.784622, -122.413090);
					var mapRegion = MKCoordinateRegion.FromDistance (fakeLocation, 2000, 2000);
					mapView.CenterCoordinate = fakeLocation;
					mapView.Region = mapRegion;
				}
				/*var mapRegion = MKCoordinateRegion.FromDistance (this.MapCenter, 2000, 2000);
				map.CenterCoordinate = this.MapCenter;
				map.Region = mapRegion;*/
			}

		}
		class LocationDelegate : CLLocationManagerDelegate{

			MonoTouch.MapKit.MKMapView mapView;

			public LocationDelegate(MonoTouch.MapKit.MKMapView map){
				this.mapView = map;
			}

			public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
			{
				//Console.WriteLine ("UpdatedLocation");
				if (oldLocation == null || (newLocation != null && ! newLocation.Coordinate.Equals(oldLocation.Coordinate))) {
					Console.WriteLine ("UpdatedLocation userloc:"+newLocation.Coordinate.Latitude + "," + newLocation.Coordinate.Longitude);
					//CLLocationCoordinate2D coords = map.UserLocation.Coordinate;

					//this.MapCenter = map.UserLocation.Coordinate;
					var mapRegion = MKCoordinateRegion.FromDistance (newLocation.Coordinate, 2000, 2000);
					this.mapView.CenterCoordinate = newLocation.Coordinate;// this.mapView.UserLocation.Coordinate;
					this.mapView.Region = mapRegion;

				}else{
					//this.MapCenter = new CLLocationCoordinate2D (lat, lon);
				}

			}

		}

		/*class SearchDelegate : UISearchDisplayDelegate
		{
			MKMapView map;

			public SearchDelegate (MKMapView map)
			{
				this.map = map;
			}

			public override bool ShouldReloadForSearchString (UISearchDisplayController controller, string forSearchString)
			{
				// create search request
				var searchRequest = new MKLocalSearchRequest ();
				searchRequest.NaturalLanguageQuery = forSearchString;
				searchRequest.Region = new MKCoordinateRegion (map.UserLocation.Coordinate, new MKCoordinateSpan (0.25, 0.25));

				// perform search
				var localSearch = new MKLocalSearch (searchRequest);
				localSearch.Start (delegate (MKLocalSearchResponse response, NSError error) {
					if (response != null && error == null) {
						((SearchSource)controller.SearchResultsSource).MapItems = response.MapItems.ToList();
						controller.SearchResultsTableView.ReloadData();
					} else {
						Console.WriteLine ("local search error: {0}", error);

						//						In "MKTypes.h" in the MapKit framework, the following is defined:
						//
						//							Error constants for the Map Kit framework.
						//
						//								enum MKErrorCode {
						//								MKErrorUnknown = 1,
						//								MKErrorServerFailure,
						//								MKErrorLoadingThrottled,
						//								MKErrorPlacemarkNotFound,
						//							};
					}
				});

				return true;
			}
		}

		class SearchSource : UITableViewSource
		{
			static readonly string mapItemCellId = "mapItemCellId";
			UISearchDisplayController searchController;
			MKMapView map;

			public List<MKMapItem> MapItems { get; set; }

			public SearchSource (UISearchDisplayController searchController, MKMapView map)
			{
				this.searchController = searchController;
				this.map = map;

				MapItems = new List<MKMapItem> ();
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return MapItems.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (mapItemCellId);

				if (cell == null)
					cell = new UITableViewCell ();

				cell.TextLabel.Text = MapItems [indexPath.Row].Name;
				return cell;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				searchController.SetActive (false, true);

				// add item to map
				CLLocationCoordinate2D coord = MapItems [indexPath.Row].Placemark.Location.Coordinate;
				map.AddAnnotation (new MKPointAnnotation () {
					Title = MapItems [indexPath.Row].Name, 
					Coordinate = coord
				});

				map.SetCenterCoordinate (coord, true);
			}
		}*/
	}
}


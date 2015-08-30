using System;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;

namespace Yousful.Map
{
	public class MonkeyAnnotation : MKAnnotation
	{
		string title;
		int eventId;
		CLLocationCoordinate2D coord;
		//BL.Event Event;

		public MonkeyAnnotation (BL.Event mapEvent)//int id,string title, CLLocationCoordinate2D coord)
		{
			//this.Event = mapEvent;
			this.eventId = mapEvent.ID;
			this.title = mapEvent.ID.ToString ();
			//this.title = mapEvent.Name;//title;
			this.coord = new CLLocationCoordinate2D (mapEvent.Lat, mapEvent.Long);
		}

		public override string Title {
			get {
				return title;
			}
		}
		public int EventId{
			get{
				return eventId;
			}
		}

		public override CLLocationCoordinate2D Coordinate {
			get {
				return coord;
			}
			set {
				coord = value;
			}
		}
	}
}


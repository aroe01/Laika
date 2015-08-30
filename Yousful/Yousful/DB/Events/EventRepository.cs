using System;
using System.Collections.Generic;
using System.IO;
using Yousful.BL;

namespace Yousful.DAL {
	public class EventRepository {
		DL.EventDatabase db = null;
		protected static string dbLocation;		
		protected static EventRepository me;		
		
		static EventRepository ()
		{
			me = new EventRepository();
		}
		
		protected EventRepository()
		{
			// set the db location
			dbLocation = DatabaseFilePath;
			
			// instantiate the database	
			db = new Yousful.DL.EventDatabase(dbLocation);
		}
		
		public static string DatabaseFilePath {
			get { 
				var sqliteFilename = "EventDB.db3";
#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
	            var path = sqliteFilename;
#else



#if __ANDROID__
				// Just use whatever directory SpecialFolder.Personal returns
	            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
#endif










				var path = Path.Combine (libraryPath, sqliteFilename);
#endif		






				return path;	
			}
		}

		public static Event GetEvent(int id)
		{
            return me.db.GetItem<Event>(id);
		}
		
		public static IEnumerable<Event> GetEvents ()
		{
			return me.db.GetItems<Event>();
		}
		
		public static int SaveEvent (Event item)
		{
			return me.db.SaveItem<Event>(item);
		}

		public static int DeleteEvent(int id)
		{
			return me.db.DeleteItem<Event>(id);
		}
	}
}


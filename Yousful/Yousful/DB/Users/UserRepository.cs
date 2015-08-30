using System;
using System.Collections.Generic;
using System.IO;
using Yousful.BL;

namespace Yousful.DAL {
	public class UserRepository {
		DL.UserDatabase db = null;
		protected static string dbLocation;		
		protected static UserRepository me;		
		
		static UserRepository ()
		{
			me = new UserRepository();
		}
		
		protected UserRepository()
		{
			// set the db location
			dbLocation = DatabaseFilePath;
			
			// instantiate the database	
			db = new Yousful.DL.UserDatabase(dbLocation);
		}
		
		public static string DatabaseFilePath {
			get { 
				var sqliteFilename = "UserDB.db3";
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

		public static User GetUser(int id)
		{
            return me.db.GetItem<User>(id);
		}
		
		public static IEnumerable<User> GetUsers ()
		{
			return me.db.GetItems<User>();
		}
		
		public static int SaveUser (User item)
		{
			return me.db.SaveItem<User>(item);
		}

		public static int DeleteUser(int id)
		{
			return me.db.DeleteItem<User>(id);
		}
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using Yousful.BL;
using Yousful.DL.SQLite;

namespace Yousful.DL
{
	/// <summary>
	/// EventDatabase builds on SQLite.Net and represents a specific database, in our case, the Event DB.
	/// It contains methods for retrieval and persistance as well as db creation, all based on the 
	/// underlying ORM.
	/// </summary>
	public class EventDatabase : SQLiteConnection
	{
		static object locker = new object ();

		/// <summary>
		/// Initializes a new instance of the <see cref="Eventy.DL.EventDatabase"/> EventDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		public EventDatabase (string path) : base (path)
		{
			// create the tables
			CreateTable<Event> ();
		}
		
		public IEnumerable<T> GetItems<T> () where T : BL.Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
                return (from i in Table<T> () select i).ToList ();
            }
		}

		public T GetItem<T> (int id) where T : BL.Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
                return Table<T>().FirstOrDefault(x => x.ID == id);
                // Following throws NotSupportedException - thanks aliegeni
                //return (from i in Table<T> ()
                //        where i.ID == id
                //        select i).FirstOrDefault ();
            }
		}

		public int SaveItem<T> (T item) where T : BL.Contracts.IBusinessEntity
		{
            lock (locker) {
                //if (item.ID != 0) {
				if(this.GetItem<Event>(item.ID) != null){
					Console.WriteLine ("Updating");
                    Update (item);
                    return item.ID;
                } else {
					Console.WriteLine ("Inserting");
                    return Insert (item);
                }
            }
		}
		
		public int DeleteItem<T>(int id) where T : BL.Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
                return Delete<T> (new T () { ID = id });
            }
		}
	}
}
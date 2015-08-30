using System;
using System.Collections.Generic;
using Yousful.BL;

namespace Yousful.BL.Managers
{
	public static class EventManager
	{

		static EventManager ()
		{
		}

		public static Event GetEvent(int id)
		{
			return DAL.EventRepository.GetEvent(id);
		}
		
		public static IList<Event> GetEvents ()
		{
			return new List<Event>(DAL.EventRepository.GetEvents());
		}
		
		public static int SaveEvent (Event item)
		{
			return DAL.EventRepository.SaveEvent(item);
		}
		
		public static int DeleteEvent(int id)
		{
			return DAL.EventRepository.DeleteEvent(id);
		}
		
	}
}
using System;
using System.Collections.Generic;
using Yousful.BL;

namespace Yousful.BL.Managers
{
	public static class UserManager
	{

		static UserManager ()
		{
		}

		public static User GetUser(int id)
		{
			return DAL.UserRepository.GetUser(id);
		}
		
		public static IList<User> GetUsers ()
		{
			return new List<User>(DAL.UserRepository.GetUsers());
		}
		
		public static int SaveUser (User item)
		{
			return DAL.UserRepository.SaveUser(item);
		}
		
		public static int DeleteUser(int id)
		{
			return DAL.UserRepository.DeleteUser(id);
		}
		
	}
}
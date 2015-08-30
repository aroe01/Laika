using Yousful.BL.Contracts;
using Yousful.DL.SQLite;

namespace Yousful.BL
{
	/// <summary>
	/// Represents a Task.
	/// </summary>
	public class User : IBusinessEntity
	{
		public User ()
		{
			//PhotoUrls = "[]";
		}

		[PrimaryKey]
        public int UserID { get; set; }
		public string Name { get; set; }
		//public string Date { get; set; }
		//public string Notes { get; set; }
		//public double Lat { get; set; }
		//public double Long { get; set; }
		// new property
		//public bool Done { get; set; }
		//public double Heat { get; set; }
		//public string PhotoUrls {get; set;}
	}
}


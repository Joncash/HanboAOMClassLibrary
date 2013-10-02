using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Hanbo.SDMS.Model
{
	public partial class SDMSDataContext
	{
		partial void OnCreated()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["SDMSConnString"].ToString();
			this.Connection.ConnectionString = connectionString;
		}
	}
}

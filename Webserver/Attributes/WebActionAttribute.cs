using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.HttpServer {
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true, Inherited=false)]
	public sealed class WebActionAttribute : Attribute {}
}

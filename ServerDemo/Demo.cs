using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TomiSoft.Web.HttpServer;

namespace ServerDemo {
	[WebController]
	public class Demo {
		private WebClient client;

		public Demo(WebClient client) {
			this.client = client;
		}

		[WebAction]
		public void Index(Dictionary<string, string> Parameters) {
			Console.WriteLine("WebClient invoked this method with the following parameters:");

			foreach (var item in Parameters) {
				Console.WriteLine("   {0}: {1}", item.Key, item.Value);
			}

			HttpHeader h = new HttpHeader(HttpStatus.Ok, ProtocolVersion.Http1_1);
			h.SetParameter("Content-Type", "text/html; charset=utf-8");

			this.client.Send(h, "<h1>Működik</h1>");
		}
	}
}

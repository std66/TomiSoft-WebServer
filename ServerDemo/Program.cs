using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TomiSoft.Web.HttpServer;

namespace ServerDemo {
	public class Program {
		public static void Main(string[] args) {
			WebServer Server = new WebServer(typeof(Program).Assembly, "demo", "index");
			
			while (true) {
				Thread.Sleep(1000);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Dynamic;

namespace TomiSoft.Web.HttpServer {
	public class WebServer {
		private Socket ServerSocket;
		private static dynamic parameters = new ExpandoObject();

		public static dynamic Parameters {
			get {
				return WebServer.parameters;
			}
		}

		public WebServer(Assembly ApplicationAssembly, string DefaultController, string DefaultAction) : this(ApplicationAssembly, DefaultController, DefaultAction, 80) { }
		
		public WebServer(Assembly ApplicationAssembly, string DefaultController, string DefaultAction, int Port) {
			if (ApplicationAssembly == null)
				throw new ArgumentNullException("ApplicationAssembly must be set");
			
			this.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.ServerSocket.Bind(new IPEndPoint(IPAddress.Loopback, Port));
			this.ServerSocket.Listen(1);

			WebServer.Parameters.DefaultAssembly = ApplicationAssembly.GetName().Name;
			WebServer.Parameters.DefaultController = DefaultController;
			WebServer.Parameters.DefaultAction = DefaultAction;

			Thread t = new Thread(this.ListenThread);
			t.Start();
		}

		private void ListenThread() {
			while (true) {
				Socket Client = this.ServerSocket.Accept();

				if (Client != null) {
					WebClient cl = new WebClient(Client);
				}
			}
		}
	}
}

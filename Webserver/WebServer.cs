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
	public class WebServer : IDisposable {
		private volatile Socket ServerSocket;
		private static dynamic parameters = new ExpandoObject();
		private Thread ListenerThread;

		/// <summary>
		/// Gets the ExpandoObject instance that stores all data associated
		/// to the webserver. The following parameters are used by the webserver:
		/// WebServer.Parameters.DefaultAssembly, WebServer.Parameters.DefaultController,
		/// WebServer.Parameters.DefaultAction.
		/// </summary>
		public static dynamic Parameters {
			get {
				return WebServer.parameters;
			}
		}

		/// <summary>
		/// Creates a new WebServer instance that listens on port 80.
		/// </summary>
		/// <param name="ApplicationAssembly">The assembly that contains all the controller classes</param>
		/// <param name="DefaultController">The default controller class' name</param>
		/// <param name="DefaultAction">The default action method's name</param>
		public WebServer(Assembly ApplicationAssembly, string DefaultController, string DefaultAction) : this(ApplicationAssembly, DefaultController, DefaultAction, 80) { }
		
		/// <summary>
		/// Creates a new WebServer instance that listens on a specific port.
		/// </summary>
		/// <param name="ApplicationAssembly">The assembly that contains all the controller classes</param>
		/// <param name="DefaultController">The default controller class' name</param>
		/// <param name="DefaultAction">The default action method's name</param>
		/// <param name="Port">The port number on which the webserver listens for incoming connections</param>
		public WebServer(Assembly ApplicationAssembly, string DefaultController, string DefaultAction, int Port) {
			if (ApplicationAssembly == null)
				throw new ArgumentNullException("ApplicationAssembly must be set");
			
			this.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.ServerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
			this.ServerSocket.Listen(1);

			WebServer.Parameters.DefaultAssembly = ApplicationAssembly.GetName().Name;
			WebServer.Parameters.DefaultController = DefaultController;
			WebServer.Parameters.DefaultAction = DefaultAction;

			this.ListenerThread = new Thread(this.ListenThread);
			this.ListenerThread.Start();
		}

		private void ListenThread() {
			try {
				while (true) {
					Socket Client = this.ServerSocket.Accept();

					if (Client != null) {
						WebClient cl = new WebClient(Client);
					}
				}
			}
			catch (ThreadAbortException e) {
				this.ServerSocket.Close();
			}
		}

		/// <summary>
		/// Stops the webserver.
		/// </summary>
		public void Dispose() {
			this.ListenerThread.Abort();
		}
	}
}

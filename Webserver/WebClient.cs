using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.IO;

namespace TomiSoft.Web.HttpServer {
	public class WebClient {
		private Socket Client;
		private bool SetCookie = false;
		private Session session = null;
		private Dictionary<string, string> cookies;

		public Session Session {
			get {
				return this.session;
			}
		}

		public Dictionary<string, string> Cookies {
			get {
				return this.cookies;
			}
		}

		public WebClient(Socket ClientConnection) {
			this.Client = ClientConnection;

			Thread t = new Thread(this.ListenClientThread);
			t.Start();
		}

		private void ListenClientThread() {
			while (true) {
				if (this.Client.Available > 0) {
					byte[] Data = new byte[this.Client.Available];
					this.Client.Receive(Data);

					RequestParser parser = new RequestParser(new String(Encoding.UTF8.GetChars(Data)), this.Client);
					this.cookies = parser.Cookies;

					if (parser.Cookies.ContainsKey("sessionid")) {
						int SessionID = Convert.ToInt32(parser.Cookies["sessionid"]);
						this.session = Sessions.GetSession(SessionID);
					}

					UriParser uri = new UriParser(parser.Resource);

					if (uri.Resource == "") {
						IDictionary<string, object> Params = (IDictionary<string, object>)WebServer.Parameters;
						if (!Params.ContainsKey("DefaultController") || !Params.ContainsKey("DefaultAction"))
							throw new HttpException(HttpStatus.InternalServerError, ProtocolVersion.Http1_1, this, "DefaultController and DefaultAction must be set in WebServer.Parameters");

						this.Redirect(WebServer.Parameters.DefaultController, WebServer.Parameters.DefaultAction);
						return;
					}

					try {
						string[] Parts = uri.Resource.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						this.InvokeController(Parts[0], Parts[1], uri.Parameters);
					}
					catch {}
				}

				Thread.Sleep(50);
			}
		}

		private void InvokeController(string Controller, string Action, Dictionary<string, string> Parameters) {
			string TypeName = WebServer.Parameters.DefaultAssembly + "." + Utils.FirstCharToUpper(Controller) + ", " + WebServer.Parameters.DefaultAssembly;

			Type ControllerClass = Type.GetType(TypeName);
			if (ControllerClass == null || Attribute.GetCustomAttribute(ControllerClass, typeof(WebControllerAttribute)) == null)
				throw new HttpException(HttpStatus.NotFound, ProtocolVersion.Http1_1, this, "The requested controller not found in assembly " + WebServer.Parameters.DefaultAssembly);

			string MethodName = Utils.FirstCharToUpper(Action);
			MethodInfo info = ControllerClass.GetMethod(MethodName);

			if (info == null || Attribute.GetCustomAttribute(info, typeof(WebActionAttribute)) == null)
				throw new HttpException(HttpStatus.NotFound, ProtocolVersion.Http1_1, this, "The requested action not found in the requested controller.");

			object ControllerInstance = ControllerClass.GetConstructor(new Type[] {this.GetType()}).Invoke(new object[] { this });
			info.Invoke(ControllerInstance, new object[] { Parameters });
		}

		public void Send(HttpHeader Header, string Content) {
			byte[] resp = Encoding.UTF8.GetBytes(Content);

			Header.SetParameter("Content-Length", resp.Length.ToString());
			
			byte[] h = Encoding.UTF8.GetBytes(Header.ToString());
			
			this.Client.Send(h);
			this.Client.Send(resp);
		}

		public void Send(HttpHeader Header, byte[] Content) {
			Header.SetParameter("Content-Length", Content.Length.ToString());

			byte[] h = Encoding.UTF8.GetBytes(Header.ToString());

			this.Client.Send(h);
			this.Client.Send(Content);
		}

		public void Send(HttpHeader Header, Stream Content) {
			Header.SetParameter("Content-Length", Content.Length.ToString());

			byte[] h = Encoding.UTF8.GetBytes(Header.ToString());

			this.Client.Send(h);

			byte[] Buffer = new byte[512];
			while (Content.Position != Content.Length) {
				Content.Read(Buffer, 0, Buffer.Length);
				this.Client.Send(Buffer, SocketFlags.Partial);
			}
		}

		public void Redirect(string Controller, string Action) {
			HttpHeader h = new HttpHeader(HttpStatus.Found, ProtocolVersion.Http1_1);
			h.SetParameter("Location", "/" + Controller + "/" + Action);

			byte[] Header = Encoding.UTF8.GetBytes(h.ToString());
			this.Client.Send(Header);
		}

		public void StartSession(HttpHeader Header, DateTime Expires) {
			if (Header == null)
				throw new ArgumentNullException("Header must be set");

			this.session = Sessions.StartSession();
			Header.SetCookie("sessionid", this.session.SessionID.ToString());
			Header.CookieExpires = Expires;
		}
	}
}
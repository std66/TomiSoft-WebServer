using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.HttpServer {
	public class HttpException : Exception {
		public HttpException(HttpStatus Status, ProtocolVersion HttpVersion, WebClient Client, string Message)
			: base(Message) {

				StringBuilder sb = new StringBuilder();
				sb.Append(HttpHeader.GetHttpStatusCode(Status).ToString() + " ");
				sb.Append(HttpHeader.GetHttpStatusMessage(Status) + "<br>" + this.Message);
				sb.Append("<hr><i>TomiSoft WebServer</i>");

				HttpHeader h = new HttpHeader(Status, HttpVersion);
				h.SetParameter("Content-Type", "text/html; charset=utf-8");

				Client.Send(h, sb.ToString());
		}

		public HttpException(HttpStatus Status, ProtocolVersion HttpVersion, Socket Client, string Message)
			: base(Message) {

			StringBuilder sb = new StringBuilder();
			sb.Append(HttpHeader.GetHttpStatusCode(Status).ToString() + " ");
			sb.Append(HttpHeader.GetHttpStatusMessage(Status) + "<br>" + this.Message);
			sb.Append("<hr><i>TomiSoft WebServer</i>");

			byte[] Response = Encoding.UTF8.GetBytes(sb.ToString());

			HttpHeader h = new HttpHeader(Status, HttpVersion);
			h.SetParameter("Content-Type", "text/html; charset=utf-8");
			h.SetParameter("Content-Length", Response.Length.ToString());

			byte[] Header = Encoding.UTF8.GetBytes(h.ToString());

			Client.Send(Header);
			Client.Send(Response);
		}
	}
}

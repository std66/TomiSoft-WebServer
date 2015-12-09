using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.HttpServer {
	public enum RequestMethod {
		Get, Post
	}

	public enum ProtocolVersion {
		Http1_0, Http1_1
	}

	public class RequestParser {
		private RequestMethod method;
		private string resource;
		private ProtocolVersion version;
		private Dictionary<string, string> Params = new Dictionary<string, string>();
		private Dictionary<string, string> cookies = new Dictionary<string, string>();

		public ProtocolVersion HttpVersion {
			get {
				return this.version;
			}
		}

		public string Resource {
			get {
				return this.resource;
			}
		}

		public RequestMethod Method {
			get {
				return this.method;
			}
		}

		public Dictionary<string, string> Cookies {
			get {
				return this.cookies;
			}
		}

		public RequestParser(string Request, Socket Client) {
			string[] Lines = Request.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			string[] RequestParts = Lines[0].Split(' ');
			switch (RequestParts[0]) {
				case "GET":
					this.method = RequestMethod.Get;
					break;

				case "POST":
					this.method = RequestMethod.Post;
					break;

				default:
					throw new HttpException(HttpStatus.MethodNotAllowed, ProtocolVersion.Http1_1, Client, RequestParts[0] + " type requests not supported");
			}

			this.resource = RequestParts[1];

			switch (RequestParts[2]) {
				case "HTTP/1.0":
					this.version = ProtocolVersion.Http1_0;
					break;

				case "HTTP/1.1":
					this.version = ProtocolVersion.Http1_1;
					break;

				default:
					throw new HttpException(HttpStatus.HttpVersionNotSupported, ProtocolVersion.Http1_1, Client, RequestParts[2] + " protocol not supported");
			}

			for (int i = 1; i < Lines.Length; i++) {
				string[] Parts = Lines[i].Split(":".ToCharArray());

				if (Parts.Length == 2)
					this.Params[Parts[0]] = Parts[1];
			}

			if (this.Params.ContainsKey("Cookie")) {
				foreach (var item in this.Params["Cookie"].Split(';')) {
					string[] Parts = item.Split('=');

					this.cookies[Parts[0].Trim()] = Parts[1].Trim();
				}
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.HttpServer {
	public enum HttpStatus {
		Ok = 0, Found, BadRequest, Unauthorized, NotFound, MethodNotAllowed, InternalServerError, NotImplemented, ServiceUnavailable, HttpVersionNotSupported
	}

	public class HttpHeader {
		private static int[] HttpCodes = { 200, 302, 400, 401, 404, 405, 500, 501, 503, 505 };
		private static string[] HttpMessages = {
												   "OK", "Found", "Bad Request", "Unauthorized", "Not Found", "Method Not allowed", "Internal Server Error",
												   "Not Implemented", "Service Unavailable", "HTTP Version Not Supported"
											   };

		
		private string HttpResult = "";
		private Dictionary<string, string> parameters = new Dictionary<string, string>();
		private Dictionary<string, string> setcookies = new Dictionary<string, string>();
		
		public DateTime CookieExpires { get; set; }

		public HttpHeader(HttpStatus Status, ProtocolVersion HttpVersion) {
			int Index = (int)Status;

			int statusCode = HttpHeader.HttpCodes[Index];
			string statusMessage = HttpHeader.HttpMessages[Index];

			switch (HttpVersion) {
				case ProtocolVersion.Http1_0:
					HttpResult += "HTTP/1.0 ";
					break;

				case ProtocolVersion.Http1_1:
					HttpResult += "HTTP/1.1 ";
					break;
			}

			HttpResult += statusCode + " " + statusMessage;
		}

		public void SetParameter(string Parameter, string Value) {
			this.parameters[Parameter] = Value;
		}

		public void SetCookie(string Name, string Value) {
			this.setcookies.Add(Name, Value);
		}

		public override string ToString() {
			this.InsertCookies();

			string Result = this.HttpResult;

			foreach (var item in this.parameters) {
				Result += "\r\n" + item.Key + ": " + item.Value;
			}

			return Result + "\r\n\r\n";
		}

		public static int GetHttpStatusCode(HttpStatus Status) {
			return HttpCodes[(int)Status];
		}

		public static string GetHttpStatusMessage(HttpStatus Status) {
			return HttpMessages[(int)Status];
		}

		private void InsertCookies() {
			if (this.setcookies.Count == 0)
				return;

			StringBuilder sb = new StringBuilder();
			sb.Append(String.Join("; ", this.setcookies.Select(x => String.Format("{0}={1}", x.Key, x.Value))));

			if (this.CookieExpires != null)
				sb.AppendFormat("; expires={0}", this.CookieExpires.ToString("R"));

			this.parameters.Add("Set-Cookie", sb.ToString());
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.HttpServer {
	class UriParser {
		private string resource;
		private Dictionary<string, string> GetParameters;

		public bool HasGetParameters {
			get {
				return this.GetParameters != null;
			}
		}

		public string Resource {
			get {
				return this.resource;
			}
		}

		public Dictionary<string, string> Parameters {
			get {
				return this.GetParameters;
			}
		}

		public UriParser(string Uri) {
			string[] Parts = Uri.Split('?');
			this.GetParameters = new Dictionary<string, string>();

			this.resource = Parts[0].Substring(1);

			if (Parts.Length == 2) {
				string[] Parameters = Parts[1].Split('&');

				foreach (var item in Parameters) {
					string[] Param = item.Split('=');
					this.GetParameters[Param[0]] = Param[1];
				}
			}
		}
	}
}

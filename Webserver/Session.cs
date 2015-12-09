using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.HttpServer {
	public class Session {
		private Dictionary<string, string> parameters = new Dictionary<string, string>();
		private int sessionID;

		public int SessionID {
			get {
				return this.sessionID;
			}
		}

		public Dictionary<string, string> Parameters {
			get {
				return this.parameters;
			}
		}

		public Session(int SessionID) {
			this.sessionID = SessionID;
		}
	}
}

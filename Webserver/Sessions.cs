using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.HttpServer {
	public static class Sessions {
		private static ArrayList sessions = new ArrayList();

		public static Session StartSession() {
			int SessionID = Sessions.sessions.Count;
			Session Result = new Session(SessionID);
			Sessions.sessions.Add(Result);

			return Result;
		}

		public static Session StartSession(int SessionID) {
			Session Result = new Session(SessionID);
			Sessions.sessions.Add(Result);

			return Result;
		}

		public static void DestroySession(int SessionID) {
			for (int i = 0; i < Sessions.sessions.Count; i++) {
				Session s = Sessions.sessions[i] as Session;

				if (s.SessionID == SessionID) {
					Sessions.sessions.RemoveAt(i);
					return;
				}
			}
		}

		public static Session GetSession(int SessionID) {
			foreach (Session s in Sessions.sessions) {
				if (s.SessionID == SessionID)
					return s;
			}

			return null;
		}
	}
}

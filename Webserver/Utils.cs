using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.HttpServer {
	class Utils {
		public static string FirstCharToUpper(string s) {
			char[] chars = s.ToCharArray();
			chars[0] = Char.ToUpper(chars[0]);

			return new String(chars);
		}
	}
}

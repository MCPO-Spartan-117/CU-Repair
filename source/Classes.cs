using System.Collections.Generic;

namespace MCPO {
	public record class YAMLConf {
		public record class Banned {
			public List<string> liq {get; set;}
			public List<string> liqqua {get; set;}
			public List<string> item {get; set;}
			public List<string> itemqua {get; set;}
			public List<string> repair {get; set;}

			public Banned() {
				liq = [];
				liqqua = [];
				item = [];
				itemqua = ["hammering", "cutting"];
				repair = [];
			}
		}

		public Banned banned {get; set;}

		public YAMLConf() {
			banned = new();
		}
	}
}

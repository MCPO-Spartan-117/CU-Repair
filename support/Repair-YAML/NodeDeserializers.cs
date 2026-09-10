using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectFactories;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace MCPO {
	//https://github.com/aaubry/YamlDotNet/issues/443
	public sealed class RemoveNull : INodeDeserializer {
		private readonly IObjectFactory objectFactory = new DefaultObjectFactory();

		public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value, ObjectDeserializer rootDeserializer) {
			value = null;

			if(parser.Accept<NodeEvent>(out var evt)) {
				if(NodeIsNull(evt)) {
					parser.SkipThisAndNestedEvents();
					value = objectFactory.Create(expectedType);
					return true;
				}
			}
			return false;
		}

		private bool NodeIsNull(NodeEvent nodeEvent) {
			// http://yaml.org/type/null.html

			if (nodeEvent.Tag == "tag:yaml.org,2002:null") {
				return true;
			}

			if (nodeEvent is Scalar scalar && scalar.Style == ScalarStyle.Plain) {
				var value = scalar.Value;
				return value == "" || value == "~" || value == "null" || value == "Null" || value == "NULL";
			}

			return false;
		}
	}
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<PagerDutyEvent>))]
	public interface IPagerDutyEvent
	{
		[JsonProperty("account")]
		string Account { get; set; }

		[JsonProperty("description")]
		string Description { get; set; }

		[JsonProperty("event_type")]
		PagerDutyEventType? EventType { get; set; }

		[JsonProperty("incident_key")]
		string IncidentKey { get; set; }

		[JsonProperty("client")]
		string Client { get; set; }

		[JsonProperty("client_url")]
		string ClientUrl { get; set; }

		[JsonProperty("attach_payload")]
		bool? AttachPayload { get; set; }

		[JsonProperty("context")]
		IEnumerable<IPagerDutyContext> Context { get; set; }
	}
}
using System.Collections.Generic;

namespace Nest
{
	public class PagerDutyEvent : IPagerDutyEvent
	{
		public string Account { get; set; }

		public string Description { get; set; }

		public PagerDutyEventType? EventType { get; set; }

		public string IncidentKey { get; set; }

		public string Client { get; set; }

		public string ClientUrl { get; set; }

		public bool? AttachPayload { get; set; }

		public IEnumerable<IPagerDutyContext> Context { get; set; }
	}
}

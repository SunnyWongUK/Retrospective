using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreMVCForjQueryAjax.Models
{
	class CompareRetrospectives : IComparer<Retrospective>
	{
		public int Compare(Retrospective obj1, Retrospective obj2)
		{
			string s1 = obj1.Name;
			string s2 = obj2.Name;
			return s1.CompareTo(s2);
		}
	}

	public class ItemList
	{
		public string Name { get; set; }
	}

	public class Result 
	{
		public string Message { get; set; }
	}

	public class SearchCriteria
	{
		public string FromDate { get; set; }
		public string ToDate { get; set; }
	}

	public class RetrospectiveDto
	{
		public string Name { get; set; }
		public string Summary { get; set; }
		public string Date { get; set; }
		public string Participants { get; set; }
		public List<string> ParticipantList { get; set; }
		public string NameOfPerson { get; set; }
		public string Body { get; set; }
		public string FeedbackType { get; set; }

		public static List<RetrospectiveDto> Map(List<Retrospective> retrospectives)
		{
			IEnumerable<RetrospectiveDto> dto = retrospectives.SelectMany(r => r.Feedbacks.DefaultIfEmpty(), (r, e) => new RetrospectiveDto()
			{
				Name = r.Name,
				Summary = r.Summary,
				Date = r.Date.ToString("yyyy/MM/dd"),
				Participants = String.Join(",", r.Participants.Select(p => p)),
				NameOfPerson = e?.NameOfPerson,
				Body = e?.Body,
				FeedbackType = e?.FeedbackType.ToString()
			});
			return dto.ToList();
		}
	}

	public enum FeedbackTypes
	{
		Positive = 1,
		Negative = 2,
		Idea = 3,
		Praise = 4
	}

	public class Retrospective
	{
		public string Name { get; set; }
		public string Summary { get; set; }
		public DateTime Date { get; set; }
		public List<string> Participants { get; set; }
		public List<Feedback> Feedbacks { get; set; }
	}

	public class Feedback
	{
		public string Name { get; set; }
		public string NameOfPerson { get; set; }
		public string Body { get; set; }
		public FeedbackTypes FeedbackType { get; set; }
	}
}

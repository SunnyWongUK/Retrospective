using ASPNETCoreMVCForjQueryAjax.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ASPNETCoreMVCForjQueryAjax.Controllers
{								   
	[ApiController]
	public class APIController : Controller
	{
		[Route("/API/View")]
		[HttpPost]	
		public IEnumerable<RetrospectiveDto> ViewRetrospectives() 
		{
			List<RetrospectiveDto> dtos;

			var stream = HttpContext.Request.HttpContext;
			StreamReader reader = new StreamReader(HttpContext.Request.Body, System.Text.Encoding.UTF8);
			var data = reader.ReadToEndAsync();
			var json = data.Result;
			var dto = JsonConvert.DeserializeObject<SearchCriteria>(json);

			var a = HttpContext.Session.GetString("Retrospectives");

			if (a != null)
			{
				var b = JsonConvert.DeserializeObject<List<Retrospective>>(a);
				var c = b.Where(r => dto.FromDate == "" || r.Date >= Convert.ToDateTime(dto.FromDate)).Where(r => dto.ToDate == "" || r.Date <= Convert.ToDateTime(dto.ToDate)).ToList();
				dtos = RetrospectiveDto.Map(c);
			}
			else 
			{
				dtos = new List<RetrospectiveDto>();
			}		

			return dtos;
		}

		[Route("/API/RetrospectiveNames")]
		[HttpPost]
		public IEnumerable<ItemList> RetrospectiveNames()
		{
			List<ItemList> dtos;				

			var a = HttpContext.Session.GetString("Retrospectives");			
			dtos = a == null ? new List<ItemList>() : JsonConvert.DeserializeObject<List<Retrospective>>(a).Select(r => new ItemList() { Name = r.Name }).ToList();
			return dtos;
		}

		[Route("/API/Add")]
		[HttpPost]
		public Result Add()
		{
			Result result = new Result();

			try
			{
				var stream = HttpContext.Request.HttpContext;
				StreamReader reader = new StreamReader(HttpContext.Request.Body, System.Text.Encoding.UTF8);
				var data = reader.ReadToEndAsync();
				var json = data.Result;
				var dto = JsonConvert.DeserializeObject<RetrospectiveDto>(json);
				Retrospective retrospective; 
				List<Retrospective> retrospectives;

				var a = HttpContext.Session.GetString("Retrospectives");
				retrospectives = a == null ? new List<Retrospective>() : JsonConvert.DeserializeObject<List<Retrospective>>(a);

				retrospective = new Retrospective()
				{
					Name = dto.Name,
					Summary = dto.Summary,
					Date = Convert.ToDateTime(dto.Date),
					Participants = dto.ParticipantList,
					Feedbacks = new List<Feedback>()
				};							

				int index = retrospectives.BinarySearch(retrospective, new CompareRetrospectives());
				if (index < 0)
				{
					retrospectives.Insert(~index, retrospective);
				}
				else 
				{
					result.Message = "The record has existed and was fail being added.";
					return result;
				};							

				HttpContext.Session.SetString("Retrospectives", JsonConvert.SerializeObject(retrospectives));

				result.Message = "The record has been added successfully.";
			}
			catch (Exception e)
			{
				result.Message = "The record was fail being added.";
				Console.WriteLine(e.Message);
			};

			return result;
		}

		[Route("/API/AddFeedback")]
		[HttpPost]
		public Result AddFeedback()
		{
			Result result = new Result();

			try
			{
				var stream = HttpContext.Request.HttpContext;
				StreamReader reader = new StreamReader(HttpContext.Request.Body, System.Text.Encoding.UTF8);
				var data = reader.ReadToEndAsync();
				var json = data.Result;
				var dto = JsonConvert.DeserializeObject<RetrospectiveDto>(json);
				Retrospective retrospective;
				List<Retrospective> retrospectives;

				var a = HttpContext.Session.GetString("Retrospectives");
				retrospectives = a == null ? new List<Retrospective>() : JsonConvert.DeserializeObject<List<Retrospective>>(a);

				retrospective = new Retrospective()
				{
					Name = dto.Name,					
				};

				int index = retrospectives.BinarySearch(retrospective, new CompareRetrospectives());
				if (index < 0)
				{
					result.Message = "The record does not exist.";
					return result;
				}
				else
				{
					retrospective = retrospectives[index];
					retrospective.Feedbacks.Add(new Feedback()
					{
						Name = dto.Name,
						NameOfPerson = dto.NameOfPerson,
						Body = dto.Body,
						FeedbackType = (FeedbackTypes)Enum.Parse(typeof(FeedbackTypes), dto.FeedbackType)
					});
					retrospectives[index] = retrospective;
				};

				HttpContext.Session.SetString("Retrospectives", JsonConvert.SerializeObject(retrospectives));

				result.Message = "The feedback has been added successfully.";
			}
			catch (Exception e)
			{
				result.Message = "The feedback was fail being added.";
				Console.WriteLine(e.Message);
			};

			return result;
		}

		[Route("/API/FeedbackTypes")]
		[HttpPost]
		public IEnumerable<ItemList> FeedbackTypes()
		{
			List<ItemList> dtos = new List<ItemList>();
			foreach (int i in Enum.GetValues(typeof(FeedbackTypes)))
			{
				dtos.Add(new ItemList() { Name = ((FeedbackTypes)i).ToString() });
			}
			return dtos;
		}
	}

	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}
		
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TASVideos.Data;
using TASVideos.Data.Entity;

namespace TASVideos.Pages.Publications
{
	[RequirePermission(PermissionTo.EditPublicationFiles)]
	public class EditUrlsModel : BasePageModel
	{
		private readonly ApplicationDbContext _db;

		private static List<PublicationUrlType> PublicaitonUrlTypes = Enum
			.GetValues(typeof(PublicationUrlType))
			.Cast<PublicationUrlType>()
			.ToList();

		public EditUrlsModel(ApplicationDbContext db)
		{
			_db = db;
		}

		public IEnumerable<SelectListItem> AvailableTypes =
			PublicaitonUrlTypes
				.Select(t => new SelectListItem
				{
					Text = t.ToString(),
					Value = ((int)t).ToString()
				});

		[FromRoute]
		public int Id { get; set; }

		[BindProperty]
		public string Title { get; set; } = "";

		public ICollection<PublicationUrl> CurrentUrls { get; set; } = new List<PublicationUrl>();

		[Required]
		[BindProperty]
		[Url]
		[Display(Name = "Url")]
		public string PublicationUrl { get; set; } = "";

		[Required]
		[BindProperty]
		[Display(Name = "Type")]
		public PublicationUrlType UrlType { get; set; }

		public async Task<IActionResult> OnGet()
		{
			Title = await _db.Publications
				.Where(p => p.Id == Id)
				.Select(p => p.Title)
				.SingleOrDefaultAsync();

			if (Title == null)
			{
				return NotFound();
			}

			CurrentUrls = await _db.PublicationUrls
				.Where(u => u.PublicationId == Id)
				.ToListAsync();

			return Page();
		}

		public async Task<IActionResult> OnPost()
		{ 
			if (!ModelState.IsValid)
			{
				CurrentUrls = await _db.PublicationUrls
					.Where(u => u.PublicationId == Id)
					.ToListAsync();
			
				return Page();
			}

			_db.PublicationUrls.Add(new PublicationUrl
			{
				PublicationId = Id,
				Url = PublicationUrl,
				Type = UrlType
			});

			await _db.SaveChangesAsync();

			return RedirectToPage("EditUrls", new { Id });
		}

		public async Task<IActionResult> OnPostDelete(int publicationUrlId)
		{
			var url = await _db.PublicationUrls
				.SingleOrDefaultAsync(pf => pf.Id == publicationUrlId);

			_db.PublicationUrls.Remove(url);

			// TODO: catch update exceptions
			await _db.SaveChangesAsync();

			return RedirectToPage("EditUrls", new { Id });
		}

	}
}

﻿using Microsoft.AspNetCore.Mvc;
using TASVideos.Core.Services.Wiki;
using TASVideos.WikiEngine;

namespace TASVideos.ViewComponents.TODO;

[WikiModule(WikiModules.WikiOrphans)]
public class WikiOrphans : ViewComponent
{
	private readonly IWikiPages _wikiPages;

	public WikiOrphans(IWikiPages wikiPages)
	{
		_wikiPages = wikiPages;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var orphans = await _wikiPages.Orphans();
		return View(orphans);
	}
}

using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class UstSoyViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}

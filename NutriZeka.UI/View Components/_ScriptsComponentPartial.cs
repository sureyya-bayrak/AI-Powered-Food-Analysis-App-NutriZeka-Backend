using Microsoft.AspNetCore.Mvc;

namespace NutriZeka.UI.View_Components
{

    public class _ScriptsComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}


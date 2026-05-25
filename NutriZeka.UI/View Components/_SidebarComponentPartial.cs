using Microsoft.AspNetCore.Mvc;

namespace NutriZeka.UI.View_Components
{

    public class _SidebarComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}


using Microsoft.AspNetCore.Mvc;

namespace NutriZeka.UI.View_Components
{
    
        public class _HeadComponentPartial : ViewComponent
        {
            public IViewComponentResult Invoke()
            {
                return View();
            }
        }
    }


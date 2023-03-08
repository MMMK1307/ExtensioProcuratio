using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Enumerators;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExtensioProcuratio.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(IProjectRepository projectRepository, UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index() => View(await _projectRepository.List());

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyProjects()
        { 
            var userId = await GetUserId();
            return View(await _projectRepository.ListUserProjects(userId));
        } 

        public IActionResult Create() => View();

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectModel project)
        {
            if (String.IsNullOrEmpty(project.Name) || String.IsNullOrEmpty(project.Description))
                return View();

            project.Id = Guid.NewGuid().ToString();
            project.DateCreated = DateTime.Now;
            project.Status = ProjectStatus.Started;
            project.UserId = await GetUserId();

            await _projectRepository.Create(project);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var project = await _projectRepository.ListProjectById(id);

            if(project == null)
                return NotFound();

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectModel project)
        {
            if (!IsModelValid(project))
                return View();

            if (!await IsUserOwner(project.Id))
                return View();
            
            project.UserId = await GetUserId();
            project.DateUpdated = DateTime.Now;

            await _projectRepository.Update(project);

            return RedirectToAction("MyProjects");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var project = await _projectRepository.ListProjectById(id);

            if (project.UserId != await GetUserId())
                return NotFound();

            await _projectRepository.Delete(project);
            return RedirectToAction("MyProjects");
        }

        private bool IsModelValid(ProjectModel project)
        {
            bool check = project.Id is not null && project.Name is not null
                && project.Description is not null;

            return check;
        }

        private async Task<string> GetUserId()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }

        private async Task<bool> IsUserOwner(string projectId)
        {
            var project = await _projectRepository.ListProjectById(projectId);
            return project.UserId ==  await GetUserId();
        }
    }
}

using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Enumerators;
using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExtensioProcuratio.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ProjectController(IProjectRepository projectRepository, UserManager<ApplicationUser> userManager, IDateTimeProvider dateTimeProvider)
            : base(projectRepository, userManager)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Index() => View(await _projectRepository.List());

        [HttpGet]
        public async Task<IActionResult> MyProjects()
        {
            var userId = await GetUserId();
            return View(await _projectRepository.ListUserProjects(userId));
        }

        public async Task<IActionResult> Create()
        {
            if (!await IsUserWithinProjectLimit())
            {
                return RedirectToAction(nameof(LimitReached));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectModel project)
        {
            if (string.IsNullOrEmpty(project.Name) || string.IsNullOrEmpty(project.Description))
            {
                return View();
            }

            if (!await IsUserWithinProjectLimit())
            {
                return RedirectToAction(nameof(LimitReached));
            }

            project.Id = new ProjectId(Guid.NewGuid().ToString());
            project.DateCreated = _dateTimeProvider.GetBrazil();
            project.UserId = await GetUserId();

            await _projectRepository.Create(project);
            await _projectRepository.AddAssociateUser(
                new ProjectAssociatesModel(project.UserId, project.Id));

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(ProjectId id)
        {
            var project = await _projectRepository.ListProjectById(id);

            if (!await IsUserAssociate(project.Id))
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectModel project)
        {
            if (!IsModelValid(project))
            {
                return View();
            }

            if (!await IsUserAssociate(project.Id))
            {
                return NotFound();
            }

            project.UserId = await GetUserId();
            project.DateUpdated = _dateTimeProvider.GetBrazil();

            await _projectRepository.Update(project);

            return RedirectToAction("MyProjects");
        }

        public async Task<IActionResult> AdoptionRequest(ProjectId projectId)
        {
            var project = await _projectRepository.ListProjectById(projectId);

            if (!await IsUserAssociate(projectId))
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> AdoptionConfirmation(ProjectId projectId)
        {
            var project = await _projectRepository.ListProjectById(projectId);

            if (!await IsUserAssociate(projectId))
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> SendAdoptionConfirmation(ProjectId projectId)
        {
            await Task.CompletedTask;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult LimitReached()
        {
            return View();
        }

        public async Task<IActionResult> Delete(ProjectId id)
        {
            var project = await _projectRepository.ListProjectById(id);

            if (project.UserId != await GetUserId())
            {
                return NotFound();
            }

            project.Status = ProjectStatus.Hidden;

            await _projectRepository.Update(project);

            return RedirectToAction("MyProjects");
        }

        private static bool IsModelValid(ProjectModel project)
        {
            bool check = project.Name is not null
                && project.Description is not null;

            return check;
        }

        private async Task<bool> IsUserOwner(ProjectId projectId)
        {
            var project = await _projectRepository.ListProjectById(projectId);
            return await GetUserId() == project.UserId;
        }

        private async Task<bool> IsUserAssociate(ProjectId projectId)
        {
            var associatedUsers = await _projectRepository.ListProjectAssociates(projectId);
            var loggedUserId = await GetUserId();

            foreach(var user in associatedUsers)
            {
                if (user == loggedUserId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
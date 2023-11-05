using ExtensioProcuratio.App.Project.Commands.Create;
using ExtensioProcuratio.App.Project.Commands.Delete;
using ExtensioProcuratio.App.Project.Commands.Update;
using ExtensioProcuratio.App.Project.Queries.GetAll;
using ExtensioProcuratio.App.Project.Queries.GetById;
using ExtensioProcuratio.App.Project.Queries.GetByUser;
using ExtensioProcuratio.App.ProjectAssociates.Queries.GetByProject;
using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExtensioProcuratio.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        private readonly ISender _mediator;

        public ProjectController(UserManager<ApplicationUser> userManager, ISender mediator)
            : base(userManager, mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index() => View(await _mediator.Send(new GetAllProjectsQuery()));

        [HttpGet]
        public async Task<IActionResult> MyProjects()
        {
            var userId = await GetUserId();
            return View(await _mediator.Send(new GetProjectByUserQuery(userId)));
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

            project.UserId = await GetUserId();

            await _mediator.Send(project.Adapt<CreateProjectCommand>());

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(ProjectId id)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery(id));

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

            await _mediator.Send(project.Adapt<UpdateProjectCommand>());

            return RedirectToAction("MyProjects");
        }

        public async Task<IActionResult> AdoptionRequest(ProjectId projectId)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery(projectId));

            if (!await IsUserAssociate(projectId))
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> AdoptionConfirmation(ProjectId projectId)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery(projectId));

            if (!await IsUserAssociate(projectId))
            {
                return NotFound();
            }

            return View(project);
        }

        public IActionResult LimitReached()
        {
            return View();
        }

        public async Task<IActionResult> Delete(ProjectId id)
        {
            var result = await _mediator.Send(new DeleteProjectCommand(id, await GetUserId()));

            if (!result)
            {
                return NotFound();
            }

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
            var project = await _mediator.Send(new GetProjectByIdQuery(projectId));
            return await GetUserId() == project.UserId;
        }

        private async Task<bool> IsUserAssociate(ProjectId projectId)
        {
            var associatedUsers = await _mediator.Send(new GetAssociatesByProjectQuery(projectId));
            var loggedUserId = await GetUserId();

            foreach (var user in associatedUsers)
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
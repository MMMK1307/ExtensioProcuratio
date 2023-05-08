﻿using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace ExtensioProcuratio.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(IProjectRepository projectRepository, UserManager<ApplicationUser> userManager)
            : base(projectRepository, userManager)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
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

            project.Id = Guid.NewGuid().ToString();
            project.DateCreated = DateTime.Now;
            project.UserId = await GetUserId();

            await _projectRepository.Create(project);
            await _projectRepository.AddAssociateUser(
                new ProjectAssociatesModel(project.Id, project.UserId));

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var project = await _projectRepository.ListProjectById(id);

            if (project.Id == null)
            {
                return NotFound();
            }

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
            project.DateUpdated = DateTime.Now;

            await _projectRepository.Update(project);

            return RedirectToAction("MyProjects");
        }

        public async Task<IActionResult> AdoptionRequest(string projectId)
        {
            var project = await _projectRepository.ListProjectById(projectId);

            if (project.Id == null)
            {
                return NotFound();
            }

            if (!await IsUserAssociate(projectId))
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> AdoptionConfirmation(string projectId)
        {
            var project = await _projectRepository.ListProjectById(projectId);

            if (project.Id == null)
            {
                return NotFound();
            }

            if (!await IsUserAssociate(projectId))
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> SendAdoptionConfirmation(string projectId)
        {
            await Task.CompletedTask;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult LimitReached()
        {
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var project = await _projectRepository.ListProjectById(id);

            if (project.UserId != await GetUserId())
            {
                return NotFound();
            }

            await _projectRepository.Delete(project);

            return RedirectToAction("MyProjects");
        }

        private static bool IsModelValid(ProjectModel project)
        {
            bool check = project.Id is not null && project.Name is not null
                && project.Description is not null;

            return check;
        }

        private async Task<bool> IsUserOwner(string projectId)
        {
            var project = await _projectRepository.ListProjectById(projectId);
            return await GetUserId() == project.UserId;
        }

        private async Task<bool> IsUserAssociate(string projectId)
        {
            var associatedUsers = await _projectRepository.ListProjectAssociates(projectId);
            var loggedUserId = await GetUserId();

            if (associatedUsers.Contains(loggedUserId))
            {
                return true;
            }

            return false;
        }
    }
}
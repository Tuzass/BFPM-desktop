using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Pages;

public class ProjectEditPage
{
    public ProjectEditPage()
    {
        
    }

    public static async Task<LocalChange> Run(Project project)
    {
        if (project.ID is null)
        {
            throw new InexistentProjectException();
        }

        string name = project.Name;
        string description = project.Description;
        ProjectStatus status = project.Status;

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicates before returning

        var newStatus = status switch
        {
            ProjectStatus.Active => DomainEntityStatus.Active,
            ProjectStatus.OnHold => DomainEntityStatus.OnHold,
            ProjectStatus.Archived => DomainEntityStatus.Archived,
            _ => DomainEntityStatus.Completed
        };
        
        return new(DomainEntityType.Project, project.ID.Value, name, description, newStatus, null, null, false);
    }
}

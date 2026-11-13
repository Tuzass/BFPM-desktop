using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Workflow.Enums;
using Services.Exceptions;

namespace Services;

public static class SynchronizationService
{
    private static async Task<Dictionary<EntityIdPair, int>> GetEntityVersions(AppDbContext context, List<EntityIdPair> pairs)
    {
        Dictionary<EntityIdPair, int> entityVersions = [];

        foreach (var pair in pairs)
        {
            int version = await DomainService.GetEntityVersionByRemoteID(context, pair.EntityType, pair.ID);

            if (version == -1)
            {
                throw new InvalidOperationException($"Entity ({pair.EntityType}, {pair.ID}) not found in local DB");
            }

            entityVersions[pair] = version;
        }

        return entityVersions;
    }

    public static async Task<bool> CommitChanges(AppDbContext context, ApiClient apiClient, int userID, int projectID, ActionType actionType, DomainEntityType targetType, int targetID, DomainEntityType? relatedEntityType, int? relatedEntityID, Change change)
    {
        var affectedEntitiesResponse = await apiClient.CreateAction(userID,
                                                                    projectID,
                                                                    actionType,
                                                                    targetType,
                                                                    targetID,
                                                                    relatedEntityType,
                                                                    relatedEntityID,
                                                                    change);

        Dictionary<EntityIdPair, int> entityVersions = await GetEntityVersions(context, affectedEntitiesResponse.AffectedEntities);
        var response = await apiClient.SendVersions(affectedEntitiesResponse.UserActionID, entityVersions);

        return response switch
        {
            "Approved" => true,
            "Rejected" => false,
            _ => throw new InvalidOperationException($"Unexpected user action status: {response}")
        };
    }

    public static async Task<bool> DeleteProject(AppDbContext context, ApiClient apiClient, int userID, int projectID)
    {
        var affectedEntitiesResponse = await apiClient.CreateAction(userID, projectID, ActionType.DeleteEntity, DomainEntityType.Project, projectID, null, null, null);

        Dictionary<EntityIdPair, int> entityVersions = await GetEntityVersions(context, affectedEntitiesResponse.AffectedEntities);
        var response = await apiClient.SendVersions(affectedEntitiesResponse.UserActionID, entityVersions);

        return response switch
        {
            "Approved" => true,
            "Rejected" => false,
            _ => throw new InvalidOperationException($"Unexpected user action status: {response}")
        };
    }

    public static async Task PullChanges(AppDbContext context, ApiClient apiClient, int userID, DomainEntityType entityType, int entityID)
    {
        switch (entityType)
        {
            case DomainEntityType.Project:
                var project = await DomainService.GetProjectByID(context, entityID);
                var projectResponse = await apiClient.GetProject(project.RemoteID!.Value);

                Change change = new(userID, DomainEntityType.Project, entityID, null, null, null, null, null, false);
                bool anyChanged = false;

                if (project.Name != projectResponse.Name)
                {
                    change.Name = projectResponse.Name;
                    anyChanged = true;
                }

                if (project.Description != projectResponse.Description)
                {
                    change.Description = projectResponse.Description;
                    anyChanged = true;
                }

                if (project.Status != projectResponse.Status)
                {
                    change.Status = projectResponse.Status switch
                    {
                        ProjectStatus.Active => DomainEntityStatus.Active,
                        ProjectStatus.OnHold => DomainEntityStatus.OnHold,
                        ProjectStatus.Archived => DomainEntityStatus.Archived,
                        ProjectStatus.Completed => DomainEntityStatus.Completed,
                        _ => DomainEntityStatus.Active,
                    };

                    anyChanged = true;
                }

                if (anyChanged)
                {
                    await DomainService.ApplyChange(context, change);
                }

                break;

            default:
                throw new InvalidOperationException("Unexpected entity type.");
        }
    }
}

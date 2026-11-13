using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Workflow.Enums;

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
}

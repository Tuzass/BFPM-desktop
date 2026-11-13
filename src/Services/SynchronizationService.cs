using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Workflow.Enums;

namespace Services;

public static class SynchronizationService
{
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

        Dictionary<EntityIdPair, int> entityVersions = [];
        foreach (var pair in affectedEntitiesResponse.AffectedEntities)
        {
            int version = await DomainService.GetEntityVersionByRemoteID(context, pair.EntityType, pair.ID);

            if (version == -1)
            {
                throw new InvalidOperationException($"Entity ({pair.EntityType}, {pair.ID}) not found in local DB");
            }

            entityVersions[pair] = version;
        }

        var response = await apiClient.SendVersions(affectedEntitiesResponse.UserActionID, entityVersions);

        return response switch
        {
            "Approved" => true,
            "Rejected" => false,
            _ => throw new InvalidOperationException($"Unexpected user action status: {response}")
        };
    }
}

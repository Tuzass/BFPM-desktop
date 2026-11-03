using Infrastructure.Enums;

namespace Infrastructure.Entities;

public record struct EntityIdPair(int ID, DomainEntityType EntityType);

namespace Domain.Enums;

public enum SystemState
{
    None,
    UserStatusActive,
    UserStatusInactive,
    TaskStatusTodo,
    TaskStatusInProgress,
    TaskStatusCompleted,
    TaskStatusOnHold,
    TaskStatusBlocked,
    TaskPriorityLow,
    TaskPriorityMedium,
    TaskPriorityHigh,
    TaskPriorityUrgent,
    ProjectStatusActive,
    ProjectStatusOnHold,
    ProjectStatusCompleted,
    ProjectStatusArchived,
    NoUsersAssignedToInProgressTask,
    UsersAssignedToCompletedTask,
    NoTasksInProgress,
    TasksInProgress
}

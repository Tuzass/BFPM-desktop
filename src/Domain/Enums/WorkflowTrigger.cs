namespace Domain.Enums;

public enum WorkflowTrigger
{
    UserAction,
    TagCreated,
    TagDeleted,
    TagAssignedToTask,
    TagUnassignedFromTask,
    UserCreated,
    UserStatusChanged,
    UserAssignedToProject,
    UserUnassignedFromProject,
    UserAssignmentRoleChanged,
    UserAssignedToTask,
    UserUnassignedFromTask,
    TaskCreated,
    TaskDeleted,
    TaskNameChanged,
    TaskDescriptionChanged,
    TaskStatusChanged,
    TaskPriorityChanged,
    TaskDueDateChanged,
    ProjectCreated,
    ProjectDeleted,
    ProjectNameChanged,
    ProjectDescriptionChanged,
    ProjectStatusChanged
}

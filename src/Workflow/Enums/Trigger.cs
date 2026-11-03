namespace Workflow.Enums;

public enum Trigger
{
    UserAction,
    UserAssignedToProject,
    UserUnassignedFromProject,
    UserAssignmentRoleChanged,
    UserAssignedToTask,
    UserUnassignedFromTask,
    TagAssignedToTask,
    TagUnassignedFromTask,
    TagCreated,
    TagNameChanged,
    TagDeleted,
    TaskCreated,
    TaskNameChanged,
    TaskDescriptionChanged,
    TaskStatusChanged,
    TaskPriorityChanged,
    TaskDueDateChanged,
    TaskDeleted,
    ProjectUploaded,
    ProjectNameChanged,
    ProjectDescriptionChanged,
    ProjectStatusChanged,
    ProjectDeleted
}

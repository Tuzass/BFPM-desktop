namespace Workflow.Enums;

public enum ActionTarget
{
    None,
    ProjectManagers,
    AffectedProjectManagers,
    RelatedUser,
    UsersAssignedToProject,
    UsersAssignedToTask,
    RelatedTask,
    RelatedTasks,
    InProgressTasks,
    OnHoldProject,
    ActiveProject
}

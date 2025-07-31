namespace Application.DTOs.Common
{
    public class CommonMessageResponse
    {
        public const string DontHavePermission = "You don't have permission!";
        public const string NoCurrentUser = "There is no current user!";
    }

    public class DayOffMessageResponse : CommonMessageResponse
    {
        public const string RequestNotExisted = "The request did not existed!";
        public const string RequestCanceledAlready = "This request had canceled already!";
        public const string CannotAcceptRequest = "Can not accept this request!";
        public const string CannotRefuseRequest = "Can not refuse this request!";
        public const string CannotRepeatRequest = "Can not repeat this request!";
    }

    public class UserAccountMessage : CommonMessageResponse
    {
        public const string EmailNotExist = "Email does not exist!";
        public const string PasswordResetCodeInvalid = "Invalid password reset code!";
        public const string PasswordResetCodeExpire = "Password reset code has expire!";
        public const string PasswordResetTokenInvalid = "Invalid password reset token!";
        public const string WrongCurrentPassword = "Wrong current password";
    }

    public class ValueManagementMessage
    {
        public const string ValueCodeRegistered = "This code already registered";
    }

    public class TaskMessage
    {
        public const string NotAllowToView = "You are not allowed to view this task";
        public const string NotFound = "Task does not exist!";
        public const string TaskDeleted = "Task was deleted!";
        public const string TaskNameLengthExceed = "Task name exceeds 200 characters!";
        public const string TaskDescriptionLengthExceed = "Task description exceeds 1000 characters!";
    }

    public class GroupMessage
    {
        public const string NotAllowToView = "You are not allowed to view this group or this group does not exist!";
        public const string GroupNotFound = "Group does not exist!";
        public const string EmptyMember = "This group has no member!";
        public const string EmptyAdmin = "Group must have at least one Admin!";
    }
}
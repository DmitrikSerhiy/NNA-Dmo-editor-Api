namespace NNA.Api.Features.Editor.Hubs;

public interface IEditorClient {
    Task OnServerError(object notificationBody);
}
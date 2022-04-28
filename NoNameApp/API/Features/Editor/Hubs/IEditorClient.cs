using System.Threading.Tasks;

namespace API.Features.Editor.Hubs {
    public interface IEditorClient {
        Task OnServerError(object notificationBody);
    }
}
using appEmpamedic.Models;
using CommunityToolkit.Mvvm.Input;

namespace appEmpamedic.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}
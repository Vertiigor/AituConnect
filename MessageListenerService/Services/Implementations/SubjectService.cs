using MessageListenerService.Models;
using MessageListenerService.Repositories.Abstractions;
using MessageListenerService.Services.Abstractions;

namespace MessageListenerService.Services.Implementations
{
    public class SubjectService : Service<Subject>, ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        public SubjectService(ISubjectRepository subjectRepository) : base(subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }
    }
}

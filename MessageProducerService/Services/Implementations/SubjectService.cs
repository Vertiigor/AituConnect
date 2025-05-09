using MessageProducerService.Models;
using MessageProducerService.Repositories.Abstractions;
using MessageProducerService.Services.Abstractions;

namespace MessageProducerService.Services.Implementations
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

using MessageProducerService.Data;
using MessageProducerService.Models;
using MessageProducerService.Repositories.Abstractions;

namespace MessageProducerService.Repositories.Implementations
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationContext context) : base(context)
        {
        }
    }
}

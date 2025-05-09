using MessageListenerService.Data;
using MessageListenerService.Models;
using MessageListenerService.Repositories.Abstractions;

namespace MessageListenerService.Repositories.Implementations
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationContext context) : base(context)
        {
        }
    }
}

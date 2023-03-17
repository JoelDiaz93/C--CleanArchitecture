using CleanArchitecture.Domain.Common;
using System.Diagnostics.Contracts;

namespace CleanArchitecture.Domain
{
    public class Actor : BaseDomainModel
    {
        public Actor() {
            Videos = new HashSet<Video>();
        }
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
    }
}

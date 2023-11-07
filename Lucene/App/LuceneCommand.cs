using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Lucene.App
{
    public class LuceneCommand : IRequest<LuceneResponseModel>
    {
        [Required]
        public string? keyword { get; set; }
    }
}

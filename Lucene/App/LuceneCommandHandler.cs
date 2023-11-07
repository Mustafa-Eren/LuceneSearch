using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using MediatR;
using Newtonsoft.Json;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System.Collections.Generic;
using System.Linq;

namespace Lucene.App
{
    public class LuceneCommandHandler : IRequestHandler<LuceneCommand, LuceneResponseModel>
    {
        private object dyn;

        public Task<LuceneResponseModel> Handle(LuceneCommand request, CancellationToken cancellationToken)
        {
            StreamReader read = new StreamReader("listing.json");

            // Write values
            string res = read.ReadToEnd();
            lucener obj = JsonConvert.DeserializeObject<lucener>(res);
            //dynamic dyn = JsonConvert.DeserializeObject<LuceneResponseModel>(res);
            //var lstInstagramObjects = new List<Lucene>();
            var directory = new RAMDirectory();

            using (Analyzer analyzer = new StandardAnalyzer(Net.Util.Version.LUCENE_30))
            using (var writer = new IndexWriter(directory, analyzer, new IndexWriter.MaxFieldLength(1000)))
            {
                foreach (var item in obj.data.children)
                {
                    var document = new Document();

                    document.Add(new Field("name", item.data.name.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("kind", item.kind.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("subreddit", item.data.subreddit.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("subreddit_name_prefixed", item.data.subreddit_name_prefixed.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("subreddit_type", item.data.subreddit_type.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("author_fullname", item.data.author_fullname.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("author", item.data.author.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("author_flair_text", item.data.author_flair_text == null ? "--" : item.data.author_flair_text, Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("title", item.data.title.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("link_flair_css_class", item.data.link_flair_css_class.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("link_flair_text", item.data.link_flair_text.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("thumbnail", item.data.thumbnail.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("url_overridden_by_dest", item.data.url_overridden_by_dest.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("url", item.data.url.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("suggested_sort", item.data.suggested_sort.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("permalink", item.data.permalink.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("domain", item.data.domain.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                    //document.Add(new Field("FullText", String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16}", 
                    //    item.data.name, item.kind, item.data.subreddit, item.data.subreddit_name_prefixed, item.data.subreddit_type, item.data.author_fullname, item.data.author, item.data.author_flair_text,
                    //    item.data.title, item.data.link_flair_css_class, item.data.link_flair_text, item.data.thumbnail, item.data.url_overridden_by_dest, item.data.url, item.data.suggested_sort,
                    //    item.data.permalink, item.data.domain), 
                    //    Field.Store.YES, Field.Index.ANALYZED));

                    writer.AddDocument(document);
                }

                QueryParser queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "name", analyzer);
                Query query = queryParser.Parse(request.keyword);

                using (IndexSearcher indexSearcher = new IndexSearcher(directory, true))
                {
                    List<lucener> products = new List<lucener>();
                    var result = indexSearcher.Search(query, 10);

                    foreach (var loopDoc in result.ScoreDocs.OrderBy(s => s.Score))
                    {
                        Document document = indexSearcher.Doc(loopDoc.Doc);

                        products.Add(new lucener() { kind = document.Get("name") });
                    }

                    //return products;
                }

                writer.Optimize();
                writer.Flush(true, true, true);
            }

            return Task.FromResult(new LuceneResponseModel
            {
                data = (Data)dyn,
                kind = (string)dyn
            });
        }
    }
}

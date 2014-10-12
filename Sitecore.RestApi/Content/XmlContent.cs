using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;


namespace Sitecore.RestApi.Content
{
    public class XmlContent : HttpContent
    {
        private readonly MemoryStream _stream = new MemoryStream();
        public static IEnumerable<string> XmlContentTypes = new List<string> { "application/xml", "text/xml" };

        public XmlContent(XmlDocument document, string xmlContentType)
        {
            document.Save(_stream);
            _stream.Position = 0;
            Headers.ContentType = new MediaTypeHeaderValue(xmlContentType);
        }

        protected override Task SerializeToStreamAsync(Stream stream, System.Net.TransportContext context)
        {
            _stream.CopyTo(stream);

            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _stream.Length;
            return true;
        }
    }
}
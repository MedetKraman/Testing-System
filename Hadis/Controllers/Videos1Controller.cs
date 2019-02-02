using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Web.Configuration;
using System.Web.Http;
using Hadis.Models.DBModels;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Collections;
using System.Web.Mvc;

namespace Hadis.Controllers
{
    public class Videos1Controller : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public const int ReadStreamBufferSize = 1024 * 1024;
        public static IReadOnlyDictionary<string, string> MimeNames;
        public static IReadOnlyCollection<char> InvalidFileNameChars;
        public static string InitialDirectory;

        public Videos1Controller()
        {
            var mimeNames = new Dictionary<string, string>();

            mimeNames.Add(".mp3", "audio/mpeg");    // List all supported media types; 
            mimeNames.Add(".mp4", "video/mp4");
            mimeNames.Add(".ogg", "application/ogg");
            mimeNames.Add(".ogv", "video/ogg");
            mimeNames.Add(".oga", "audio/ogg");
            mimeNames.Add(".wav", "audio/x-wav");
            mimeNames.Add(".webm", "video/webm");

            MimeNames = new ReadOnlyDictionary<string, string>(mimeNames);

            InvalidFileNameChars = Array.AsReadOnly(Path.GetInvalidFileNameChars());
            InitialDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/VideoFiles");
        }

        // GET: api/Videos1
        public dynamic GetVideos()
        {
            return db.Videos.ToList().Select(u => new { Duration = "", Title = u.Title, Uploader = u.Uploader, Description = u.Description, Url = u.Id + "" }).ToList();
        }

        // GET: api/Videos1/5
        public async Task<HttpResponseMessage> GetVideo(int id)
        {
            Video video = await db.Videos.FindAsync(id);
            if (video == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }


            FileInfo fileInfo = new FileInfo(Path.Combine(InitialDirectory, video.Url));

            long totalLength = fileInfo.Length;

            RangeHeaderValue rangeHeader = base.Request.Headers.Range;
            HttpResponseMessage response = new HttpResponseMessage();

            response.Headers.AcceptRanges.Add("bytes");

            if (rangeHeader == null || !rangeHeader.Ranges.Any())
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
                =>
                {
                    using (outputStream)
                    using (Stream inputStream = fileInfo.OpenRead())
                    {
                        try
                        {
                            inputStream.CopyTo(outputStream, ReadStreamBufferSize);
                        }
                        catch (Exception error)
                        {
                            Debug.WriteLine(error);
                        }
                    }
                }, GetMimeNameFromExt(fileInfo.Extension));

                response.Content.Headers.ContentLength = totalLength;
                return response;
            }

            long start = 0, end = 0;

            if (rangeHeader.Unit != "bytes" || rangeHeader.Ranges.Count > 1 ||
                !TryReadRangeItem(rangeHeader.Ranges.First(), totalLength, out start, out end))
            {
                response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Content = new StreamContent(Stream.Null);
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
                response.Content.Headers.ContentType = GetMimeNameFromExt(fileInfo.Extension);

                return response;
            }

            var contentRange = new ContentRangeHeaderValue(start, end, totalLength);

            response.StatusCode = HttpStatusCode.PartialContent;
            response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
            =>
            {
                using (outputStream)
                using (Stream inputStream = fileInfo.OpenRead())
                    CreatePartialContent(inputStream, outputStream, start, end);

            }, GetMimeNameFromExt(fileInfo.Extension));

            response.Content.Headers.ContentLength = end - start + 1;
            response.Content.Headers.ContentRange = contentRange;

            return response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Others

        private static bool AnyInvalidFileNameChars(string fileName)
        {
            return InvalidFileNameChars.Intersect(fileName).Any();
        }

        private static MediaTypeHeaderValue GetMimeNameFromExt(string ext)
        {
            string value;

            if (MimeNames.TryGetValue(ext.ToLowerInvariant(), out value))
                return new MediaTypeHeaderValue(value);
            else
                return new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);
        }

        private static bool TryReadRangeItem(RangeItemHeaderValue range, long contentLength,
            out long start, out long end)
        {
            if (range.From != null)
            {
                start = range.From.Value;
                if (range.To != null)
                    end = range.To.Value;
                else
                    end = contentLength - 1;
            }
            else
            {
                end = contentLength - 1;
                if (range.To != null)
                    start = contentLength - range.To.Value;
                else
                    start = 0;
            }
            return (start < contentLength && end < contentLength);
        }

        private static void CreatePartialContent(Stream inputStream, Stream outputStream,
            long start, long end)
        {
            int count = 0;
            long remainingBytes = end - start + 1;
            long position = start;
            byte[] buffer = new byte[ReadStreamBufferSize];

            inputStream.Position = start;
            do
            {
                try
                {
                    if (remainingBytes > ReadStreamBufferSize)
                        count = inputStream.Read(buffer, 0, ReadStreamBufferSize);
                    else
                        count = inputStream.Read(buffer, 0, (int)remainingBytes);
                    outputStream.Write(buffer, 0, count);
                }
                catch (Exception error)
                {
                    Debug.WriteLine(error);
                    break;
                }
                position = inputStream.Position;
                remainingBytes = end - position + 1;
            } while (position <= end);
        }

        #endregion

    }
}
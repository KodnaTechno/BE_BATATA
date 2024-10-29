using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace FileStorge.Providers.Mongodb
{
    public class GridFSFileProvider : IDocumentProvider, IDisposable
    {
        private readonly IGridFSBucket _bucket;
        private readonly IDocumentVerification _documentVerification;

        public GridFSFileProvider(string connectionString, string database) : this(connectionString, database, null)
        {
        }

        public GridFSFileProvider(string connectionString, string database, IDocumentVerification documentVerification)
        {
            if (connectionString == null || database == null)
                throw new ArgumentNullException("Invalid param");

            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase(database);
            _bucket = new GridFSBucket(mongoDatabase);

            _documentVerification = documentVerification == null ? new DocumentVerification() : documentVerification;
        }


        public async Task<Stream> DownloadFile(string id)
        {
            try
            {
                var mongoId = new ObjectId();

                var isParsed = ObjectId.TryParse(id, out mongoId);
                if (!isParsed)
                    throw new InvalidOperationException("Invalid objectId");

                var bytes = await this._bucket.DownloadAsBytesAsync(mongoId);

                return new MemoryStream(bytes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FileModel UploadFile(byte[] stream, string filename, string contentType, string owner)
        {
            var verificationResult = _documentVerification.Verify(stream, filename);

            if (!verificationResult.IsValid)
                throw new InvalidDataException(verificationResult.Message);


            var file = new FileModel
            {
                Name = filename,
                ContentType = contentType,
                Size = stream.Length,
                Owner = owner,
                CreatedAt = DateTime.UtcNow
            };

            var options = new GridFSUploadOptions
            {
                ChunkSizeBytes = 64512,
                Metadata = new BsonDocument
                {
                    { "Name", filename },
                    { "ContentType", contentType },
                    { "Size", stream.Length },
                    { "Extension", file.Extension  },
                    { "CreatedAt", file.CreatedAt },
                    { "Owner", file.Owner },
                }
            };

            var mongoId = _bucket.UploadFromBytes(filename, stream, options);

            file.Id = mongoId.ToString();

            return file;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public FileModel GetMetaData(string id)
        {
            try
            {
                var mongoId = new ObjectId();

                var isParsed = ObjectId.TryParse(id, out mongoId);
                if (!isParsed)
                    throw new InvalidOperationException("Invalid objectId");

                var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, mongoId);

                var fileCursor = _bucket.Find(filter);

                fileCursor.MoveNext();

                var file = fileCursor.Current.First();

                return file.Metadata.ToBsonDocument().Meta(file.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteFile(string id)
        {
            try
            {
                var mongoId = new ObjectId();

                var isParsed = ObjectId.TryParse(id, out mongoId);
                if (!isParsed)
                    throw new InvalidOperationException("Invalid objectId");

                _bucket.Delete(mongoId);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
using Microsoft.SharePoint.Client;
using System.Net;
using System.Security;

namespace FileStorge.Providers.SharePoint
{
    public class SharePointFileProvider : IDocumentProvider, IDisposable
    {
        private readonly IDocumentVerification _documentVerification;

        private readonly string _siteUrl;
        private readonly string _documentLibrary;

        private ClientContext? _clientContext;

        private string _username;
        private string _password;

        public SharePointFileProvider(string siteUrl, string documentLibrary, string username, string password) :
            this(siteUrl, documentLibrary, username, password, null)
        {
        }

        public SharePointFileProvider(string siteUrl, string documentLibrary, string username, string password, IDocumentVerification documentVerification)
        {
            _documentLibrary = documentLibrary;
            _siteUrl = siteUrl;
            _username = username;
            _password = password;

            this._documentVerification = documentVerification == null ? new DocumentVerification() : documentVerification;
        }

        public void Dispose()
        {
            if (_clientContext != null)
                _clientContext.Dispose();

            GC.SuppressFinalize(this);
        }

        public Task<Stream> DownloadFile(string id)
        {
            Authenticate(_username, _password);

            if (_clientContext == null)
                throw new UnauthorizedAccessException();

            // get Document
            List eDocumentList = _clientContext.Web.Lists.GetByTitle(_documentLibrary);

            CamlQuery query = new CamlQuery();
            query.ViewXml = $"<View>" +
                                $"<Query>" +
                                    $"<Where>" +
                                        $"<Eq>" +
                                            $"<FieldRef Name='" + "LinkFilename" + "' />" +
                                            $"<Value Type='Text'>{id}</Value>" +
                                        $"</Eq>" +
                                    $"</Where>" +
                                $"</Query>" +
                            $"</View>";

            var document = eDocumentList.GetItems(query);
            _clientContext.Load(document, d => d.Include(f => f.File));
            _clientContext.ExecuteQuery();

            if (document.Count == 0)
                throw new FileNotFoundException();

            if (document.Count != 1)
                throw new InvalidDataException();

            var file = _clientContext.Web.GetFileByUrl(document.First().File.ServerRelativeUrl);
            _clientContext.Load(file);
            _clientContext.ExecuteQuery();
            var stream = file.OpenBinaryStream();
            _clientContext.ExecuteQuery();
            return Task.FromResult(stream.Value);
        }

        public FileModel GetMetaData(string id)
        {
            throw new NotImplementedException();
        }

        public FileModel UploadFile(byte[] stream, string filename, string contentType, string owner)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            var verificationResult = _documentVerification.Verify(stream, filename);

            if (!verificationResult.IsValid)
                throw new InvalidDataException(verificationResult.Message);

            Authenticate(_username, _password);

            if (_clientContext == null)
                throw new UnauthorizedAccessException();

            Web web = _clientContext.Web;
            _clientContext.Load(web);
            _clientContext.ExecuteQuery();

            List list = _clientContext.Web.Lists.GetByTitle(_documentLibrary);
            _clientContext.Load(list);
            _clientContext.ExecuteQuery();

            FileCreationInformation newFile = new FileCreationInformation();
            newFile.ContentStream = new MemoryStream(stream);
            newFile.Overwrite = true;
            string fileID = Guid.NewGuid().ToString();
            newFile.Url = $"{_siteUrl}/{_documentLibrary}/{fileID}";
            Microsoft.SharePoint.Client.File uploadFile = list.RootFolder.Files.Add(newFile);
            uploadFile.ListItemAllFields.Update();
            _clientContext.Load(uploadFile, f => f.ListItemAllFields);
            _clientContext.ExecuteQuery();

            return new FileModel
            {
                Name = filename,
                ContentType = contentType,
                Guid = Guid.Parse(fileID),
                Id = fileID,
                Size = stream.Length,
                Owner = owner
            };
        }

        private void Authenticate(string username, string password)
        {
            SecureString securestring = new SecureString();
            password.ToCharArray().ToList().ForEach(s => securestring.AppendChar(s));
            var clientContext = new PnP.Framework.AuthenticationManager()
                .GetOnPremisesContext(_siteUrl,
                new NetworkCredential(username, securestring));
            clientContext.ExecuteQuery();
            _clientContext = clientContext;

        }

        public bool DeleteFile(string id)
        {
            Authenticate(_username, _password);

            if (_clientContext == null)
                throw new UnauthorizedAccessException();

            // get Document
            List eDocumentList = _clientContext.Web.Lists.GetByTitle(_documentLibrary);

            CamlQuery query = new CamlQuery();
            query.ViewXml = $"<View>" +
                                $"<Query>" +
                                    $"<Where>" +
                                        $"<Eq>" +
                                            $"<FieldRef Name='" + "LinkFilename" + "' />" +
                                            $"<Value Type='Text'>{id}</Value>" +
                                        $"</Eq>" +
                                    $"</Where>" +
                                $"</Query>" +
                            $"</View>";

            var document = eDocumentList.GetItems(query);
            _clientContext.Load(document, d => d.Include(f => f.File));
            _clientContext.ExecuteQuery();

            if (document.Count == 0)
                throw new FileNotFoundException();

            if (document.Count != 1)
                throw new InvalidDataException();

            var file = _clientContext.Web.GetFileByUrl(document.First().File.ServerRelativeUrl);
            _clientContext.Load(file);
            _clientContext.ExecuteQuery();

            file.DeleteObject();
            _clientContext.ExecuteQuery();

            return true;
        }
    }
}
/*
 * IMPLEMENTED BY VRCGalleryManager
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRChat.API.Api
{
    public interface IPrintsApiSync : IApiAccessor
    {
        List<Print> GetPrints(string userId = default(string), int? n = default(int?), int? offset = default(int?));
        ApiResponse<List<Print>> GetPrintsWithHttpInfo(string userId = default(string), int? n = default(int?), int? offset = default(int?));

        // Metodi per la DELETE
        void DeletePrint(string printId);
        ApiResponse<Object> DeletePrintWithHttpInfo(string printId);
    }

    public interface IPrintsApiAsync : IApiAccessor
    {
        Task<List<Print>> GetPrintsAsync(string userId = default(string), int? n = default(int?), int? offset = default(int?), CancellationToken cancellationToken = default(CancellationToken));
        Task<ApiResponse<List<Print>>> GetPrintsWithHttpInfoAsync(string userId = default(string), int? n = default(int?), int? offset = default(int?), CancellationToken cancellationToken = default(CancellationToken));

        // Metodi per la DELETE in modalità asincrona
        Task DeletePrintAsync(string printId, CancellationToken cancellationToken = default(CancellationToken));
        Task<ApiResponse<Object>> DeletePrintWithHttpInfoAsync(string printId, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IPrintsApi : IPrintsApiSync, IPrintsApiAsync { }

    public partial class PrintsApi : IPrintsApi
    {
        private ExceptionFactory _exceptionFactory = (name, response) => null;
        public PrintsApi(VRChat.API.Client.ISynchronousClient client, VRChat.API.Client.IAsynchronousClient asyncClient, VRChat.API.Client.IReadableConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (asyncClient == null) throw new ArgumentNullException("asyncClient");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = VRChat.API.Client.Configuration.DefaultExceptionFactory;
        }

        public VRChat.API.Client.ISynchronousClient Client { get; set; }
        public VRChat.API.Client.IAsynchronousClient AsynchronousClient { get; set; }
        public string GetBasePath() => this.Configuration.BasePath;
        public VRChat.API.Client.IReadableConfiguration Configuration { get; set; }

        public VRChat.API.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        #region Synchronous Operations

        /// <summary>
        /// Recupera le prints dell’utente specificato
        /// </summary>
        /// <param name="userId">ID dell’utente</param>
        /// <param name="n">Numero di elementi da ritornare</param>
        /// <param name="offset">Offset per la paginazione</param>
        /// <returns>Lista di Print</returns>
        public List<Print> GetPrints(string userId = default(string), int? n = default(int?), int? offset = default(int?))
        {
            ApiResponse<List<Print>> localVarResponse = GetPrintsWithHttpInfo(userId, n, offset);
            return localVarResponse.Data;
        }

        public ApiResponse<List<Print>> GetPrintsWithHttpInfo(string userId, int? n = default(int?), int? offset = default(int?))
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Il parametro userId è obbligatorio per questa richiesta API.");

            VRChat.API.Client.RequestOptions localVarRequestOptions = new VRChat.API.Client.RequestOptions();

            string[] _contentTypes = new string[] { };
            string[] _accepts = new string[] { "application/json" };

            var localVarContentType = VRChat.API.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            var localVarAccept = VRChat.API.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            if (n.HasValue)
                localVarRequestOptions.QueryParameters.Add(VRChat.API.Client.ClientUtils.ParameterToMultiMap("", "n", n.Value));
            if (offset.HasValue)
                localVarRequestOptions.QueryParameters.Add(VRChat.API.Client.ClientUtils.ParameterToMultiMap("", "offset", offset.Value));

            localVarRequestOptions.Operation = "PrintsApi.GetPrints";
            localVarRequestOptions.OperationIndex = 0;

            if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("auth")))
                localVarRequestOptions.Cookies.Add(new Cookie("auth", this.Configuration.GetApiKeyWithPrefix("auth"), "/", "vrchat.com"));

            // Costruisce correttamente l'URL con userId
            string endpoint = $"/prints/user/{userId}";

            var response = this.Client.Get<object>(endpoint, localVarRequestOptions, this.Configuration);

            string jsonResponse = JsonConvert.SerializeObject(response.Data);
            Console.WriteLine($"[DEBUG] Risposta API: {jsonResponse}");

            if (jsonResponse.StartsWith("{"))
            {
                throw new Exception($"Errore API: {jsonResponse}");
            }

            var dataList = JsonConvert.DeserializeObject<List<Print>>(jsonResponse);
            return new ApiResponse<List<Print>>(response.StatusCode, response.Headers, dataList);
        }

        /// <summary>
        /// Elimina la print specificata dall’ID
        /// </summary>
        /// <param name="printId">ID della print da eliminare</param>
        public void DeletePrint(string printId)
        {
            DeletePrintWithHttpInfo(printId);
        }

        /// <summary>
        /// Elimina la print specificata dall’ID e restituisce le informazioni HTTP
        /// </summary>
        /// <param name="printId">ID della print da eliminare</param>
        /// <returns>ApiResponse contenente i dettagli della risposta</returns>
        public ApiResponse<Object> DeletePrintWithHttpInfo(string printId)
        {
            if (string.IsNullOrEmpty(printId))
                throw new ArgumentException("Il parametro printId è obbligatorio per questa richiesta API.");

            VRChat.API.Client.RequestOptions localVarRequestOptions = new VRChat.API.Client.RequestOptions();

            string[] _contentTypes = new string[] { };
            string[] _accepts = new string[] { "application/json" };

            var localVarContentType = VRChat.API.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            var localVarAccept = VRChat.API.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.Operation = "PrintsApi.DeletePrint";
            localVarRequestOptions.OperationIndex = 1;

            if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("auth")))
                localVarRequestOptions.Cookies.Add(new Cookie("auth", this.Configuration.GetApiKeyWithPrefix("auth"), "/", "vrchat.com"));

            // Costruisce l'endpoint con printId
            string endpoint = $"/prints/{printId}";

            // Effettua la chiamata DELETE
            var response = this.Client.Delete<object>(endpoint, localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("DeletePrint", response);
                if (exception != null) throw exception;
            }

            return response;
        }

        #endregion Synchronous Operations

        #region Asynchronous Operations

        public async Task<List<Print>> GetPrintsAsync(string userId = default(string), int? n = default(int?), int? offset = default(int?), CancellationToken cancellationToken = default(CancellationToken))
        {
            ApiResponse<List<Print>> localVarResponse = await GetPrintsWithHttpInfoAsync(userId, n, offset, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        public async Task<ApiResponse<List<Print>>> GetPrintsWithHttpInfoAsync(string userId, int? n = default(int?), int? offset = default(int?), CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Il parametro userId è obbligatorio per questa richiesta API.");

            VRChat.API.Client.RequestOptions localVarRequestOptions = new VRChat.API.Client.RequestOptions();

            string[] _contentTypes = new string[] { };
            string[] _accepts = new string[] { "application/json" };

            var localVarContentType = VRChat.API.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            var localVarAccept = VRChat.API.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            if (n.HasValue)
                localVarRequestOptions.QueryParameters.Add(VRChat.API.Client.ClientUtils.ParameterToMultiMap("", "n", n.Value));
            if (offset.HasValue)
                localVarRequestOptions.QueryParameters.Add(VRChat.API.Client.ClientUtils.ParameterToMultiMap("", "offset", offset.Value));

            localVarRequestOptions.Operation = "PrintsApi.GetPrints";
            localVarRequestOptions.OperationIndex = 0;

            if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("auth")))
                localVarRequestOptions.Cookies.Add(new Cookie("auth", this.Configuration.GetApiKeyWithPrefix("auth"), "/", "vrchat.com"));

            string endpoint = $"/prints/user/{userId}";

            var response = await this.AsynchronousClient
                .GetAsync<object>(endpoint, localVarRequestOptions, this.Configuration, cancellationToken)
                .ConfigureAwait(false);

            string jsonResponse = JsonConvert.SerializeObject(response.Data);
            Console.WriteLine($"[DEBUG] Risposta API: {jsonResponse}");

            if (jsonResponse.StartsWith("{"))
            {
                throw new Exception($"Errore API: {jsonResponse}");
            }

            var dataList = JsonConvert.DeserializeObject<List<Print>>(jsonResponse);
            return new ApiResponse<List<Print>>(response.StatusCode, response.Headers, dataList);
        }

        public async Task DeletePrintAsync(string printId, CancellationToken cancellationToken = default(CancellationToken))
        {
            await DeletePrintWithHttpInfoAsync(printId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponse<Object>> DeletePrintWithHttpInfoAsync(string printId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(printId))
                throw new ArgumentException("Il parametro printId è obbligatorio per questa richiesta API.");

            VRChat.API.Client.RequestOptions localVarRequestOptions = new VRChat.API.Client.RequestOptions();

            string[] _contentTypes = new string[] { };
            string[] _accepts = new string[] { "application/json" };

            var localVarContentType = VRChat.API.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            var localVarAccept = VRChat.API.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.Operation = "PrintsApi.DeletePrint";
            localVarRequestOptions.OperationIndex = 1;

            if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("auth")))
                localVarRequestOptions.Cookies.Add(new Cookie("auth", this.Configuration.GetApiKeyWithPrefix("auth"), "/", "vrchat.com"));

            string endpoint = $"/prints/{printId}";

            var response = await this.AsynchronousClient
                .DeleteAsync<object>(endpoint, localVarRequestOptions, this.Configuration, cancellationToken)
                .ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("DeletePrint", response);
                if (exception != null) throw exception;
            }

            return response;
        }

        #endregion Asynchronous Operations
    }
}
